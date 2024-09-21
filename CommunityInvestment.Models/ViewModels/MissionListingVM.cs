using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class MissionListingVM
    {
        public int MissionCount { get; set; }
        public Pager Pager { get; set; }
        public List<MissionsDetailsVM> Missions { get; set; }
    }
}
