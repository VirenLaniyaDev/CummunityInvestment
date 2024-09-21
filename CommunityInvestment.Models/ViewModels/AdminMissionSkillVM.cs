using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminMissionSkillVM
    {
        public long? SkillId { get; set; }

        [Required]
        [Display(Name = "Skill Name")]
        [StringLength(255, ErrorMessage = "Only 255 characters are allowed!")]
        public string? SkillName { get; set; }

        [Required]
        public string Status { get; set; } = null!;

    }
}
