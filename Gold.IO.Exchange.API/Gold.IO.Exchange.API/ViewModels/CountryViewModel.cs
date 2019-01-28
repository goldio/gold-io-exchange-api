using Gold.IO.Exchange.API.Domain;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CountryViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public FileViewModel Flag { get; set; }
        public LocaleViewModel Locale { get; set; }

        public CountryViewModel() { }

        public CountryViewModel(Country country)
        {
            if (country != null)
            {
                ID = country.ID;
                Name = country.Name;

                Flag = new FileViewModel(country.Flag);
                Locale = new LocaleViewModel(country.Locale);
            }
        }
    }
}
