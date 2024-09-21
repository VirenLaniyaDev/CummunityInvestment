using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminStoryVM
    {
        public long StoryId { get; set; }
        public string? StoryTitle { get; set; }
        public string MissionTitle { get; set; } = string.Empty;
        public string? UserName { get; set; }
    }
}
