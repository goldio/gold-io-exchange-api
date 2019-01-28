
namespace Gold.IO.Exchange.API.Domain
{
    public class File : PersistentObject, IDeletableObject
    {
        public virtual string Name { get; set; }
        public virtual string LocalPath { get; set; }
        public virtual string URL { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
