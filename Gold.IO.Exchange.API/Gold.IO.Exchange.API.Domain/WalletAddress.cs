using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class WalletAddress : PersistentObject, IDeletableObject
    {
        public virtual Coin.Coin Coin { get; set; }
        public virtual bool IsUsed { get; set; }
        public virtual string Address { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
