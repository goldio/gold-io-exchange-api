using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class UserNotificationsMap : ClassMap<UserNotifications>
    {
        public UserNotificationsMap()
        {
            Table("user_notifications");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.EmailNews, "email_news");
            Map(u => u.EmailLogins, "email_logins");
            Map(u => u.EmailCoinsRemovals, "email_coins_removals");
            Map(u => u.EmailMarketRemovals, "email_market_removals");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
