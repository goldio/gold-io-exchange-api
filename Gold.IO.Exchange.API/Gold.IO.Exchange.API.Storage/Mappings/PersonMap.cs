using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Table("persons");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");
            References(e => e.City, "id_city");

            Map(u => u.FullName, "full_name");
            Map(u => u.BirthDate, "birth_date");
            Map(u => u.Email, "email");
            Map(u => u.PhoneNumber, "phone");
            Map(u => u.Address, "address");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
