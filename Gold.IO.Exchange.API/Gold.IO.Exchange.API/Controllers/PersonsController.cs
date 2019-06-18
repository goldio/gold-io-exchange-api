using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PersonsController : Controller
    {
        private IUserService UserService { get; set; }
        private IPersonService PersonService { get; set; }
        private IUserNotificationsService UserNotificationsService { get; set; }
        private ICityService CityService { get; set; }

        public PersonsController([FromServices]
            IUserService userService,
            IPersonService personService,
            IUserNotificationsService userNotificationsService,
            ICityService cityService)
        {
            UserService = userService;
            PersonService = personService;
            UserNotificationsService = userNotificationsService;
            CityService = cityService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var person = PersonService.GetAll().FirstOrDefault(x => x.User == user);

            return Json(new DataResponse<PersonViewModel> { Data = new PersonViewModel(person) });
        }

        [HttpGet("me/notifications")]
        public async Task<IActionResult> GetMeNotifications()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var notifications = UserNotificationsService.GetAll().FirstOrDefault(x => x.User == user);

            return Json(new DataResponse<UserNotificationsViewModel> { Data = new UserNotificationsViewModel(notifications) });
        }

        [HttpPost("me/notifications/update")]
        public async Task<IActionResult> UpdateMeNotifications([FromBody] UpdateUserNotificationsRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var notifications = UserNotificationsService.GetAll().FirstOrDefault(x => x.User == user);

            notifications.EmailNews = request.EmailNews;
            notifications.EmailLogins = request.EmailLogins;
            notifications.EmailCoinsRemovals = request.EmailCoinsRemovals;
            notifications.EmailMarketRemovals = request.EmailMarketRemovals;

            UserNotificationsService.Update(notifications);

            return Json(new DataResponse<UserNotificationsViewModel> { Data = new UserNotificationsViewModel(notifications) });
        }

        [HttpPost("me/update")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdatePersonRequest request)
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

            return Json(new DataResponse<PersonViewModel> { Data = new PersonViewModel(person) });
        }
    }
}