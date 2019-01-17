using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserViewModel
    {
        public long ID { get; set; }
        public string Login { get; set; }
        public DateTime RegistrationDate { get; set; }
        public UserRole Role { get; set; }

        public UserViewModel() { }

        public UserViewModel(User user)
        {
            ID = user.ID;
            Login = user.Login;
            RegistrationDate = user.RegistrationDate;
            Role = user.Role;
        }

        public static explicit operator UserViewModel(User user) => new UserViewModel(user);
    }
}
