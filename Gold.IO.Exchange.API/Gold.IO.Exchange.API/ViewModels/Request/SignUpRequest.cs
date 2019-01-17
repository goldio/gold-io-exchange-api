using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
