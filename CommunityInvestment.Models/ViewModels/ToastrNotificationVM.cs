using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class ToastrNotificationVM
    {
        public string? Type { get; set; } = "Success";
        public string? Title { get; set; } = null!;
        public string? Message { get; set; } = null!;
    }
}
