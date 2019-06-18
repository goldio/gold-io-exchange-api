using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class ApiKeyMap : ClassMap<ApiKey>
    {
        public ApiKeyMap()
        {
            Table("api_keys");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.PublicKey, "public_key");
            Map(u => u.Role, "role").CustomType<ApiKeyRole>();
            Map(u => u.Expired, "expired");

            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
