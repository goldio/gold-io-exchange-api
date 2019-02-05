using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserNotificationsViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public bool EmailNews { get; set; }
        public bool EmailLogins { get; set; }
        public bool EmailCoinsRemovals { get; set; }
        public bool EmailMarketRemovals { get; set; }

        public UserNotificationsViewModel() { }

        public UserNotificationsViewModel(UserNotifications userNotifications)
        {
            ID = userNotifications.ID;
            User = new UserViewModel(userNotifications.User);
            EmailNews = userNotifications.EmailNews;
            EmailLogins = userNotifications.EmailLogins;
            EmailCoinsRemovals = userNotifications.EmailCoinsRemovals;
            EmailMarketRemovals = userNotifications.EmailMarketRemovals;
        }
    }
}
