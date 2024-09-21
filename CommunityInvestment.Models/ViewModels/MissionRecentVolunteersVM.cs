using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class MissionRecentVolunteersVM
    {
        public List<MissionApplication> MissionApplications { get; set; }
        public Pager Pager { get; set; }
    }
}
