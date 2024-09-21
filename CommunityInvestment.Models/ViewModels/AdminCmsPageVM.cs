using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminCmsPageVM
    {
        public long? CmsPageId { get; set; } = null!;

        [Required]
        [StringLength(255, ErrorMessage = "Only 255 characters are allowed!")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Only 255 characters are allowed!")]
        public string Slug { get; set; } = null!;

        public string? Status { get; set; } = "1";
    }
}
