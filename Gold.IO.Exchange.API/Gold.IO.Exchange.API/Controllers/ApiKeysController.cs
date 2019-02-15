using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Utils.Helpers;
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

        [HttpGet]
        public async Task<IActionResult> GetUserApiKeys()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var apiKeys = ApiKeyService.GetAll().Where(x => x.User == user).Select(x => new ApiKeyViewModel(x)).ToList();

            return Json(new DataResponse<List<ApiKeyViewModel>> { Data = apiKeys });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserApiKey([FromBody] CreateUpdateApiKeyRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var apiKey = new ApiKey
            {
                User = user,
                PublicKey = CryptHelper.CreateMD5($"{user.Login}_public_{DateTime.UtcNow}"),
                SecretKey = CryptHelper.CreateMD5($"{user.Login}_secret_{DateTime.UtcNow}"),
                AccountPermissions = request.AccountPermissions,
                OrdersPermissions = request.OrdersPermissions,
                FundsPermissions = request.FundsPermissions                
            };

            ApiKeyService.Create(apiKey);

            var apiKeys = ApiKeyService.GetAll().Where(x => x.User == user).Select(x => new ApiKeyViewModel(x)).ToList();
            return Json(new DataResponse<List<ApiKeyViewModel>> { Data = apiKeys });
        }

        [HttpPost("{id}/update")]
        public async Task<IActionResult> UpdateUserApiKey([FromBody] CreateUpdateApiKeyRequest request, long id)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var apiKey = ApiKeyService.Get(id);

            if (apiKey == null)
                return Json(new ResponseModel { Success = false, Message = "API key not found" });

            apiKey.AccountPermissions = request.AccountPermissions;
            apiKey.OrdersPermissions = request.OrdersPermissions;
            apiKey.FundsPermissions = request.FundsPermissions;

            ApiKeyService.Update(apiKey);

            var apiKeys = ApiKeyService.GetAll().Where(x => x.User == user).Select(x => new ApiKeyViewModel(x)).ToList();
            return Json(new DataResponse<List<ApiKeyViewModel>> { Data = apiKeys });
        }

        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteUserApiKey(long id)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            ApiKeyService.Delete(id);

            var apiKeys = ApiKeyService.GetAll().Where(x => x.User == user).Select(x => new ApiKeyViewModel(x)).ToList();
            return Json(new DataResponse<List<ApiKeyViewModel>> { Data = apiKeys });
        }
    }
}