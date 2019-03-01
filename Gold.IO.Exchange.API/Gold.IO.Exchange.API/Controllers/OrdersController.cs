using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.Order;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
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
        private IUserWalletService UserWalletService { get; set; }

        public OrdersController([FromServices]
            IOrderService orderService,
            ICoinService coinService,
            IUserService userService,
            IUserWalletService userWalletService)
        {
            OrderService = orderService;
            CoinService = coinService;
            UserService = userService;
            UserWalletService = userWalletService;
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

            var buyOrders = OrderService.GetAll().Where(x => x.Type == OrderType.Buy && x.Status == OrderStatus.Open && x.User != user).ToList();
            var sellOrders = OrderService.GetAll().Where(x => x.Type == OrderType.Sell && x.Status == OrderStatus.Open && x.User != user).ToList();

            if (order.Type == OrderType.Buy)
            {
                var sellOrder = sellOrders.FirstOrDefault(x => x.Price <= order.Price);
                if (sellOrder != null)
                {
                    var buyWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == order.User && x.Coin == order.BaseAsset);
                    var buyWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == order.User && x.Coin == order.QuoteAsset);

                    var sellWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == sellOrder.User && x.Coin == sellOrder.BaseAsset);
                    var sellWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == sellOrder.User && x.Coin == sellOrder.QuoteAsset);

                    if (sellOrder.Balance > order.Balance)
                    {
                        buyWalletQuote.Balance += sellOrder.Balance;
                        buyWalletBase.Balance -= sellOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * sellOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        sellOrder.Balance -= order.Balance;
                        order.Balance = 0;
                        order.Status = OrderStatus.Closed;

                        if (sellOrder.Balance == 0)
                            sellOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(sellOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                    else if (sellOrder.Balance < order.Balance)
                    {
                        buyWalletQuote.Balance += sellOrder.Balance;
                        buyWalletBase.Balance -= sellOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * sellOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        order.Balance -= sellOrder.Balance;
                        if (order.Balance == 0)
                            order.Status = OrderStatus.Closed;

                        sellOrder.Balance = 0;
                        sellOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(sellOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                    else if (sellOrder.Balance == order.Balance)
                    {
                        buyWalletQuote.Balance += sellOrder.Balance;
                        buyWalletBase.Balance -= sellOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * sellOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        order.Balance = 0;
                        order.Status = OrderStatus.Closed;

                        sellOrder.Balance = 0;
                        sellOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(sellOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                }
            }
            else if (order.Type == OrderType.Sell)
            {
                var buyOrder = buyOrders.FirstOrDefault(x => x.Price <= order.Price);
                if (buyOrder != null)
                {
                    var sellWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == order.User && x.Coin == order.BaseAsset);
                    var sellWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == order.User && x.Coin == order.QuoteAsset);

                    var buyWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == buyOrder.User && x.Coin == buyOrder.BaseAsset);
                    var buyWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == buyOrder.User && x.Coin == buyOrder.QuoteAsset);

                    if (buyOrder.Balance > order.Balance)
                    {
                        buyWalletBase.Balance += buyOrder.Balance;
                        buyWalletQuote.Balance -= buyOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * buyOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        buyOrder.Balance -= order.Balance;
                        order.Balance = 0;
                        order.Status = OrderStatus.Closed;

                        if (buyOrder.Balance == 0)
                            buyOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(buyOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                    else if (buyOrder.Balance < order.Balance)
                    {
                        buyWalletBase.Balance += buyOrder.Balance;
                        buyWalletQuote.Balance -= buyOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * buyOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        order.Balance -= buyOrder.Balance;
                        if (order.Balance == 0)
                            order.Status = OrderStatus.Closed;

                        buyOrder.Balance = 0;
                        buyOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(buyOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                    else if (buyOrder.Balance == order.Balance)
                    {
                        buyWalletBase.Balance += buyOrder.Balance;
                        buyWalletQuote.Balance -= buyOrder.Balance * order.Price;

                        sellWalletQuote.Balance += order.Balance * buyOrder.Price;
                        sellWalletBase.Balance -= order.Balance;

                        order.Balance = 0;
                        order.Status = OrderStatus.Closed;

                        buyOrder.Balance = 0;
                        buyOrder.Status = OrderStatus.Closed;

                        OrderService.Update(order);
                        OrderService.Update(buyOrder);

                        UserWalletService.Update(buyWalletQuote);
                        UserWalletService.Update(buyWalletBase);

                        UserWalletService.Update(sellWalletBase);
                        UserWalletService.Update(sellWalletQuote);
                    }
                }
            }

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