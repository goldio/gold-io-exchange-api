using System.ComponentModel.DataAnnotations;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class SignUpRequest
    {
        public string FullName { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
