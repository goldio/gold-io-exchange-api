using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class UserSession : PersistentObject, IDeletableObject
    {
        public virtual DateTime Time { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual ActivityType Type { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual User User { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
