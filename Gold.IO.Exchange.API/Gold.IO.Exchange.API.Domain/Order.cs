using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class Order : PersistentObject, IDeletableObject
    {
        public virtual Coin BaseAsset { get; set; }
        public virtual Coin QuoteAsset { get; set; }
        public virtual double Amount { get; set; }
        public virtual double Price { get; set; }
        public virtual OrderType Type { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
