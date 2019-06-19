using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class CreateOrderRequest
    {
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public double Limit { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
}
