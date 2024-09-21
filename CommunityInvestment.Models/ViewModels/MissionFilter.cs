using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class MissionFilter
    {
        public string SearchInput { get; set; } = null;
        public string[] Country { get; set; }
        public string[] City { get; set; }
        public string[] MissionThemes { get; set; }
        public string[] MissionSkills { get; set; }
    }
}
