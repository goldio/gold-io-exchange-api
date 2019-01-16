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

            Map(u => u.FirstName, "first_name");
            Map(u => u.LastName, "last_name");
            Map(u => u.BirthDate, "birth_date");
            Map(u => u.Email, "email");
            Map(u => u.PhoneNumber, "phone");
            Map(u => u.Country, "country");
            Map(u => u.City, "city");
            Map(u => u.Address, "address");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
