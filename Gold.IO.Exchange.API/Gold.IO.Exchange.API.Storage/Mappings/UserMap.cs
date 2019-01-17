using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("users");

            Id(u => u.ID, "id");

            Map(u => u.Login, "login");
            Map(u => u.Password, "password");
            Map(u => u.RegistrationDate, "reg_date");
            Map(u => u.Role, "role");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
