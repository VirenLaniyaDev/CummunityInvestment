using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class VolunteerStoryListingVM
    {
        public List<Story> Stories { get; set; }
        public Pager Pager { get; set; }
    }
}
