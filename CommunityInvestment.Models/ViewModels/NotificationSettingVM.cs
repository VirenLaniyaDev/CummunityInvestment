using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class NotificationSettingVM
    {
        public long? UserId { get; set; } = null!;
        public byte? RecommendedMissions { get; set; } = 0;
        public byte? VolunteeringHours { get; set; } = 0;
        public byte? VolunteeringGoals { get; set; } = 0;
        public byte? UserComments { get; set; } = 0;
        public byte? UserStories { get; set; } = 0;
        public byte? NewMissions { get; set; } = 0;
        public byte? NewMessages { get; set; } = 0;
        public byte? RecommendedStory { get; set; } = 0;
        public byte? NotificationByEmail { get; set; } = 0;
    }
}
