using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class StoryViewCount
    {
        public long StoryViewCountId { get; set; }
        public long StoryId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Story Story { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
