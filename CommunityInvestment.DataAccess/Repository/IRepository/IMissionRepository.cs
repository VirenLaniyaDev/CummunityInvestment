using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IMissionRepository
    {
        public Country GetCountryById(long countryId);
        public City GetCityById(long cityId);
        public PageList<MissionsDetailsVM> GetAllMissions(string userId, int pageNo, int pageSize, MissionFilter missionFilter = null, string MissionSort = null);
        public MissionDetailsVM GetMissionById(long missionId, string userId);
        public List<MissionApplication> GetMissionApplications(long missionId);
        public bool SetFavoriteMission(long MissionId, long UserId);
        public void PostComment(long missionId, long userId, string commentText);
        public void SetUserRatings(long missionId, long userId, string ratings);
        public string ApplyMission(long userId, long missionId);
        public void RecommendToCoWorker(long missionId, long userId, long coWorkerId);
    }
}
