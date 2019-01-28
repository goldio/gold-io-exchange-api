using System;

namespace Gold.IO.Exchange.API.Domain
{
    public class Person : PersistentObject, IDeletableObject
    {
        public virtual User User { get; set; }

        public virtual string FullName { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual City City { get; set; }
        public virtual string Address { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
