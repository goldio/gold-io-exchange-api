using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet("symbols")]
        //public async Task<IActionResult> GetSymbol([FromQuery] string symbol)
        //{
        //    var coins = symbol.Split(".");

        //    var baseAsset = CoinService.GetAll()
        //        .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

        //    var quoteAsset = CoinService.GetAll()
        //        .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

        //    new
        //    {
        //        description = ""
        //    }
        //}
    }
}