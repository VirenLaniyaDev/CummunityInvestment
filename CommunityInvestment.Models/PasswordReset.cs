using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class PasswordReset
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
