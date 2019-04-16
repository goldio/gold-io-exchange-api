using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Binance.Net;
using Gold.IO.Exchange.API.ViewModels.Response;
using Binance.Net.Objects;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceController : Controller
    {
        private ICoinService CoinService { get; set; }
        private IOrderService OrderService { get; set; }

        public BinanceController([FromServices]
            ICoinService coinService,
            IOrderService orderService)
        {
            CoinService = coinService;
            OrderService = orderService;
        }

        [HttpGet("exchangeInfo")]
        public async Task<IActionResult> GetExchangeInfo()
        {
            using (var client = new BinanceClient())
            {
                client.SetApiCredentials(
                    "ryHIGtf0risXmrDLlsorJgtCCp395HGtEWdRIOETMcLJq45AbK5hFx4xDYt8p0aE", 
                    "ZOONzdSsQFG1opcll62ueeU8Vn4wInrHTyxUbY3kyk4HjNEHBLBc3Jf4FUcjxx4X"
                );

                var result = client.GetExchangeInfo();

                if (!result.Success)
                    return Json(new ResponseModel {
                        Success = result.Success,
                        Message = result.Error.Message
                    });

                return Json(new DataResponse<BinanceSymbol[]> {
                    Success = result.Success,
                    Message = "OK",
                    Data = result.Data.Symbols
                });
            }
        }

        [HttpGet("trades/{symbol}")]
        public async Task<IActionResult> GetTrades(string symbol)
        {
            using (var client = new BinanceClient())
            {
                client.SetApiCredentials(
                    "ryHIGtf0risXmrDLlsorJgtCCp395HGtEWdRIOETMcLJq45AbK5hFx4xDYt8p0aE", 
                    "ZOONzdSsQFG1opcll62ueeU8Vn4wInrHTyxUbY3kyk4HjNEHBLBc3Jf4FUcjxx4X"
                );
                
                var result = await client.GetHistoricalTradesAsync(symbol);

                if (!result.Success)
                    return Json(new ResponseModel {
                        Success = result.Success,
                        Message = result.Error.Message
                    });

                return Json(new DataResponse<BinanceRecentTrade[]> {
                    Success = result.Success,
                    Message = "OK",
                    Data = result.Data
                });
            }
        }

        [HttpGet("klines/{symbol}")]
        public async Task<IActionResult> GetKlines(string symbol)
        {
            using (var client = new BinanceClient())
            {
                client.SetApiCredentials(
                    "ryHIGtf0risXmrDLlsorJgtCCp395HGtEWdRIOETMcLJq45AbK5hFx4xDYt8p0aE", 
                    "ZOONzdSsQFG1opcll62ueeU8Vn4wInrHTyxUbY3kyk4HjNEHBLBc3Jf4FUcjxx4X"
                );

                var result = await client.GetKlinesAsync(symbol, KlineInterval.OneWeek);

                if (!result.Success)
                    return Json(new ResponseModel {
                        Success = result.Success,
                        Message = result.Error.Message
                    });

                return Json(new DataResponse<BinanceKline[]> {
                    Success = result.Success,
                    Message = "OK",
                    Data = result.Data
                });
            }
        }

        [HttpGet("depth/{symbol}")]
        public async Task<IActionResult> GetOrderBook(string symbol)
        {
            var coins = symbol.Split(".");

            var baseCoin = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteCoin = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseCoin == null || quoteCoin == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var result = new BinanceOrderBook()
            {
                Symbol = symbol,
                Asks = new List<BinanceOrderBookEntry>(),
                Bids = new List<BinanceOrderBookEntry>()
            };

            var orders = OrderService.GetAll()
                .Where(x => x.Status == Domain.Enum.OrderStatus.Open)
                .ToList();

            foreach (var order in orders)
                if (order.Type == Domain.Enum.OrderType.Buy)
                    result.Asks.Add(new BinanceOrderBookEntry
                    {
                        Price = (decimal)order.Price,
                        Quantity = (decimal)order.Balance
                    });
                else if (order.Type == Domain.Enum.OrderType.Sell)
                    result.Bids.Add(new BinanceOrderBookEntry
                    {
                        Price = (decimal)order.Price,
                        Quantity = (decimal)order.Balance
                    });

            return Json(new DataResponse<BinanceOrderBook>
            {
                Success = true,
                Message = "OK",
                Data = result
            });
        }
    }
}