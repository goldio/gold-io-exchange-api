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
using Gold.IO.Exchange.API.ViewModels.WebSocket;
using Gold.IO.Exchange.API.WebSocketManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : Controller
    {
        private IUserService UserService { get; set; }
        private ICoinService CoinService { get; set; }
        private IOrderService OrderService { get; set; }
        private IUserWalletService UserWalletService { get; set; }

        private NotificationsMessageHandler WebSocketService { get; set; }

        public TradeController([FromServices]
            IUserService userService,
            ICoinService coinService,
            IOrderService orderService,
            IUserWalletService userWalletService,
            NotificationsMessageHandler webSocketService)
        {
            UserService = userService;
            CoinService = coinService;
            OrderService = orderService;
            UserWalletService = userWalletService;
            WebSocketService = webSocketService;
        }

        [HttpPost("orders")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request.Price == 0 || request.Amount == 0)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Price and Amount must be not 0"
                });

            var baseAssetCoin = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName == request.BaseAsset);

            if (baseAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Base asset not found"
                });

            var quoteAssetCoin = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName == request.QuoteAsset);

            if (quoteAssetCoin == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Quote asset not found"
                });

            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (request.Type == OrderType.Buy)
            {
                var wallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == user &&
                        x.Coin == quoteAssetCoin);

                if (wallet.Balance < (request.Amount * request.Price))
                    return BadRequest(new ResponseModel
                    {
                        Success = false,
                        Message = "Insuficcient funds"
                    });
            }

            if (request.Type == OrderType.Sell)
            {
                var wallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == user &&
                        x.Coin == baseAssetCoin);

                if (wallet.Balance < request.Amount)
                    return BadRequest(new ResponseModel
                    {
                        Success = false,
                        Message = "Insuficcient funds"
                    });
            }

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

            var price = new PriceViewModel
            {
                Price = GetCurrentPrice(request.BaseAsset, request.QuoteAsset),
                IsHigher = true
            };

            OrderService.Create(order);

            var websocketUsers = WebSocketService.orderBookSubscribers
                .Where(x => x.Pairs.Contains($"{order.BaseAsset.ShortName}/{order.QuoteAsset.ShortName}"))
                .ToList();

            foreach (var wsUser in websocketUsers)
            {
                WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                {
                    Type = "orderBookUpdate",
                    Message = "orderBookUpdate"
                })).Wait();
            }

            var openOrders = OrderService.GetAll()
                .Where(x => x.ID != order.ID &&
                    x.Status == OrderStatus.Open);

            Order toCloseOrder;

            if (order.Type == OrderType.Buy)
                toCloseOrder = openOrders
                    .FirstOrDefault(x => x.Type == OrderType.Sell &&
                        x.Price <= order.Price && x.Balance > 0);
            else if (order.Type == OrderType.Sell)
                toCloseOrder = openOrders
                    .FirstOrDefault(x => x.Type == OrderType.Buy &&
                        x.Price >= order.Price && x.Balance > 0);
            else
                toCloseOrder = null;

            if (toCloseOrder != null)
                CompareTwoOrders(order, toCloseOrder, price);

            return Json(new ResponseModel());
        }

        private void CompareTwoOrders(Order order1, Order order2, PriceViewModel price)
        {
            var orders = new List<Order>
            {
                order1,
                order2
            };

            var buyOrder = orders.FirstOrDefault(x => x.Type == OrderType.Buy);
            var sellOrder = orders.FirstOrDefault(x => x.Type == OrderType.Sell);

            if (buyOrder == null || sellOrder == null)
                return;

            var buyerAccrualWallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == buyOrder.User &&
                        x.Coin == buyOrder.BaseAsset);

            var buyerWriteOffWallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == buyOrder.User &&
                        x.Coin == buyOrder.QuoteAsset);

            var sellerAccrualWallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == sellOrder.User &&
                        x.Coin == sellOrder.QuoteAsset);

            var sellerWriteOffWallet = UserWalletService.GetAll()
                    .FirstOrDefault(x => x.User == sellOrder.User &&
                        x.Coin == sellOrder.BaseAsset);

            if (buyOrder.Balance > sellOrder.Balance)
            {
                var buyOrderBalance = Math.Round(buyOrder.Balance, 8) - Math.Round(sellOrder.Balance, 8);
                var sellOrderBalance = 0;

                var buyerAccrual = Math.Round(sellOrder.Balance, 8);
                var buyerWriteOff = Math.Round(sellOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);

                var sellerAccrual = Math.Round(sellOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);
                var sellerWriteOff = Math.Round(sellOrder.Balance, 8);

                buyOrder.Balance = buyOrderBalance;
                OrderService.Update(buyOrder);

                sellOrder.Balance = sellOrderBalance;
                sellOrder.Status = OrderStatus.Closed;
                OrderService.Update(sellOrder);

                buyerAccrualWallet.Balance = Math.Round(buyerAccrualWallet.Balance, 8) + buyerAccrual;
                UserWalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                UserWalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                UserWalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                UserWalletService.Update(sellerWriteOffWallet);

                var websocketUsers = WebSocketService.orderBookSubscribers
                .Where(x => x.Pairs.Contains($"{sellOrder.BaseAsset.ShortName}/{sellOrder.QuoteAsset.ShortName}"))
                .ToList();

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "orderBookUpdate",
                        Message = "orderBookUpdate"
                    })).Wait();
                }

                if (buyOrder.Price >= price.Price)
                    price.IsHigher = true;
                else
                    price.IsHigher = false;

                price.Price = buyOrder.Price;

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "priceUpdate",
                        Message = "priceUpdate"
                    })).Wait();
                }

                var openOrders = OrderService.GetAll()
                    .Where(x => x.ID != buyOrder.ID &&
                        x.Status == OrderStatus.Open);

                var toCloseOrder = openOrders
                        .FirstOrDefault(x => x.Type == OrderType.Sell &&
                            x.Price <= buyOrder.Price && x.Balance > 0);

                if (toCloseOrder != null)
                    CompareTwoOrders(buyOrder, toCloseOrder, price);

                return;
            }

            if (buyOrder.Balance == sellOrder.Balance)
            {
                var buyOrderBalance = 0;
                var sellOrderBalance = 0;

                var buyerAccrual = Math.Round(sellOrder.Balance, 8);
                var buyerWriteOff = Math.Round(sellOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);

                var sellerAccrual = Math.Round(sellOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);
                var sellerWriteOff = Math.Round(sellOrder.Balance, 8);

                buyOrder.Balance = buyOrderBalance;
                buyOrder.Status = OrderStatus.Closed;
                OrderService.Update(buyOrder);

                sellOrder.Balance = sellOrderBalance;
                sellOrder.Status = OrderStatus.Closed;
                OrderService.Update(sellOrder);

                buyerAccrualWallet.Balance = Math.Round(buyerAccrualWallet.Balance, 8) + buyerAccrual;
                UserWalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                UserWalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                UserWalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                UserWalletService.Update(sellerWriteOffWallet);

                var websocketUsers = WebSocketService.orderBookSubscribers
                .Where(x => x.Pairs.Contains($"{sellOrder.BaseAsset.ShortName}/{sellOrder.QuoteAsset.ShortName}"))
                .ToList();

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "orderBookUpdate",
                        Message = "orderBookUpdate"
                    })).Wait();
                }

                if (buyOrder.Price >= price.Price)
                    price.IsHigher = true;
                else
                    price.IsHigher = false;

                price.Price = buyOrder.Price;

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "priceUpdate",
                        Message = "priceUpdate"
                    })).Wait();
                }

                return;
            }

            if (buyOrder.Balance < sellOrder.Balance)
            {
                var buyOrderBalance = 0;
                var sellOrderBalance = Math.Round(sellOrder.Balance, 8) - Math.Round(buyOrder.Balance, 8);

                var buyerAccrual = Math.Round(buyOrder.Balance, 8);
                var buyerWriteOff = Math.Round(buyOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);

                var sellerAccrual = Math.Round(buyOrder.Balance, 8) * Math.Round(buyOrder.Price, 8);
                var sellerWriteOff = Math.Round(buyOrder.Balance, 8);

                buyOrder.Balance = buyOrderBalance;
                buyOrder.Status = OrderStatus.Closed;
                OrderService.Update(buyOrder);

                sellOrder.Balance = sellOrderBalance;
                OrderService.Update(sellOrder);

                buyerAccrualWallet.Balance = Math.Round(buyerAccrualWallet.Balance, 8) + buyerAccrual;
                UserWalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                UserWalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                UserWalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                UserWalletService.Update(sellerWriteOffWallet);

                var websocketUsers = WebSocketService.orderBookSubscribers
                .Where(x => x.Pairs.Contains($"{sellOrder.BaseAsset.ShortName}/{sellOrder.QuoteAsset.ShortName}"))
                .ToList();

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "orderBookUpdate",
                        Message = "orderBookUpdate"
                    })).Wait();
                }

                if (buyOrder.Price >= price.Price)
                    price.IsHigher = true;
                else
                    price.IsHigher = false;

                price.Price = buyOrder.Price;

                foreach (var wsUser in websocketUsers)
                {
                    WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = "priceUpdate",
                        Message = "priceUpdate"
                    })).Wait();
                }

                var openOrders = OrderService.GetAll()
                    .Where(x => x.ID != sellOrder.ID &&
                        x.Status == OrderStatus.Open);

                var toCloseOrder = openOrders
                        .FirstOrDefault(x => x.Type == OrderType.Buy &&
                            x.Price >= sellOrder.Price && x.Balance > 0);

                if (toCloseOrder != null)
                    CompareTwoOrders(sellOrder, toCloseOrder, price);

                return;
            }

            return;
        }

        [HttpGet("pairs/{symbol}/price")]
        public async Task<IActionResult> GetPairPrice(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Closed)
                    .ToList();

            if (orders == null || orders.Count == 0)
                return Ok(new PriceResponse
                {
                    Price = 0,
                    IsHigher = true
                });

            if (orders.Count == 1)
                return Ok(new PriceResponse
                {
                    Price = orders.FirstOrDefault().Price,
                    IsHigher = true
                });

            var lastOrder = orders.AsEnumerable().LastOrDefault();
            var prevLastOrder = orders[orders.Count - 2];

            if (lastOrder == null || prevLastOrder == null)
                return Ok(new PriceResponse
                {
                    Price = 0,
                    IsHigher = true
                });

            return Ok(new PriceResponse
            {
                Price = lastOrder.Price,
                IsHigher = lastOrder.Price > prevLastOrder.Price ? true : lastOrder.Price < prevLastOrder.Price ? false : true
            });
        }

        [HttpGet("pairs/{symbol}/stats")]
        public async Task<IActionResult> GetPairStats(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var now = DateTime.UtcNow;

            var dailyOrders = OrderService.GetAll()
                .FirstOrDefault(x => x.Time >= new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) &&
                    x.Time <= new DateTime(now.Year, now.Month, now.Day, 23, 59, 59));

            var stats = new PairStatsViewModel
            {
                Last = GetCurrentPrice(baseAsset.ShortName, quoteAsset.ShortName),
                Change = 0,
                High = 0,
                Low = 0,
                Volume = 0
            };

            return Ok(new DataResponse<PairStatsViewModel>
            {
                Data = stats
            });
        }

        [HttpGet("pairs/{symbol}/orders/book")]
        public async Task<IActionResult> GetOrderBook(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Open)
                .ToList();

            var asks = new List<double[]>();
            var bids = new List<double[]>();

            foreach (var order in orders)
            {
                if (order.Type == OrderType.Buy)
                {
                    asks.Add(new double[] { order.Price, order.Amount });
                    continue;
                }

                if (order.Type == OrderType.Sell)
                {
                    bids.Add(new double[] { order.Price, order.Amount });
                }
            }

            return Ok(new DepthResponse
            {
                Asks = asks,
                Bids = bids
            });
        }

        [HttpGet("pairs/{symbol}/orders/open")]
        public async Task<IActionResult> GetOpenOrders(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Open);

            var response = new OpenOrdersResponse
            {
                BuyOrders = orders.Where(x => x.Type == OrderType.Buy).Select(x => new OrderViewModel(x)).ToList(),
                SellOrders = orders.Where(x => x.Type == OrderType.Sell).Select(x => new OrderViewModel(x)).ToList()
            };

            return Ok(response);
        }

        [HttpGet("pairs/{symbol}/orders/closed")]
        public async Task<IActionResult> GetClosedOrders(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Closed)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<OrderViewModel>>
            {
                Data = orders
            });
        }

        [HttpGet("pairs/{symbol}/orders/my/open")]
        [Authorize]
        public async Task<IActionResult> GetMyOpenOrders(string symbol)
        {
            var coins = symbol.Split(".");

            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[0]));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(coins[1]));

            if (baseAsset == null || quoteAsset == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Coin error"
                });

            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Open &&
                    x.User == user)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<OrderViewModel>>
            {
                Data = orders
            });
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
            for (var i = 0; i < coins.Count - 1; i++)
            {
                for (var j = 1; j < coins.Count; j++)
                {
                    if (!coins[i].Equals(coins[j]) && j > i)
                        pairs.Add(new PairViewModel(coins[j], coins[i]));
                }

            }

            return Json(new DataResponse<HashSet<PairViewModel>> { Data = pairs });
        }

        private double GetCurrentPrice(string baseAssetShortName, string quoteAssetShortName)
        {
            var baseAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(baseAssetShortName));

            var quoteAsset = CoinService.GetAll()
                .FirstOrDefault(x => x.ShortName.Equals(quoteAssetShortName));

            if (baseAsset == null || quoteAsset == null)
                return 0;

            var order = OrderService.GetAll()
                .AsEnumerable()
                .LastOrDefault(x => x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset &&
                    x.Status == OrderStatus.Closed);

            if (order == null)
                return 0;

            return order.Price;
        }
    }
}