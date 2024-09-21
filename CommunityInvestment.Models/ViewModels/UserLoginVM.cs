using System.ComponentModel.DataAnnotations;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserLoginVM
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
