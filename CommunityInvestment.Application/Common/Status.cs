using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Application.Common
{
    public class Status
    {
        // Approval Status
        public static readonly string Approved = "approved";
        public static readonly string Rejected = "rejected";
        public static readonly string Pending = "pending";
        public static readonly string Published = "published";

        // Active / In-Active
        public static readonly string Active = "1";
        public static readonly string InActive = "0";
    }
}
