using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.Coin
{
    public class PrivateKey : PersistentObject, IDeletableObject
    {
        public virtual bool Deleted { get; set; }
    }
}
