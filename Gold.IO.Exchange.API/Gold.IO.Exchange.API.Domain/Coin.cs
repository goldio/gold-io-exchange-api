
namespace Gold.IO.Exchange.API.Domain
{
    public class Coin : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
        public virtual File Icon { get; set; }
        public virtual bool IsCrypto { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
