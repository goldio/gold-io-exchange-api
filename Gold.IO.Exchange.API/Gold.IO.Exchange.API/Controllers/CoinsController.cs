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

        [HttpGet("pairs")]
        public async Task<IActionResult> GetPairs()
        {
            var coins = CoinService.GetAll()
                .Select(x => new CoinViewModel(x))
                .ToList();

            if (coins == null || coins.Count == 0)
                return Json(new ResponseModel { Success = false, Message = "Coins list is empty" });

            var pairs = new List<PairViewModel>();
            for (var i = 0; i < coins.Count; i++)
            {
                var list = coins.Skip(i).ToList();
                for (var j = 0; j < list.Count; j++)
                {
                    if (j != list.Count - 1)
                    {
                        pairs.Add(new PairViewModel(list[j], list[j + 1]));
                    }
                }
                
            }

            return Json(new DataResponse<HashSet<PairViewModel>> { Data = new HashSet<PairViewModel>(pairs) });
        }
    }
}