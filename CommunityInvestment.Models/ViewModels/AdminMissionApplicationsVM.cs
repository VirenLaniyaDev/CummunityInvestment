using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class AdminMissionApplicationsVM
    {
        public long MissionApplicationId { get; set; }
        public long MissionId { get; set; }
        public long UserId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public string MissionTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
