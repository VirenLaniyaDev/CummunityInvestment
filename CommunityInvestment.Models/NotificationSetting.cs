using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class NotificationSetting
    {
        public long NotificationSettingId { get; set; }
        public long UserId { get; set; }
        public byte? RecommendedMissions { get; set; }
        public byte? VolunteeringHours { get; set; }
        public byte? VolunteeringGoals { get; set; }
        public byte? UserComments { get; set; }
        public byte? UserStories { get; set; }
        public byte? NewMissions { get; set; }
        public byte? NewMessages { get; set; }
        public byte? RecommendedStory { get; set; }
        public byte? NotificationByEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
