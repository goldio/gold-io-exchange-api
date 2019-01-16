using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class Person : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Country { get; set; }
        public virtual string City { get; set; }
        public virtual string Address { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
