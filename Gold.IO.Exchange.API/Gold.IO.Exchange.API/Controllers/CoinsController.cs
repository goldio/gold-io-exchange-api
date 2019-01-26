using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinsController : Controller
    {
        private ICoinService CoinService { get; set; }

        public CoinsController([FromServices]
            ICoinService coinService)
        {
            CoinService = coinService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coins = CoinService.GetAll()
                .Select(x => new CoinViewModel(x))
                .ToList();

            return Json(new DataResponse<List<CoinViewModel>> { Data = coins });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var coin = CoinService.Get(id);
            if (coin == null)
                return Json(new ResponseModel {
                    Success = false,
                    Message = "Coin not found"
                });

            return Json(new DataResponse<CoinViewModel> { Data = new CoinViewModel(coin) });
        }
    }
}