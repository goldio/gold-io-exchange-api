using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private IUserService UserService { get; set; }
        private IUserKeyService UserKeyService { get; set; }
        private IEmailService EmailService { get; set; }
        private IPersonService PersonService { get; set; }
        private ICoinService CoinService { get; set; }
        private IUserWalletService WalletService { get; set; }
        private IUserNotificationsService UserNotificationsService { get; set; }
        private IUserSessionService UserSessionService { get; set; }

        public UsersController([FromServices]
            IUserService userService,
            IUserKeyService userKeyService,
            IEmailService emailService,
            IPersonService personService,
            ICoinService coinService,
            IUserWalletService walletService,
            IUserNotificationsService userNotificationsService,
            IUserSessionService userSessionService)
        {
            UserService = userService;
            UserKeyService = userKeyService;
            EmailService = emailService;
            PersonService = personService;
            CoinService = coinService;
            WalletService = walletService;
            UserNotificationsService = userNotificationsService;
            UserSessionService = userSessionService;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> UserSignUp([FromBody] SignUpRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == request.Email);

            if (user != null)
                return Json(new ResponseModel {
                    Success = false,
                    Message = "Email already used."
                });

            var person = PersonService.GetAll().FirstOrDefault(x => x.Email == request.Email);

            if (person != null)
                return Json(new ResponseModel {
                    Success = false,
                    Message = "Email already used."
                });

            user = new User
            {
                Login = request.Email,
                Password = CreateMD5(request.Password),
                RegistrationDate = DateTime.UtcNow,
                Role = UserRole.User,
                IsActive = false
            };

            UserService.Create(user);

            var activationKey = new UserKey
            {
                KeyValue = CreateMD5($"{user.Login}_{DateTime.UtcNow}"),
                Type = UserKeyType.Activation,
                User = user
            };

            UserKeyService.Create(activationKey);

            person = new Person
            {
                FullName = request.FullName,
                Email = request.Email,
                User = user
            };

            PersonService.Create(person);

            var userNotifications = new UserNotifications
            {
                User = user,
                EmailNews = true,
                EmailLogins = false,
                EmailCoinsRemovals = false,
                EmailMarketRemovals = false
            };

            UserNotificationsService.Create(userNotifications);

            var coins = CoinService.GetAll().ToList();
            foreach (var c in coins)
            {
                WalletService.Create(new UserWallet
                {
                    Coin = c,
                    Balance = 0,
                    User = user
                });
            }

            await EmailService.SendActivationMessage(user.Login, activationKey.KeyValue);

            return Json(new ResponseModel());
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> UserSignIn([FromBody] SignInRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => 
                x.Login == request.Login && 
                x.Password == CreateMD5(request.Password));

            if (user == null)
                return Json(new ResponseModel {
                    Success = false,
                    Message = "Wrong email or password"
                });

            if (!user.IsActive)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Your account is inactive"
                });

            var identity = GetIdentity(request.Login, CreateMD5(request.Password));
            var token = GetSecurityToken(identity, user.Role, request.Remember);

            var session = new UserSession
            {
                User = user,
                Time = DateTime.UtcNow,
                IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                AccessToken = token.Token,
                Type = ActivityType.LogIn
            };

            UserSessionService.Create(session);

            return Json(new SignInResponse { SecurityToken = token });
        }

        [HttpPost("activation")]
        public async Task<IActionResult> UserActivation([FromBody] ActivationRequest request)
        {
            var activationKey = UserKeyService.GetAll().FirstOrDefault(x => x.KeyValue == request.Key);
            if (activationKey == null || activationKey.Type != UserKeyType.Activation || activationKey.ActivationTime != DateTime.MinValue)
                return Json(new ResponseModel { Success = false, Message = "Key is invalid" });

            activationKey.ActivationTime = DateTime.UtcNow;
            UserKeyService.Update(activationKey);

            activationKey.User.IsActive = true;
            UserService.Update(activationKey.User);

            return Json(new ResponseModel { Message = "Activation was successful!" });
        }

        [HttpPost("recovery/password")]
        public async Task<IActionResult> UserRecoveryPassword([FromBody] RecoveryChangePasswordRequest request)
        {
            var key = UserKeyService.GetAll().FirstOrDefault(x => x.KeyValue.Equals(request.Key));
            if (key == null)
                return Json(new ResponseModel { Success = false, Message = "Invalid key" });

            if (!key.Type.Equals(UserKeyType.Recovery))
                return Json(new ResponseModel { Success = false, Message = "Invalid key" });

            if (!key.ActivationTime.Equals(DateTime.MinValue))
                return Json(new ResponseModel { Success = false, Message = "Invalid key" });

            if (!request.Password.Equals(request.RepeatPassword))
                return Json(new ResponseModel { Success = false, Message = "Passwords don't match" });

            key.ActivationTime = DateTime.UtcNow;
            UserKeyService.Update(key);

            key.User.Password = CreateMD5(request.Password);
            UserService.Update(key.User);

            return Json(new ResponseModel());
        }

        [HttpPost("recovery")]
        public async Task<IActionResult> UserRecovery([FromBody] RecoveryRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == request.Login);
            if (user == null)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "User not found"
                });

            if (!user.IsActive)
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = "Activate your account first"
                });

            var recoveryKey = new UserKey
            {
                KeyValue = CreateMD5($"{user.Login}_recovery_{DateTime.UtcNow}"),
                Type = UserKeyType.Recovery,
                User = user
            };

            UserKeyService.Create(recoveryKey);

            await EmailService.SendRecoveryMessage(user.Login, recoveryKey.KeyValue);

            return Json(new ResponseModel());
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            return Json(new DataResponse<UserViewModel> { Data = new UserViewModel(user) });
        }

        [HttpGet("me/activity")]
        [Authorize]
        public async Task<IActionResult> GetActivity()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var activity = UserSessionService.GetAll().Where(x => x.User == user).Select(x => new UserSessionViewModel(x)).ToList();

            return Json(new DataResponse<List<UserSessionViewModel>> { Data = activity });
        }

        [HttpPost("me/security/update")]
        [Authorize]
        public async Task<IActionResult> UpdateSecurity([FromBody] ChangePasswordRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            if (!CreateMD5(request.OldPassword).Equals(user.Password))
                return Json(new ResponseModel { Success = false, Message = "Old password wrong" });

            if (!request.NewPassword.Equals(request.RepeatPassword))
                return Json(new ResponseModel { Success = false, Message = "Passwords not equals" });

            user.Password = CreateMD5(request.NewPassword);
            UserService.Update(user);

            return Json(new ResponseModel());
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            var user = UserService.GetAll().FirstOrDefault(x => 
                x.Login == login && 
                x.Password == password);

            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private SecurityTokenViewModel GetSecurityToken(ClaimsIdentity identity, UserRole role, bool remember)
        {
            var now = DateTime.UtcNow;
            var expires = now;

            if (remember)
                expires = now.Add(TimeSpan.MaxValue);
            else
                expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

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

            var token = new SecurityTokenViewModel()
            {
                Token = encodedJwt,
                Role = role,
                ExpireDate = expires
            };

            return token;
        }

        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));

                return sb.ToString();
            }
        }
    }
}
