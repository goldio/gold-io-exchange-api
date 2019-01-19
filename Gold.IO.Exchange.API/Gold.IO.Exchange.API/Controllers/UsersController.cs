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
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
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
        private IPersonService PersonService { get; set; }

        public UsersController([FromServices]
            IUserService userService,
            IPersonService personService)
        {
            UserService = userService;
            PersonService = personService;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> UserSignUp([FromBody] SignUpRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == request.Email);
            if (user != null)
                return Json(new ResponseModel { Success = false, Message = "Email already used." });

            var person = PersonService.GetAll().FirstOrDefault(x => x.Email == request.Email);
            if (person != null)
                return Json(new ResponseModel { Success = false, Message = "Email already used." });

            user = new User
            {
                Login = request.Email,
                Password = CreateMD5(request.Password),
                RegistrationDate = DateTime.UtcNow,
                Role = UserRole.User
            };

            UserService.Create(user);

            person = new Person
            {
                FullName = request.FullName,
                Email = request.Email,
                User = user
            };

            PersonService.Create(person);

            return Json(new ResponseModel { Success = true, Message = "OK" });
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> UserSignIn([FromBody] SignInRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == request.Login && x.Password == CreateMD5(request.Password));
            if (user == null)
                return Json(new ResponseModel { Success = false, Message = "Wrong email or password" });

            var identity = GetIdentity(request.Login, CreateMD5(request.Password));
            var token = GetSecurityToken(identity, user.Role);

            return Json(new SignInResponse { Success = true, Message = "OK", SecurityToken = token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            return Json(new DataResponse<UserViewModel> { Success = true, Message = "OK", Data = new UserViewModel(user) });
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password);
            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private SecurityTokenViewModel GetSecurityToken(ClaimsIdentity identity, UserRole role)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
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
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
