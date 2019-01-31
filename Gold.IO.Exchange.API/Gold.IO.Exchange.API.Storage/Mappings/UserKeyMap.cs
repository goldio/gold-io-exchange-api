using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class UserKeyMap : ClassMap<UserKey>
    {
        public UserKeyMap()
        {
            Table("user_keys");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.KeyValue, "key_value");
            Map(u => u.Type, "key_type").CustomType<UserKeyType>();
            Map(u => u.ActivationTime, "activation_time");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
