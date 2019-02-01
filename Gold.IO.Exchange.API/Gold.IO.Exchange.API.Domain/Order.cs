using Gold.IO.Exchange.API.Domain.Enum;
using System;

namespace Gold.IO.Exchange.API.Domain
{
    public class Order : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual Coin BaseAsset { get; set; }
        public virtual Coin QuoteAsset { get; set; }
        public virtual double Amount { get; set; }
        public virtual double Balance { get; set; }
        public virtual double Price { get; set; }
        public virtual OrderType Type { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
