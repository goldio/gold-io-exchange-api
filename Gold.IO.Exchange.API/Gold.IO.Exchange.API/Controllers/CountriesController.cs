using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Locale;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CountriesController : Controller
    {
        private ILocaleService LocaleService { get; set; }
        private ICountryService CountryService { get; set; }
        private ICityService CityService { get; set; }

        public CountriesController([FromServices]
            ILocaleService localeService,
            ICountryService countryService,
            ICityService cityService)
        {
            LocaleService = localeService;
            CountryService = countryService;
            CityService = cityService;
        }

        [HttpGet("startData")]
        public async Task<IActionResult> StartData()
        {
            var enLocale = new Locale
            {
                Name = "English",
                LangCode = "en"
            };

            var ruLocale = new Locale
            {
                Name = "Russian",
                LangCode = "Ru"
            };

            LocaleService.Create(enLocale);
            LocaleService.Create(ruLocale);

            var usCountry = new Country
            {
                Name = "United States of America",
                Locale = enLocale
            };

            var ruCountry = new Country
            {
                Name = "Russian Federation",
                Locale = ruLocale
            };

            CountryService.Create(usCountry);
            CountryService.Create(ruCountry);

            var nyCity = new City
            {
                Name = "New York",
                Country = usCountry
            };

            var laCity = new City
            {
                Name = "Los Angeles",
                Country = ruCountry
            };

            CityService.Create(nyCity);
            CityService.Create(laCity);

            var mwCity = new City
            {
                Name = "Moscow",
                Country = ruCountry
            };

            var spbCity = new City
            {
                Name = "Saint Petersburg",
                Country = ruCountry
            };

            CityService.Create(mwCity);
            CityService.Create(spbCity);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = CountryService.GetAll()
                .Select(x => new CountryViewModel(x))
                .ToList();

            return Json(new DataResponse<List<CountryViewModel>> { Data = countries });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var country = CountryService.Get(id);
            if (country == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Country not found"
                });

            return Json(new DataResponse<CountryViewModel> { Data = new CountryViewModel(country) });
        }

        [HttpGet("{id}/cities")]
        public async Task<IActionResult> GetCountryCities(long id)
        {
            var country = CountryService.Get(id);
            if (country == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Country not found"
                });

            var cities = CityService.GetAll()
                .Where(x => x.Country == country)
                .Select(x => new CityViewModel(x))
                .ToList();

            return Json(new DataResponse<List<CityViewModel>> { Data = cities });
        }
    }
}