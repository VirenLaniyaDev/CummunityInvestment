using MessagePack.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserProfileVM
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Please enter your Name!")]
        [Display(Name = "Name*")]
        [StringLength(16)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter your Surname!")]
        [Display(Name = "Surname*")]
        [StringLength(16)]
        public string LastName { get; set; } = null!;
        //public string Password { get; set; } = null!;
        //public int PhoneNumber { get; set; }
        public string? Avatar { get; set; } = null;
        public IFormFile? NewUserAvatar { get; set; }

        [Display(Name = "Why I Volunteer?")]
        public string? WhyIVolunteer { get; set; }

        [Display(Name = "Employee ID")]
        [StringLength(16)]
        public string? EmployeeId { get; set; }

        [StringLength(16)]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Please select City!")]
        [Display(Name = "City")]
        public long CityId { get; set; }

        [Required(ErrorMessage = "Please select Country!")]
        [Display(Name = "Country")]
        public long CountryId { get; set; }

        [Display(Name="My Profile")]
        public string? ProfileText { get; set; }
        public string? Availability { get; set; }

        [Display(Name = "LinkedIn")]
        public string? LinkedInUrl { get; set; }

        [StringLength(255)]
        public string? Title { get; set; }
        public List<long>? SkillIds { get; set; } = null!;
        //public string Status { get; set; } = null!;
        //public DateTime CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public City City { get; set; } = null!;
        //public Country Country { get; set; } = null!;
        //public DateTime? DeletedAt { get; set; }
        [BindNever]
        public ChangePasswordVM? ChangePassword { get; set; }
    }
}
