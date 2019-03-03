using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Gold.IO.Exchange.API.TradeManager
{
    public class TradeManager
    {
        private IOrderService OrderService { get; set; }
        private IUserWalletService UserWalletService { get; set; }

        private bool IsWorking { get; set; } = false;

        public TradeManager()
        {
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(CheckOrders);
            myTimer.Interval = 10000;
            myTimer.Start();
        }

        public void SetServices(IOrderService orderService, IUserWalletService userWalletService)
        {
            if (OrderService == null)
                OrderService = orderService;

            if (UserWalletService == null)
                UserWalletService = userWalletService;
        }

        public void CheckOrders(object source, ElapsedEventArgs e)
        {
            if (IsWorking)
                return;

            if (OrderService == null || UserWalletService == null)
                return;

            IsWorking = true;

            var orders = OrderService.GetAll().Where(x => x.Status == OrderStatus.Open);
            var buyOrders = orders.Where(x => x.Type == OrderType.Buy);
            var sellOrders = orders.Where(x => x.Type == OrderType.Sell);

            foreach (var buyOrder in buyOrders)
            {
                foreach (var sellOrder in sellOrders)
                {
                    if (sellOrder.User != buyOrder.User)
                    {
                        var buyWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == buyOrder.User && x.Coin == buyOrder.BaseAsset);
                        var buyWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == buyOrder.User && x.Coin == buyOrder.QuoteAsset);

                        var sellWalletBase = UserWalletService.GetAll().FirstOrDefault(x => x.User == sellOrder.User && x.Coin == sellOrder.BaseAsset);
                        var sellWalletQuote = UserWalletService.GetAll().FirstOrDefault(x => x.User == sellOrder.User && x.Coin == sellOrder.QuoteAsset);

                        if (sellOrder.Price <= buyOrder.Price)
                        {
                            if (sellOrder.Balance > buyOrder.Balance)
                            {
                                buyWalletBase.Balance += sellOrder.Balance;
                                buyWalletQuote.Balance -= sellOrder.Balance * buyOrder.Price;

                                sellWalletQuote.Balance += buyOrder.Balance * sellOrder.Price;
                                sellWalletBase.Balance -= buyOrder.Balance;

                                sellOrder.Balance -= buyOrder.Balance;
                                buyOrder.Balance = 0;
                                buyOrder.Status = OrderStatus.Closed;

                                if (sellOrder.Balance == 0)
                                    sellOrder.Status = OrderStatus.Closed;
                                
                                OrderService.Update(buyOrder);
                                OrderService.Update(sellOrder);

                                UserWalletService.Update(buyWalletQuote);
                                UserWalletService.Update(buyWalletBase);

                                UserWalletService.Update(sellWalletBase);
                                UserWalletService.Update(sellWalletQuote);
                            }
                            else if (sellOrder.Balance < buyOrder.Balance)
                            {
                                buyWalletBase.Balance += sellOrder.Balance;
                                buyWalletQuote.Balance -= sellOrder.Balance * buyOrder.Price;

                                sellWalletQuote.Balance += buyOrder.Balance * sellOrder.Price;
                                sellWalletBase.Balance -= buyOrder.Balance;

                                buyOrder.Balance -= sellOrder.Balance;
                                if (buyOrder.Balance == 0)
                                    buyOrder.Status = OrderStatus.Closed;

                                sellOrder.Balance = 0;
                                sellOrder.Status = OrderStatus.Closed;
                                
                                OrderService.Update(buyOrder);
                                OrderService.Update(sellOrder);

                                UserWalletService.Update(buyWalletQuote);
                                UserWalletService.Update(buyWalletBase);

                                UserWalletService.Update(sellWalletBase);
                                UserWalletService.Update(sellWalletQuote);
                            }
                            else if (sellOrder.Balance == buyOrder.Balance)
                            {
                                buyWalletBase.Balance += sellOrder.Balance;
                                buyWalletQuote.Balance -= sellOrder.Balance * buyOrder.Price;

                                sellWalletQuote.Balance += buyOrder.Balance * sellOrder.Price;
                                sellWalletBase.Balance -= buyOrder.Balance;

                                buyOrder.Balance = 0;
                                buyOrder.Status = OrderStatus.Closed;
                                
                                sellOrder.Balance = 0;
                                sellOrder.Status = OrderStatus.Closed;
                                
                                OrderService.Update(buyOrder);
                                OrderService.Update(sellOrder);

                                UserWalletService.Update(buyWalletQuote);
                                UserWalletService.Update(buyWalletBase);

                                UserWalletService.Update(sellWalletBase);
                                UserWalletService.Update(sellWalletQuote);
                            }
                        }
                    }
                }
            }

            IsWorking = false;
        }
    }
}
