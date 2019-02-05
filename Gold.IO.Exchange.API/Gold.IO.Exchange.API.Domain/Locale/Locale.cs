
namespace Gold.IO.Exchange.API.Domain.Locale
{
    public class Locale : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual string LangCode { get; set; }
        public virtual File.File Icon { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
