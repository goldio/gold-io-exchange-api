using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class CoinMap : ClassMap<Coin>
    {
        public CoinMap()
        {
            Table("coins");

            Id(u => u.ID, "id");

            References(e => e.Icon, "id_icon");

            Map(u => u.Name, "name");
            Map(u => u.ShortName, "short_name");
            Map(u => u.IsCrypto, "is_crypto");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
