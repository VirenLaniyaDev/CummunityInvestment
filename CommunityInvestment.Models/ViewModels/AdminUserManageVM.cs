using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminUserManageVM
    {
        public long? UserId { get; set; } = null!;

        [Required(ErrorMessage = "First Name is required!")]
        [Display(Name = "First Name")]
        [StringLength(16)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required!")]
        [Display(Name = "Last Name")]
        [StringLength(16)]
        public string LastName { get; set; } = null!;

        [Required]
        [Display(Name = "Phone Number")]
        public int? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [PasswordPropertyText]
        public string? Password { get; set; } = null!;

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Both password not match!")]
        [PasswordPropertyText]
        public string? ConfirmPassword { get; set; } = null!;

        public string? Avatar { get; set; } = null!;

        public IFormFile? NewUserAvatar { get; set; }

        [Display(Name = "Employee ID")]
        [StringLength(16)]
        public string? EmployeeId { get; set; }

        [StringLength(16)]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Please select Country!")]
        [Display(Name = "Country")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Please select City!")]
        [Display(Name = "City")]
        public long CityId { get; set; }

        [Display(Name = "Profile")]
        public string? ProfileText { get; set; }

        public string Status { get; set; } = "1";
    }

    //public class RequiredForNewUser : RequiredAttribute
    //{
    //    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    //    {
    //        var model = (AdminUserManageVM)validationContext.ObjectInstance;
    //        if (model.UserId != null)
    //            return ValidationResult.Success;
    //        return base.IsValid(value, validationContext);
    //    }
    //}
}
