using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.Order;
using System;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class OrderViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public CoinViewModel BaseAsset { get; set; }
        public CoinViewModel QuoteAsset { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public double Price { get; set; }
        public double Limit { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Time { get; set; }

        public OrderViewModel() { }

        public OrderViewModel(Order order)
        {
            ID = order.ID;
            User = new UserViewModel(order.User);
            BaseAsset = new CoinViewModel(order.BaseAsset);
            QuoteAsset = new CoinViewModel(order.QuoteAsset);
            Amount = order.Amount;
            Balance = order.Balance;
            Limit = order.Limit;
            Price = order.Price;
            Type = order.Type;
            Side = order.Side;
            Status = order.Status;
            Time = order.Time;
        }
    }
}
