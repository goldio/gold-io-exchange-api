using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.Coin
{
    public class CoinAccount : PersistentObject, IDeletableObject
    {
        public virtual Coin Coin { get; set; }
        public virtual string AccountKey { get; set; }
        public virtual long Derivations { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
