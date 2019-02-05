using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserViewModel
    {
        public long ID { get; set; }
        public string Login { get; set; }
        public DateTime RegistrationDate { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        public UserViewModel() { }

        public UserViewModel(User user)
        {
            if (user != null)
            {
                ID = user.ID;
                Login = user.Login;
                RegistrationDate = user.RegistrationDate;
                Role = user.Role;
                IsActive = user.IsActive;
            }
        }
    }
}
