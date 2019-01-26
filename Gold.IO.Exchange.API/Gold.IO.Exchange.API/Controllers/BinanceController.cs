using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Binance.Net;
using Gold.IO.Exchange.API.ViewModels.Response;
using Binance.Net.Objects;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceController : Controller
    {
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
            using (var client = new BinanceClient())
            {
                client.SetApiCredentials(
                    "ryHIGtf0risXmrDLlsorJgtCCp395HGtEWdRIOETMcLJq45AbK5hFx4xDYt8p0aE", 
                    "ZOONzdSsQFG1opcll62ueeU8Vn4wInrHTyxUbY3kyk4HjNEHBLBc3Jf4FUcjxx4X"
                );

                var result = await client.GetOrderBookAsync(symbol, 1000);

                if (!result.Success)
                    return Json(new ResponseModel {
                        Success = result.Success,
                        Message = result.Error.Message
                    });

                return Json(new DataResponse<BinanceOrderBook> {
                    Success = result.Success,
                    Message = "OK",
                    Data = result.Data
                });
            }
        }
    }
}