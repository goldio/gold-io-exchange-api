using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
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
                supported_resolutions = new[] { "1", "5", "15", "30", "60", "1D", "1W", "1M" },
                supports_group_request = false,
                supports_marks = false,
                supports_search = true,
                supports_timescale_marks = false
            });
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
                HasIntraday = false,
                HasNoVolume = false,
                Minmov = 1,
                Minmov2 = 0,
                Name = $"{coins[0]}/{coins[1]}",
                PointValue = 1,
                Pricescale = 100,
                Session = "0930-1630",
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
            public int PointValue { get; set; }

            [JsonProperty("pricescale")]
            public int Pricescale { get; set; }

            [JsonProperty("session")]
            public string Session { get; set; }

            [JsonProperty("supported_resolutions")]
            public string[] SupportedResolutions { get; set; } = new string[] { "D", "2D", "3D", "W", "3W", "M", "6M" };

            [JsonProperty("ticker")]
            public string Ticker { get; set; }

            [JsonProperty("timezone")]
            public string Timezone { get; set; } = "Etc/UTC";

            [JsonProperty("type")]
            public string Type { get; set; } = "bitcoin";
        }
    }
}