using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Locale;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            Table("countries");

            Id(u => u.ID, "id");

            References(e => e.Flag, "id_flag");
            References(e => e.Locale, "id_locale");

            Map(u => u.Name, "name");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
