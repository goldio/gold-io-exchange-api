using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
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

        public PersonsController([FromServices]
            IUserService userService,
            IPersonService personService)
        {
            UserService = userService;
            PersonService = personService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var person = PersonService.GetAll().FirstOrDefault(x => x.User == user);

            return Json(new DataResponse<PersonViewModel> { Success = true, Message = "OK", Data = new PersonViewModel(person) });
        }
    }
}