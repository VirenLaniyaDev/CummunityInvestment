using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class UserNotification
    {
        public long UserNotificationId { get; set; }
        public long? UserId { get; set; } = null!;
        public string NotificationFor { get; set; } = null!;
        public string? NotificationMessage { get; set; }
        public string? NotificationLink { get; set; }
        public byte? IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
