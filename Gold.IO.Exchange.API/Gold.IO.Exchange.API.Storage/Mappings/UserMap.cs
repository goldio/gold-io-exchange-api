using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;

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
            Map(u => u.Role, "role").CustomType<UserRole>();
            Map(u => u.IsActive, "is_active");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
