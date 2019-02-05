using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Locale;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class LocaleMap : ClassMap<Locale>
    {
        public LocaleMap()
        {
            Table("locales");

            Id(u => u.ID, "id");

            References(e => e.Icon, "id_icon");

            Map(u => u.Name, "name");
            Map(u => u.LangCode, "lang_code");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
