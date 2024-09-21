using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminMissionVM
    {
        public long? MissionId { get; set; } = null!;

        [Display(Name = "Mission Country")]
        [Required(ErrorMessage = "Please Select Mission Country!")]
        public long CountryId { get; set; }

        [Display(Name = "Mission City")]
        [Required(ErrorMessage = "Please Select Mission City!")]
        public long CityId { get; set; }

        [Display(Name = "Mission Theme")]
        [Required(ErrorMessage = "Please Select Mission Theme!")]
        public long ThemeId { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Only 128 characters are allowed!")]
        public string Title { get; set; } = null!;

        [Display(Name = "Short Description")]
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Registration Deadline")]
        public DateTime? RegistrationDeadline { get; set; }

        [Display(Name = "Goal Value")]
        [Required(ErrorMessage = "This field is Required!")]
        public int GoalValue { get; set; }

        [Display(Name = "Goal Objective Text")]
        public string? GoalObjectiveText { get; set; }

        [Required(ErrorMessage = "Please Select Mission Type!")]
        [Display(Name = "Mission Type")]
        public string MissionType { get; set; } = null!;
        public string? Status { get; set; } = "1";

        [Display(Name = "Organization Name")]
        [Required]
        public string? OrganizationName { get; set; }

        [Display(Name = "Organization Detail")]
        public string? OrganizationDetail { get; set; }
        public string? Availability { get; set; }
        public List<long>? SkillIds { get; set; } = new List<long>();
        public string? MissionImagesUniqueNames { get; set; } = null;
        public string? MissionDocumentsUniqueNames { get; set; } = null;
    }
}
