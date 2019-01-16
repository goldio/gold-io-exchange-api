using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class User : PersistentObject, IDeletableObject
    {
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
