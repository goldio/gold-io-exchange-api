using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.Coin
{
    public class CoinAddress : PersistentObject, IDeletableObject
    {
        public virtual string PrivateKey { get; set; }
        public virtual string PublicAddress { get; set; }
        public virtual UserWallet Wallet { get; set; }
        public virtual bool IsUsing { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
