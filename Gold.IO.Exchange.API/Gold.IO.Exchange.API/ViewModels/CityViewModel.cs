using Gold.IO.Exchange.API.Domain;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CityViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public CountryViewModel Country { get; set; }

        public CityViewModel() { }

        public CityViewModel(City city)
        {
            if (city != null)
            {
                ID = city.ID;
                Name = city.Name;
                Country = new CountryViewModel(city.Country);
            }
        }
    }
}
