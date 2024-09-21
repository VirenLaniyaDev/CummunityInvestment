using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class ContactUsVM
    {
        public long UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Subject { get; set; }

        [Required]
        [StringLength(60000)]
        public string? Message { get; set; }
    }
}
