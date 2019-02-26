using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using System.Linq;
using System.Timers;

namespace Gold.IO.Exchange.API.TradeManager
{
    public class TradeManager
    {
        private IOrderService OrderService { get; set; }

        public TradeManager()
        {
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(CheckOrders);
            myTimer.Interval = 10000;
            myTimer.Start();
        }

        public void SetServices(IOrderService orderService)
        {
            if (OrderService == null)
                OrderService = orderService;
        }

        public void CheckOrders(object source, ElapsedEventArgs e)
        {
            var orders = OrderService.GetAll().Where(x => x.Status == OrderStatus.Open).ToList();
            var buyOrders = orders.Where(x => x.Type == OrderType.Buy).ToList();
            var sellOrders = orders.Where(x => x.Type == OrderType.Sell).ToList();

            foreach (var buyOrder in buyOrders)
            {
                foreach (var sellOrder in sellOrders)
                {
                    if (sellOrder.User != buyOrder.User)
                    {
                        if (sellOrder.Price <= buyOrder.Price)
                        {
                            if (sellOrder.Balance > buyOrder.Balance)
                            {
                                buyOrder.Balance = 0;
                                buyOrder.Status = OrderStatus.Closed;

                                sellOrder.Balance -= buyOrder.Balance;
                                if (sellOrder.Balance == 0)
                                    sellOrder.Status = OrderStatus.Closed;

                                buyOrders.Remove(buyOrder);
                                OrderService.Update(buyOrder);

                                sellOrders.Remove(sellOrder);
                                OrderService.Update(sellOrder);
                            }
                            else if (sellOrder.Balance < buyOrder.Balance)
                            {
                                buyOrder.Balance -= sellOrder.Balance;
                                if (buyOrder.Balance == 0)
                                    buyOrder.Status = OrderStatus.Closed;

                                sellOrder.Balance = 0;
                                sellOrder.Status = OrderStatus.Closed;

                                buyOrders.Remove(buyOrder);
                                OrderService.Update(buyOrder);

                                sellOrders.Remove(sellOrder);
                                OrderService.Update(sellOrder);
                            }
                            else if (sellOrder.Balance == buyOrder.Balance)
                            {
                                buyOrder.Balance = 0;
                                buyOrder.Status = OrderStatus.Closed;

                                sellOrder.Balance = 0;
                                sellOrder.Status = OrderStatus.Closed;

                                buyOrders.Remove(buyOrder);
                                OrderService.Update(buyOrder);

                                sellOrders.Remove(sellOrder);
                                OrderService.Update(sellOrder);
                            }
                        }
                    }
                }
            }
        }
    }
}
