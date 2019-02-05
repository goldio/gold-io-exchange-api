using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class UserNotifications : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual bool EmailNews { get; set; }
        public virtual bool EmailLogins { get; set; }
        public virtual bool EmailCoinsRemovals { get; set; }
        public virtual bool EmailMarketRemovals { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
