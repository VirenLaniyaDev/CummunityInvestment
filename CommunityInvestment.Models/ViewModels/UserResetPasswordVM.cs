using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserResetPasswordVM
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirm Password")]
        [Required]
        [Compare(nameof(Password), ErrorMessage="Both password not match!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
