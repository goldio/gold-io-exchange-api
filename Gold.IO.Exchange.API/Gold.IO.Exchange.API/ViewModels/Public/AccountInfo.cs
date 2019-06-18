using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Public
{
    public class AccountInfo
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CityViewModel City { get; set; }
        public string Address { get; set; }
        public List<UserWalletViewModel> Balance { get; set; }
    }
}
