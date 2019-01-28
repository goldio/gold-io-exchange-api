using System.ComponentModel.DataAnnotations;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class SignInRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
