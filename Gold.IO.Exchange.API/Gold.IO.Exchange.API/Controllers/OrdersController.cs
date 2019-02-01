using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private IOrderService OrderService { get; set; }
        private ICoinService CoinService { get; set; }
        private IUserService UserService { get; set; }

        public OrdersController([FromServices]
            IOrderService orderService,
            ICoinService coinService,
            IUserService userService)
        {
            OrderService = orderService;
            CoinService = coinService;
            UserService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var orders = OrderService.GetAll()
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            var order = OrderService.Get(id);
            if (order == null)
                return Json(new ResponseModel { Success = false, Message = "Order not found" });

            return Json(new DataResponse<OrderViewModel> { Data = new OrderViewModel(order) });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var baseAssetCoin = CoinService.GetAll().FirstOrDefault(x => x.ShortName == request.BaseAsset);
            if (baseAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Base asset not found"
                });

            var quoteAssetCoin = CoinService.GetAll().FirstOrDefault(x => x.ShortName == request.QuoteAsset);
            if (quoteAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Quote asset not found"
                });

            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var order = new Order
            {
                User = user,
                BaseAsset = baseAssetCoin,
                QuoteAsset = quoteAssetCoin,
                Amount = request.Amount,
                Balance = request.Amount,
                Price = request.Price,
                Status = OrderStatus.Open,
                Type = request.Type,
                Time = DateTime.UtcNow
            };

            OrderService.Create(order);

            return Json(new ResponseModel());
        }

        [HttpGet("symbol/{baseAsset}/{quoteAsset}/trades")]
        public async Task<IActionResult> GetOrdersBySymbol(string baseAsset, string quoteAsset)
        {
            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset.ShortName.Equals(baseAsset) &&
                    x.QuoteAsset.ShortName.Equals(quoteAsset) &&
                    x.Status == OrderStatus.Closed)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }

        [HttpGet("symbol/{baseAsset}/{quoteAsset}/book")]
        public async Task<IActionResult> GetOrderBookBySymbol(string baseAsset, string quoteAsset)
        {
            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset.ShortName.Equals(baseAsset) &&
                    x.QuoteAsset.ShortName.Equals(quoteAsset) &&
                    x.Status == OrderStatus.Open)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }
    }
}