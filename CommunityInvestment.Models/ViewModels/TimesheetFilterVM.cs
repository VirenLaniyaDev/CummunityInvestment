using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class TimesheetFilterVM
    {
        public string? MissionType { get; set; } = string.Empty;
        public string? SearchString { get; set; } = string.Empty;
        public string? SortBy { get; set; } = string.Empty;
        public string? SortOrder { get; set; } = "asc";
        public int startPage { get; set; } = 1;
        public int pageLength { get; set; } = 10;
    }
}
