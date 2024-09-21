using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class MissionRating
    {
        public long MissionRatingId { get; set; }
        public long MissionId { get; set; }
        public long UserId { get; set; }
        public string Rating { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Mission Mission { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
