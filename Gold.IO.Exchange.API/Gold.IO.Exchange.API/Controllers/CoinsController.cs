using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Coin;
using Gold.IO.Exchange.API.Domain.Enum;
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
        private IOrderService OrderService { get; set; }

        public CoinsController([FromServices]
            ICoinService coinService,
            IOrderService orderService)
        {
            CoinService = coinService;
            OrderService = orderService;
        }

        [HttpGet("startData")]
        public async Task<IActionResult> StartData()
        {
            var btcCoin = new Coin
            {
                Name = "Bitcoin",
                ShortName = "BTC",
                IsCrypto = true
            };

            var ethCoin = new Coin
            {
                Name = "Ethereum",
                ShortName = "ETH",
                IsCrypto = true
            };

            var eosCoin = new Coin
            {
                Name = "EOS",
                ShortName = "EOS",
                IsCrypto = true
            };

            var gioCoin = new Coin
            {
                Name = "GOLD.IO",
                ShortName = "GIO",
                IsCrypto = true
            };

            CoinService.Create(btcCoin);
            CoinService.Create(ethCoin);
            CoinService.Create(eosCoin);
            CoinService.Create(gioCoin);

            return Ok();
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

            if (coins == null || coins.Count < 2)
                return Json(new ResponseModel { Success = false, Message = "Coins list is empty or < 2" });

            var pairs = new HashSet<PairViewModel>();
            for (var i = 0; i < coins.Count -1; i++)
            {
                for (var j = 1; j < coins.Count; j++)
                {
                    if (!coins[i].Equals(coins[j]) && j > i)
                        pairs.Add(new PairViewModel(coins[i], coins[j]));
                }
                
            }

            return Json(new DataResponse<HashSet<PairViewModel>> { Data = pairs });
        }

        [HttpGet("price/{baseAsset}/{quoteAsset}")]
        public async Task<IActionResult> GetPriceByPair(string baseAsset, string quoteAsset)
        {
            var baseAssetCoin = CoinService.GetAll().FirstOrDefault(x => x.ShortName == baseAsset);
            if (baseAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Base asset not found"
                });

            var quoteAssetCoin = CoinService.GetAll().FirstOrDefault(x => x.ShortName == quoteAsset);
            if (quoteAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Quote asset not found"
                });

            var priceOrder = OrderService.GetAll().FirstOrDefault(x => x.Status == OrderStatus.Closed);
            if (priceOrder == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Closed orders not found"
                });

            return Json(new DataResponse<double> { Data = priceOrder.Price });
        }

    }
}