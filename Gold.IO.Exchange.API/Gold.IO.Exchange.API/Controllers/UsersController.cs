using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

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
                RegistrationDate = DateTime.UtcNow
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
