using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public class Locale : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual string LangCode { get; set; }
        public virtual File Icon { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
