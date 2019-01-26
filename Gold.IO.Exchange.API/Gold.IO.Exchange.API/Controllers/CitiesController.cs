using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}