using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic
{
    public class AuthOptions
    {
        public const string ISSUER = "GoldIOAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:14534/"; // потребитель токена
        const string KEY = "jd645JHkdH348thdsf3ujd4wk";   // ключ для шифрации
        //public const int LIFETIME = 10; // DEV - 10 минут
        public const int LIFETIME = 120; // время жизни токена - 120 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
