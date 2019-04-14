using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatafeedController : Controller
    {
        private ICoinService CoinService { get; set; }
        private IOrderService OrderService { get; set; }

        public DatafeedController([FromServices]
            ICoinService coinService,
            IOrderService orderService)
        {
            CoinService = coinService;
            OrderService = orderService;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            return Ok(new
            {
                supported_resolutions = new[] { "1", "5", "15", "30", "60", "1D" },
                supports_group_request = false,
                supports_marks = false,
                supports_search = true,
                supports_timescale_marks = false
            });
        }

        [HttpGet("time")]
        public async Task<IActionResult> GetServerTime()
        {
            return Ok(DateToUnixTimestamp(DateTime.UtcNow));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string symbol, [FromQuery] string resolution)
        {
            var coins = symbol.Split("/");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Closed)
                    .ToList();

            if (orders == null || orders.Count == 0)
                return Ok(new
                {
                    s = "no_data",
                    nextTime = DateToUnixTimestamp(DateTime.UtcNow) + 60
                });

            var startTime = DateToUnixTimestamp(orders.Min(x => x.Time));
            var endTime = DateToUnixTimestamp(orders.Max(x => x.Time));
            var interval = 60;

            if (resolution == "1")
                interval *= 1;
            else if (resolution == "5")
                interval *= 5;
            else if (resolution == "15")
                interval *= 15;
            else if (resolution == "30")
                interval *= 30;
            else if (resolution == "60")
                interval *= 60;
            else if (resolution == "D" || resolution == "1D")
                interval *= (60 * 24);
            else
                interval *= 1;

            var t = new List<double>() { startTime };
            var tempT = startTime;

            while (tempT < endTime)
            {
                t.Add(tempT + interval);
                tempT += interval;
            }

            var o = new List<double>();
            t.ForEachNext((item, next) =>
            {
                var op = orders.FirstOrDefault(x => DateToUnixTimestamp(x.Time) >= item &&
                        DateToUnixTimestamp(x.Time) <= next);

                if (op != null)
                    o.Add(op.Price);
            });

            var c = new List<double>();
            t.ForEachNext((item, next) =>
            {
                var oc = orders.AsEnumerable().LastOrDefault(x => DateToUnixTimestamp(x.Time) >= item &&
                        DateToUnixTimestamp(x.Time) <= next);

                if (oc != null)
                    c.Add(oc.Price);
            });

            var h = new List<double>();
            t.ForEachNext((item, next) =>
            {
                var oh = orders.Where(x => DateToUnixTimestamp(x.Time) >= item &&
                        DateToUnixTimestamp(x.Time) <= next)
                        .ToList();

                if (oh != null && oh.Count != 0)
                    h.Add(oh.Max(x => x.Price));
            });

            var l = new List<double>();
            t.ForEachNext((item, next) =>
            {
                var ol = orders.Where(x => DateToUnixTimestamp(x.Time) >= item &&
                                        DateToUnixTimestamp(x.Time) <= next)
                                        .ToList();

                if (ol != null && ol.Count != 0)
                    l.Add(ol.Min(x => x.Price));
            });

            var v = new List<double>();
            t.ForEachNext((item, next) =>
            {
                var ov = orders.Where(x => DateToUnixTimestamp(x.Time) >= item &&
                        DateToUnixTimestamp(x.Time) <= next)
                        .ToList();

                if (ov != null && ov.Count != 0)
                    v.Add(ov.Sum(x => x.Amount));
            });

            if (t.Count != v.Count)
            {
                while (t.Count != v.Count)
                    t.Remove(t.AsEnumerable().Last());
            }

            return Ok(new { t, o, c, h, l, v, s = "ok" });
        }

        [HttpGet("symbols")]
        public async Task<IActionResult> GetSymbol([FromQuery] string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            var symbolResponse = new SymbolResponse
            {
                Description = $"{coins[0]}/{coins[1]}",
                ExchangeListed = "GOLD.IO",
                ExchangeTraded = "GOLD.IO",
                HasIntraday = true,
                HasNoVolume = false,
                Minmov = 1,
                Name = $"{coins[0]}/{coins[1]}",
                PointValue = 1,
                Pricescale = 100000000,
                Session = "24x7",
                Ticker = $"{coins[0]}/{coins[1]}"
            };

            return Ok(JsonConvert.SerializeObject(symbolResponse));
        }

        class SymbolResponse
        {
            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("exchange-listed")]
            public string ExchangeListed { get; set; }

            [JsonProperty("exchange-traded")]
            public string ExchangeTraded { get; set; }

            [JsonProperty("has_intraday")]
            public bool HasIntraday { get; set; }

            [JsonProperty("has_no_volume")]
            public bool HasNoVolume { get; set; }

            [JsonProperty("minmov")]
            public int Minmov { get; set; }

            [JsonProperty("minmov2")]
            public int Minmov2 { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("pointvalue")]
            public double PointValue { get; set; }

            [JsonProperty("pricescale")]
            public int Pricescale { get; set; }

            [JsonProperty("session")]
            public string Session { get; set; }

            [JsonProperty("supported_resolutions")]
            public string[] SupportedResolutions { get; set; } = new string[] { "1", "5", "15", "30", "60", "1D" };

            [JsonProperty("ticker")]
            public string Ticker { get; set; }

            [JsonProperty("timezone")]
            public string Timezone { get; set; } = "Etc/UTC";

            [JsonProperty("type")]
            public string Type { get; set; } = "crypto";
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}