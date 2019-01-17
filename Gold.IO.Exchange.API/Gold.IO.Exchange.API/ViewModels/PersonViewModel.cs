using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class PersonViewModel
    {
        public UserViewModel User { get; set; }

        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CityViewModel City { get; set; }
        public string Address { get; set; }

        public PersonViewModel() { }

        public PersonViewModel(Person person)
        {
            ID = person.ID;
            FirstName = person.FirstName;
            LastName = person.LastName;
            BirthDate = person.BirthDate;
            Email = person.Email;
            PhoneNumber = person.PhoneNumber;
            Address = person.Address;

            User = (UserViewModel)person.User;
            City = (CityViewModel)person.City;
        }
    }
}
