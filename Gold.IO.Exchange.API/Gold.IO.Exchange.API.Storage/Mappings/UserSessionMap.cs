using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class UserSessionMap : ClassMap<UserSession>
    {
        public UserSessionMap()
        {
            Table("user_sessions");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.Time, "time");
            Map(u => u.IPAddress, "ip_address");
            Map(u => u.UserAgent, "user_agent");
            Map(u => u.AccessToken, "access_token").CustomSqlType("text");
            Map(u => u.Type, "activity_type").CustomType<ActivityType>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
