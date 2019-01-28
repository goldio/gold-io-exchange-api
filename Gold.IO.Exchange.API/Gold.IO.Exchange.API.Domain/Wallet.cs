
namespace Gold.IO.Exchange.API.Domain
{
    public class Wallet : PersistentObject, IDeletableObject
    {
        public virtual bool Deleted { get; set; }

        public virtual User User { get; set; }
        public virtual Coin Coin { get; set; }
        public virtual double Balance { get; set; }
    }
}
