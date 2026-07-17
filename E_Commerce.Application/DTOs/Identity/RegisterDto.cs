using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOs.Identity
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email Is Required"), EmailAddress]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "UserName Is Required")]
        public string UserName { get; set; } = default!;
        [Required(ErrorMessage = "DisplayName Is Required")]
        public string DisplayName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
