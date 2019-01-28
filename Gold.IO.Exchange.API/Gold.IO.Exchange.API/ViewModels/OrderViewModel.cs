using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class OrderViewModel
    {
        public long ID { get; set; }
        public CoinViewModel BaseAsset { get; set; }
        public CoinViewModel QuoteAsset { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
    }
}
