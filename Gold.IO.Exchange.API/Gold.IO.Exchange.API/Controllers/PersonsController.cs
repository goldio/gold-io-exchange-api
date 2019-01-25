using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonsController : Controller
    {
        private IUserService UserService { get; set; }
        private IPersonService PersonService { get; set; }
        private ICityService CityService { get; set; }

        public PersonsController([FromServices]
            IUserService userService,
            IPersonService personService,
            ICityService cityService)
        {
            UserService = userService;
            PersonService = personService;
            CityService = cityService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var person = PersonService.GetAll().FirstOrDefault(x => x.User == user);

            return Json(new DataResponse<PersonViewModel> { Success = true, Message = "OK", Data = new PersonViewModel(person) });
        }

        [HttpPut("me")]
        public async Task<IActionResult> PutMe([FromBody] UpdatePersonRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var person = PersonService.GetAll().FirstOrDefault(x => x.User == user);

            if (request.FullName != null)
                person.FullName = request.FullName;

            if (request.BirthDate != null)
                person.BirthDate = request.BirthDate;

            if (request.Email != null)
                person.Email = request.Email;

            if (request.PhoneNumber != null)
                person.PhoneNumber = request.PhoneNumber;

            if (!CityService.Get(request.CityID).Equals(person.City))
                person.City = CityService.Get(request.CityID);

            if (request.Address != null)
                person.Address = request.Address;

            PersonService.Update(person);

            return Json(new DataResponse<PersonViewModel> { Success = true, Message = "OK", Data = new PersonViewModel(person) });
        }
    }
}