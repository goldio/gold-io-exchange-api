using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class  ApiKey : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual string PublicKey { get; set; }
        public virtual ApiKeyRole Role { get; set; }
        public virtual DateTime Expired { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
