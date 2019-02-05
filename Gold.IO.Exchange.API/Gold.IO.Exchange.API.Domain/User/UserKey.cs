using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class UserKey : PersistentObject, IDeletableObject
    {
        public virtual string KeyValue { get; set; }
        public virtual UserKeyType Type { get; set; }
        public virtual DateTime ActivationTime { get; set; }
        public virtual User User { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
