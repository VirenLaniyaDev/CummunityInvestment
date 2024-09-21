using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminProfileVM
    {
        public long? AdminId { get; set; } = null!;

        [Required]
        public string AdminFirstName { get; set; } = null!;

        [Required]
        public string AdminLastName { get; set; } = null!;
        public string? AdminAvatar { get; set; }
        public IFormFile? NewAdminAvatar { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
