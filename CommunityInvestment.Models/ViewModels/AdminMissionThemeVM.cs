using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminMissionThemeVM
    {
        public long? MissionThemeId { get; set; }

        [Required]
        [Display(Name = "Theme Title")]
        [StringLength(255, ErrorMessage = "Only 255 characters are allowed!")]
        public string? ThemeTitle { get; set; }

        [Required]
        public byte Status { get; set; } = 0;

    }
}
