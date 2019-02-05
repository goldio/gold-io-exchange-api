using Gold.IO.Exchange.API.Domain.Enum;
using System;

namespace Gold.IO.Exchange.API.Domain.User
{
    public class User : PersistentObject, IDeletableObject
    {
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
