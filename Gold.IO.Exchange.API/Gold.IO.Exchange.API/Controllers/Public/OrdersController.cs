﻿using System;
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

namespace Gold.IO.Exchange.API.Controllers.Public
{
    [Route("api/public/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : Controller
    {
        private IUserService UserService { get; set; }
        private IOrderService OrderService { get; set; }
        private ICoinService CoinService { get; set; }
        private IUserWalletService WalletService { get; set; }
        private NotificationsMessageHandler WebSocketService { get; set; }

        public OrdersController([FromServices]
            IUserService userService,
            IOrderService orderService,
            ICoinService coinService,
            IUserWalletService walletService,
            NotificationsMessageHandler webSocketService)
        {
            UserService = userService;
            OrderService = orderService;
            CoinService = coinService;
            WalletService = walletService;
            WebSocketService = webSocketService;
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

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var orders = OrderService.GetAll()
                .Where(x => x.User == user)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(long id)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var order = OrderService.Get(id);
            if (order == null)
                return NotFound(new ResponseModel { Success = false, Message = "Order not found" });

            if (order.User != user)
                return Forbid();

            return Ok(new DataResponse<OrderViewModel> { Data = new OrderViewModel(order) });
        }

        [HttpGet("pairs/{symbol}/orders/my/closed")]
        public async Task<IActionResult> GetMyClosedOrders(string symbol)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

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
                    x.User == user)
                .OrderByDescending(x => x.Time)
                .Select(x => new OrderViewModel(x))
                .ToList();

            orders = orders.Where(x =>
                x.Status == OrderStatus.Closed ||
                x.Status == OrderStatus.Canceled)
            .ToList();

            return Ok(new DataResponse<List<OrderViewModel>>
            {
                Data = orders
            });
        }

        [HttpGet("pairs/{symbol}/orders/my/open")]
        public async Task<IActionResult> GetMyOpenOrders(string symbol)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

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
                .OrderByDescending(x => x.Time)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<OrderViewModel>>
            {
                Data = orders
            });
        }

        [HttpGet("pairs/{symbol}/orders/closed")]
        public async Task<IActionResult> GetClosedOrders(string symbol)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

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
                .OrderByDescending(x => x.Time)
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<OrderViewModel>>
            {
                Data = orders
            });
        }

        [HttpGet("pairs/{symbol}/orders/open")]
        public async Task<IActionResult> GetOpenOrders(string symbol)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

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
                BuyOrders = orders.Where(x => x.Side == OrderSide.Buy)
                    .OrderByDescending(x => x.Price)
                    .Select(x => new OrderViewModel(x))
                    .ToList(),
                SellOrders = orders.Where(x => x.Side == OrderSide.Sell)
                    .OrderByDescending(x => x.Price)
                    .Select(x => new OrderViewModel(x))
                    .ToList()
            };

            return Ok(response);
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

            var stats = new PairStatsViewModel
            {
                Last = GetCurrentPrice(baseAsset.ShortName, quoteAsset.ShortName)
            };

            var now = DateTime.UtcNow;

            var dailyOrders = OrderService.GetAll()
                .Where(x => x.Time >= new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) &&
                    x.Time <= new DateTime(now.Year, now.Month, now.Day, 23, 59, 59) &&
                    x.Status == OrderStatus.Closed &&
                    x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset)
                .ToList();

            var lastOrder = OrderService.GetAll()
                .AsEnumerable()
                .LastOrDefault(x => x.Status == OrderStatus.Closed &&
                    x.BaseAsset == baseAsset &&
                    x.QuoteAsset == quoteAsset);

            if (lastOrder == null)
            {
                stats.Change = 0;
            }
            else
            {
                var prevDayOrder = OrderService.GetAll()
                    .FirstOrDefault(x => x.Time <= lastOrder.Time.Subtract(TimeSpan.FromDays(1)) &&
                        x.Status == OrderStatus.Closed &&
                        x.BaseAsset == baseAsset &&
                        x.QuoteAsset == quoteAsset);

                if (prevDayOrder == null)
                    stats.Change = 0;
                else
                    stats.Change = Math.Round(lastOrder.Price, 8) - Math.Round(prevDayOrder.Price, 8);
            }

            if (dailyOrders != null && dailyOrders.Count != 0)
            {
                stats.High = dailyOrders.Max(x => x.Price);
                stats.Low = dailyOrders.Min(x => x.Price);
                stats.Volume = dailyOrders.Sum(x => x.Amount);
            }
            else
            {
                stats.High = 0;
                stats.Low = 0;
                stats.Volume = 0;
            }

            return Ok(new DataResponse<PairStatsViewModel>
            {
                Data = stats
            });
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

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

            if (request.Side == OrderSide.Buy)
            {
                var wallet = WalletService.GetAll()
                    .FirstOrDefault(x => x.User == user &&
                        x.Coin == quoteAssetCoin);

                if (wallet.Balance < (request.Amount * request.Price))
                    return BadRequest(new ResponseModel
                    {
                        Success = false,
                        Message = "Insuficcient funds"
                    });
            }

            if (request.Side == OrderSide.Sell)
            {
                var wallet = WalletService.GetAll()
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
                Limit = request.Limit,
                Status = OrderStatus.Open,
                Side = request.Side,
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
                    x.BaseAsset == baseAssetCoin &&
                    x.QuoteAsset == quoteAssetCoin &&
                    x.Status == OrderStatus.Open);

            Order toCloseOrder;

            if (order.Side == OrderSide.Buy)
                toCloseOrder = openOrders
                    .FirstOrDefault(x => x.Side == OrderSide.Sell &&
                        x.Price <= order.Price && x.Balance > 0);
            else if (order.Side == OrderSide.Sell)
                toCloseOrder = openOrders
                    .FirstOrDefault(x => x.Side == OrderSide.Buy &&
                        x.Price >= order.Price && x.Balance > 0);
            else
                toCloseOrder = null;

            if (toCloseOrder != null)
                CompareTwoOrders(order, toCloseOrder, price);

            return Ok(new ResponseModel());
        }

        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(long id)
        {
            if (!User.IsInRole("OnlyOrders") && !User.IsInRole("AllActions"))
                return Forbid();

            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            var order = OrderService.Get(id);
            if (order == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Order not found"
                });

            if (order.User != user)
                return Forbid();

            order.Status = OrderStatus.Canceled;
            OrderService.Update(order);

            var websocketUsers = WebSocketService.orderBookSubscribers
                .Where(x => x.Pairs.Contains($"{order.BaseAsset.ShortName}/{order.QuoteAsset.ShortName}"))
                .ToList();

            foreach (var wsUser in websocketUsers)
            {
                await WebSocketService.SendMessageAsync(wsUser.ID, JsonConvert.SerializeObject(new WebSocketMessage
                {
                    Type = "orderBookUpdate",
                    Message = "orderBookUpdate"
                }));
            }

            return Ok(new ResponseModel());
        }

        private void CompareTwoOrders(Order order1, Order order2, PriceViewModel price)
        {
            var orders = new List<Order>
            {
                order1,
                order2
            };

            var buyOrder = orders.FirstOrDefault(x => x.Side == OrderSide.Buy);
            var sellOrder = orders.FirstOrDefault(x => x.Side == OrderSide.Sell);

            if (buyOrder == null || sellOrder == null)
                return;

            var buyerAccrualWallet = WalletService.GetAll()
                    .FirstOrDefault(x => x.User == buyOrder.User &&
                        x.Coin == buyOrder.BaseAsset);

            var buyerWriteOffWallet = WalletService.GetAll()
                    .FirstOrDefault(x => x.User == buyOrder.User &&
                        x.Coin == buyOrder.QuoteAsset);

            var sellerAccrualWallet = WalletService.GetAll()
                    .FirstOrDefault(x => x.User == sellOrder.User &&
                        x.Coin == sellOrder.QuoteAsset);

            var sellerWriteOffWallet = WalletService.GetAll()
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
                WalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                WalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                WalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                WalletService.Update(sellerWriteOffWallet);

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
                        x.Status == OrderStatus.Open &&
                        x.BaseAsset == buyOrder.BaseAsset &&
                        x.QuoteAsset == buyOrder.QuoteAsset);

                var toCloseOrder = openOrders
                        .FirstOrDefault(x => x.Side == OrderSide.Sell &&
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
                WalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                WalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                WalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                WalletService.Update(sellerWriteOffWallet);

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
                WalletService.Update(buyerAccrualWallet);

                buyerWriteOffWallet.Balance = Math.Round(buyerWriteOffWallet.Balance, 8) - buyerWriteOff;
                WalletService.Update(buyerWriteOffWallet);

                sellerAccrualWallet.Balance = Math.Round(sellerAccrualWallet.Balance, 8) + sellerAccrual;
                WalletService.Update(sellerAccrualWallet);

                sellerWriteOffWallet.Balance = Math.Round(sellerWriteOffWallet.Balance, 8) - sellerWriteOff;
                WalletService.Update(sellerWriteOffWallet);

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
                        x.Status == OrderStatus.Open &&
                        x.BaseAsset == sellOrder.BaseAsset &&
                        x.QuoteAsset == sellOrder.QuoteAsset);

                var toCloseOrder = openOrders
                        .FirstOrDefault(x => x.Side == OrderSide.Buy &&
                            x.Price >= sellOrder.Price && x.Balance > 0);

                if (toCloseOrder != null)
                    CompareTwoOrders(sellOrder, toCloseOrder, price);

                return;
            }

            return;
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