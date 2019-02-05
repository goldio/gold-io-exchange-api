

namespace Gold.IO.Exchange.API.Domain.Locale
{
    public class Country : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual File.File Flag { get; set; }
        public virtual Locale Locale { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
