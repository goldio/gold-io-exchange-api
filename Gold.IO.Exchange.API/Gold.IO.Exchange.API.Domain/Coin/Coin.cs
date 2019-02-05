
namespace Gold.IO.Exchange.API.Domain.Coin
{
    public class Coin : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
        public virtual File.File Icon { get; set; }
        public virtual bool IsCrypto { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
