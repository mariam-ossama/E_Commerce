using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOs.Identity
{
    public class AddressDto
    {
        [Required(ErrorMessage = "City Is Required")]
        public string City { get; set; } = default!;
        [Required(ErrorMessage = "Street Is Required")]
        public string Street { get; set; } = default!;
        [Required(ErrorMessage = "Country Is Required")]
        public string Country { get; set; } = default!;
        [Required(ErrorMessage = "FirstName Is Required")]
        public string FirstName { get; set; } = default!;
        [Required(ErrorMessage = "LastName Is Required")]
        public string LastName { get; set; } = default!;
    }
}
