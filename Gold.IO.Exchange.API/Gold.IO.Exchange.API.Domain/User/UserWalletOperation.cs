using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class UserWalletOperation : PersistentObject, IDeletableObject
    {
        public virtual UserWallet Wallet { get; set; }
        public virtual string Address { get; set; }
        public virtual double Amount { get; set; }
        public virtual UserWalletOperationType Type { get; set; }
        public virtual long Confirmations { get; set; }
        public virtual UserWalletOperationStatus Status { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
