using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class UserNotificationVM
    {
        public long? UserNotificationId { get; set; } = null!;
        public long? UserId { get; set; } = null!;
        public string NotificationFor { get; set; } = null!;
        public string? NotificationMessage { get; set; }
        public string? NotificationLink { get; set; }
        public byte? IsRead { get; set; } = 0;
    }
}
