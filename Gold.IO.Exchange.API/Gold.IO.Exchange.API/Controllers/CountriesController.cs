using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private ICountryService CountryService { get; set; }
        private ICityService CityService { get; set; }

        public CountriesController([FromServices]
            ICountryService countryService,
            ICityService cityService)
        {
            CountryService = countryService;
            CityService = cityService;
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