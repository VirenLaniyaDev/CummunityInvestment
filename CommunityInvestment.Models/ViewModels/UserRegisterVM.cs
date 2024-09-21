using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserRegisterVM
    {
        public long UserId { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string? LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        public int PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter valid Email address!")]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Password should be 8 character long with, 1 Lower case, 1 Upper case, 1 Digit and 1 Special character")]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirm Password")]
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords does not match!")]
        public string ConfirmPassword { get; set; } = null!;
        public long CityId { get; set; } = 2;
        public long CountryId { get; set; } = 98;
    }
}
