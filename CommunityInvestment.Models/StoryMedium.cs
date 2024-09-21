using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class StoryMedium
    {
        public long StoryMediaId { get; set; }
        public long StoryId { get; set; }
        public string StoryType { get; set; } = null!;
        public string StoryPath { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Story Story { get; set; } = null!;
    }
}
