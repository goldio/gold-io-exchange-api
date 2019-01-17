using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CountryViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public LocaleViewModel Locale { get; set; }

        public CountryViewModel() { }

        public CountryViewModel(Country country)
        {
            ID = country.ID;
            Name = country.Name;
            Locale = (LocaleViewModel)country.Locale;
        }

        public static explicit operator CountryViewModel(Country country) => new CountryViewModel(country);
    }
}
