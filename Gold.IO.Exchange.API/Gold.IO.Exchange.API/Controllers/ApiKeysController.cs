using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Utils.Helpers;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
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
            var identity = GetIdentity(request.Role);
            var token = GetSecurityToken(identity);
            var expired = DateTime.UtcNow.Add(TimeSpan.FromDays(30));
            var apiKey = new ApiKey
            {
                User = user,
                PublicKey = token,
                Expired = expired,
                Role = request.Role
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

            var identity = GetIdentity(request.Role);
            var token = GetSecurityToken(identity);

            apiKey.Role = request.Role;
            apiKey.PublicKey = token;

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

        private ClaimsIdentity GetIdentity(ApiKeyRole role)
        {
            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private string GetSecurityToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromDays(30));

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(
                        AuthOptions.GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}