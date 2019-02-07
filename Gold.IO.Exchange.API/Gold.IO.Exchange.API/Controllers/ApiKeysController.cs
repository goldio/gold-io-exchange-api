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
    public class ApiKeysController : Controller
    {
        private IApiKeyService ApiKeyService { get; set; }
        private IUserService UserService { get; set; }

        public ApiKeysController([FromServices]
            IApiKeyService apiKeyService,
            IUserService userService)
        {
            ApiKeyService = apiKeyService;
            UserService = userService;
        }

        public async Task<IActionResult> GetUserApiKeys()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var apiKeys = ApiKeyService.GetAll().Where(x => x.User == user).Select(x => new ApiKeyViewModel(x)).ToList();

            return Json(new DataResponse<List<ApiKeyViewModel>> { Data = apiKeys });
        }
    }
}