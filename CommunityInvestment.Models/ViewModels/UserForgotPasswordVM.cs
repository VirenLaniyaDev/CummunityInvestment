using System.ComponentModel.DataAnnotations;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserForgotPasswordVM
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
