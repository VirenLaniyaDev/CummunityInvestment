using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class StoryCreateVM
    {
        public long? StoryId { get; set; } = null!;

        [Required(ErrorMessage ="Please select mission!")]
        [Display(Name = "Select Mission")]
        public long MissionId { get; set; }

        public long UserId { get; set; }

        [Required]
        [Display(Name = "My Story Title")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters allowed!")]
        public string? Title { get; set; }

        [Display(Name ="My Story")]
        [StringLength(40000, ErrorMessage = "Maximum 40000 Characters allowed!")]
        public string? Description { get; set; } = null;

        [Display(Name ="Enter Story URL")]
        [UrlsLimit(20)]
        public string? StoryVideoURLs { get; set; } = null;

        [Display(Name = "Upload your Photos")]
        public string? UploadedImages { get; set; } = null;
        public string? StoryImagesUniqueNames { get; set; } = null;

        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime? PublishedAt { get; set; }
        public string? StoryAction { get; set; } = "draft";
        public string? StoryStatus { get; set; } = null;
    }

    public class UrlsLimitAttribute : ValidationAttribute
    {
        private readonly int _limit;
        public UrlsLimitAttribute(int limit)
        {
            _limit = limit;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var urls = value.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (urls.Length > _limit)
                {
                    return new ValidationResult($"You can add up to {_limit} URLs.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
