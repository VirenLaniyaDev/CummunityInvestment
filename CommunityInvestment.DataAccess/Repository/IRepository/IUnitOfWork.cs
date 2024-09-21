using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        UserRepository User { get; }
        FilterRepository Filter { get; }
        MissionRepository Mission { get; }
        VolunteerStoryRepository VolunteerStory { get; }
        AdminRepository Admin { get; }
        AdminMissionRepository AdminMission { get; }

        void Save();
    }
}
