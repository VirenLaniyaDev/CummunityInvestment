using CommunityInvestment.Models.ViewModels;
using CommunityInvestment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IAdminMissionRepository
    {
        public DataTableVM<Mission> GetMissionsDT(DataTableFilterVM missionsDTFilter);
        public List<MissionMedium> GetExistingMissionImages(long missionId);
        public List<MissionDocument> GetExistingMissionDocuments(long missionId);
        public Task<bool> SaveMission(AdminMissionVM missionDataObj);
        public AdminMissionVM GetMissionEdit(long missionId);
        public MissionMedium GetMissionImage(long missionImageId);
        public bool RemoveMissionImage(MissionMedium missionMediumObj);
        public MissionDocument GetMissionDocument(long missionDocumentId);
        public bool RemoveMissionDocument(MissionDocument missionDocumentObj);
        public bool RemoveMission(long missionId);


        public DataTableVM<MissionTheme> GetMissionThemesDT(DataTableFilterVM missionThemesDTFilter);
        public string AddOrUpdateMissionTheme(AdminMissionThemeVM missionThemeDataObj);
        public bool RemoveMissionTheme(long missionThemeId);


        public DataTableVM<Skill> GetMissionSkillsDT(DataTableFilterVM missionSkillsDTFilter);
        public string AddOrUpdateMissionSkill(AdminMissionSkillVM skillDataObj);
        public bool RemoveSkill(long skillId);


        public DataTableVM<AdminMissionApplicationsVM> GetMissionApplicationsDT(DataTableFilterVM missionApplicationsDTFilter);
        public string MissionApplicationAction(long missionApplicationId, string action);


        public DataTableVM<AdminStoryVM> GetStoriesDT(DataTableFilterVM storiesDTFilter);
        public string StoryAction(long storyId, string action);
        public bool RemoveStory(long storyId);
    }
}
