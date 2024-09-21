using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminBannerVM
    {
        public long? BannerId { get; set; } = null!;

        [Display(Name = "Banner Image")]
        public IFormFile? NewBannerImage { get; set; } = null!;

        public string? ImagePath { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;
        public string? Text { get; set; }
        public int? SortOrder { get; set; }
    }
}
