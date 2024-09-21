using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class TimesheetVM
    {
        public long? TimesheetId { get; set; } = null;

        [Display(Name = "Mission")]
        [Required(ErrorMessage = "Please selected volunteered Mission!")]
        public long? MissionId { get; set; }
        public long? UserId { get; set; }

        [BindNever]
        public string? MissionTitle { get; set; }

        public string? MissionType { get; set; } = null!;

        [Required]
        [Display(Name = "Hours")]
        [Range(0, 23, ErrorMessage = "Please enter value between {1} to {2}!")]
        public int? TimespanHours { get; set; }

        [Required]
        [Display(Name = "Minutes")]
        [Range(0, 59,ErrorMessage = "Please enter value between {1} to {2}!")]
        public int? TimespanMinutes { get; set; }

        [Display(Name = "Date Volunteered")]
        [Required(ErrorMessage = "Please select Date Volunteered!")]
        [SelectTodayDate]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateVolunteered { get; set; }

        [Required]
        public int? Action { get; set; }

        [Display(Name = "Message")]
        public string? Notes { get; set; }
        
        public bool IsEditable { get; set; } = false;
    }

    public class SelectTodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime selectedDate;
            if (DateTime.TryParse(value.ToString(), out selectedDate))
            {
                // Compare the selected date with today's date
                if (selectedDate.Date == DateTime.Today)
                {
                    return true;
                }
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} must be today's date.";
        }
    }
}
