﻿using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Locale;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class CityMap : ClassMap<City>
    {
        public CityMap()
        {
            Table("cities");

            Id(u => u.ID, "id");

            References(e => e.Country, "id_country");

            Map(u => u.Name, "name");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
