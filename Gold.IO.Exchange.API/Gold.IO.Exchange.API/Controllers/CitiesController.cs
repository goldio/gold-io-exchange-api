using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : Controller
    {
        private ICityService CityService { get; set; }

        public CitiesController([FromServices]
            ICityService cityService)
        {
            CityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = CityService.GetAll()
                .Select(x => new CityViewModel(x))
                .ToList();

            return Json(new DataResponse<List<CityViewModel>> { Data = cities });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var city = CityService.Get(id);
            if (city == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "City not found"
                });

            return Json(new DataResponse<CityViewModel> { Data = new CityViewModel(city) });
        }
    }
}