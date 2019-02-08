using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserSessionViewModel
    { 
        public long ID { get; set; }
        public DateTime Time { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public ActivityType Type { get; set; }
        public UserViewModel User { get; set; }

        public UserSessionViewModel() { }

        public UserSessionViewModel(UserSession userSession)
        {
            ID = userSession.ID;
            Time = userSession.Time;
            IPAddress = userSession.IPAddress;
            UserAgent = userSession.UserAgent;
            Type = userSession.Type;
            User = new UserViewModel(userSession.User);
        }
    }
}
