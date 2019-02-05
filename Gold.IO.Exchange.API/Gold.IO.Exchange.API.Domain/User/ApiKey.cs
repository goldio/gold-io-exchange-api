using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class ApiKey : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual string PublicKey { get; set; }
        public virtual string SecretKey { get; set; }
        public virtual bool AccountPermissions { get; set; }
        public virtual bool OrdersPermissions { get; set; }
        public virtual bool FundsPermissions { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
