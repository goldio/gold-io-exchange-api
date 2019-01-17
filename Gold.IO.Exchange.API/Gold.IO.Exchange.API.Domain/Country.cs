using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class Country : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual Locale Locale { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
