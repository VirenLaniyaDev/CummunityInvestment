using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models.ViewModels;
using CommunityInvestment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using CommunityInvestment.Application.Common;

namespace CommunityInvestment.DataAccess.Repository
{
    public class AdminMissionRepository : Repository<Mission>, IAdminMissionRepository
    {
        public readonly CommunityInvestmentContext _db;
        public AdminMissionRepository(CommunityInvestmentContext db) :base(db)
        {
            _db = db;
        }

        //--- Manage Missions
        public DataTableVM<Mission> GetMissionsDT(DataTableFilterVM missionsDTFilter)
        {
            var Missions = _db.Missions.Where(mission => mission.DeletedAt == null).AsQueryable();
            if (!string.IsNullOrEmpty(missionsDTFilter.Search))
            {
                Missions = Missions.Where(mission => mission.Title.ToLower().Contains(missionsDTFilter.Search.ToLower().Trim()));
            }
            switch (missionsDTFilter.SortBy)
            {
                case "startDate":
                    Missions = missionsDTFilter.SortOrder == "desc" ?
                        Missions.OrderByDescending(mission => mission.StartDate) :
                        Missions.OrderBy(mission => mission.StartDate);
                    break;
                case "endDate":
                    Missions = missionsDTFilter.SortOrder == "desc" ?
                        Missions.OrderByDescending(mission => mission.EndDate) :
                        Missions.OrderBy(mission => mission.EndDate);
                    break;
                case "missionType":
                    Missions = missionsDTFilter.SortOrder == "desc" ?
                        Missions.OrderByDescending(mission => mission.MissionType).ThenByDescending(mission => mission.UpdatedAt ?? mission.CreatedAt) :
                        Missions.OrderBy(mission => mission.MissionType).ThenByDescending(mission => mission.UpdatedAt ?? mission.CreatedAt);
                    break;
                default:
                    Missions = Missions.OrderByDescending(mission => mission.UpdatedAt ?? mission.CreatedAt);
                    break;
            }
            int totalRecords = Missions.Count();
            Missions = Missions.Skip(missionsDTFilter.PageStart).Take(missionsDTFilter.PageLength);
            DataTableVM<Mission> MissionsDT = new DataTableVM<Mission>(Missions.ToList(), totalRecords);
            return MissionsDT;
        }

        public List<MissionMedium> GetExistingMissionImages(long missionId)
        {
            var missionImages = _db.MissionMedia.Where(mm => mm.MissionId == missionId && mm.MediaType == "image" && mm.DeletedAt == null);
            return missionImages.ToList();
        }

        public List<MissionDocument> GetExistingMissionDocuments(long missionId)
        {
            var missionDocuments = _db.MissionDocuments.Where(mm => mm.MissionId == missionId && mm.DeletedAt == null);
            return missionDocuments.ToList();
        }

        public async Task<bool> SaveMission(AdminMissionVM missionDataObj)
        {
            try
            {
                var _Missions = _db.Missions.AsQueryable();
                Mission missionObj;
                if (missionDataObj.MissionId != null)
                {
                    missionObj = await _db.Missions.FindAsync(missionDataObj.MissionId);
                    if (missionObj == null)
                    {
                        throw new Exception("Mission not found!");
                    }
                }
                else
                {
                    missionObj = new Mission();
                    missionObj.CreatedAt = DateTime.Now;
                }
                //--- Saving Mission
                missionObj.CityId = missionDataObj.CityId;
                missionObj.CountryId = missionDataObj.CountryId;
                missionObj.ThemeId = missionDataObj.ThemeId;
                missionObj.Title = missionDataObj.Title;
                missionObj.ShortDescription = missionDataObj.ShortDescription;
                missionObj.Description = WebUtility.HtmlEncode(missionDataObj.Description);
                missionObj.StartDate = missionDataObj.StartDate;
                missionObj.EndDate = missionDataObj.EndDate;
                missionObj.RegistrationDeadline = missionDataObj.RegistrationDeadline;
                missionObj.MissionType = missionDataObj.MissionType;
                missionObj.Status = missionDataObj.Status;
                missionObj.OrganizationName = missionDataObj.OrganizationName;
                missionObj.OrganizationDetail = WebUtility.HtmlEncode(missionDataObj.OrganizationDetail);
                missionObj.Availability = missionDataObj.Availability;
                missionObj.UpdatedAt = DateTime.Now;
                _db.Missions.Update(missionObj);
                await _db.SaveChangesAsync();   // Save change because we need MissionId for newly added mission

                // Updating Goal Mission
                GoalMission goalMission = _db.GoalMissions.FirstOrDefault(gm => gm.MissionId == missionDataObj.MissionId);
                if (goalMission == null)
                {
                    goalMission = new GoalMission();
                    goalMission.CreatedAt = DateTime.Now;
                }
                goalMission.MissionId = missionObj.MissionId;
                goalMission.GoalObjectiveText = missionDataObj.GoalObjectiveText;
                goalMission.GoalValue = missionDataObj.GoalValue;
                goalMission.UpdatedAt = DateTime.Now;
                _db.GoalMissions.Update(goalMission);

                // Updating Mission skills
                var _MissionSkills = _db.MissionSkills.Where(ms => ms.MissionId == missionDataObj.MissionId);
                DateTime dateTime = DateTime.Now;
                await _MissionSkills.ForEachAsync(us => us.DeletedAt = dateTime);
                foreach (long skillId in missionDataObj.SkillIds)
                {
                    MissionSkill missionSkillObj = new MissionSkill();
                    if (_MissionSkills.Any(us => us.SkillId == skillId))
                    {
                        missionSkillObj = _MissionSkills.FirstOrDefault(us => us.SkillId == skillId);
                        missionSkillObj.UpdatedAt = DateTime.Now;
                        missionSkillObj.DeletedAt = null;
                        _db.MissionSkills.Update(missionSkillObj);
                    }
                    else
                    {
                        missionSkillObj.SkillId = skillId;
                        missionSkillObj.MissionId = missionObj.MissionId;
                        missionSkillObj.CreatedAt = DateTime.Now;
                        _db.MissionSkills.Add(missionSkillObj);
                    }
                }

                //-- Adding Mission Images to DB
                if (!string.IsNullOrEmpty(missionDataObj.MissionImagesUniqueNames))
                {
                    List<string> uniqueFileNamesList = missionDataObj.MissionImagesUniqueNames?.Split(',').ToList();
                    for (int i = 0; i < uniqueFileNamesList.Count; i++)
                    {
                        MissionMedium missionImageObj = new MissionMedium
                        {
                            MissionId = missionObj.MissionId,
                            MediaName = uniqueFileNamesList[i].Substring(uniqueFileNamesList[i].IndexOf("_") + 1),
                            MediaType = "image",
                            MediaPath = "/data/Images/Mission/" + uniqueFileNamesList[i],
                            Default = "0",
                            CreatedAt = DateTime.Now
                        };
                        _db.MissionMedia.Add(missionImageObj);
                    }
                }

                //-- Adding Mission Documents to DB
                if (!string.IsNullOrEmpty(missionDataObj.MissionDocumentsUniqueNames))
                {
                    List<string> uniqueFileNamesList = missionDataObj.MissionDocumentsUniqueNames?.Split(',').ToList();
                    for (int i = 0; i < uniqueFileNamesList.Count; i++)
                    {
                        MissionDocument missionDocumentObj = new MissionDocument
                        {
                            MissionId = missionObj.MissionId,
                            DocumentName = uniqueFileNamesList[i].Substring(uniqueFileNamesList[i].IndexOf("_") + 1),
                            DocumentType = uniqueFileNamesList[i].Split('.').Last(),
                            DocumentPath = "/data/Documents/MissionDocs/" + uniqueFileNamesList[i],
                            CreatedAt = DateTime.Now
                        };
                        _db.MissionDocuments.Add(missionDocumentObj);
                    }
                }


                //-- Notification for Users
                if(missionDataObj.MissionId == null)
                {
                    var userIds = _db.Users.Select(u => u.UserId).ToList();
                    var userNotificationSettings = _db.NotificationSettings.ToList();
                    foreach (var userId in userIds)
                    {
                        var userSetting = userNotificationSettings.FirstOrDefault(uns => uns.UserId.Equals(userId));
                        if (userSetting != null && userSetting.NewMissions == 1) 
                        {
                            UserNotificationVM newMissionNotification = new UserNotificationVM
                            {
                                UserId = userId,
                                NotificationFor = Notification.NewMission,
                                NotificationLink = Notification.GetMissionPath(missionObj.MissionId),
                                NotificationMessage = Notification.GetNotificationMessage(Notification.NewMission, missionObj.Title)
                            };
                            AddOrUpdateNotification(newMissionNotification);
                        }
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AdminMissionVM GetMissionEdit(long missionId)
        {
            Mission mission = _db.Missions.Include(m => m.MissionSkills.Where(ms => ms.DeletedAt == null)).FirstOrDefault(m => m.MissionId == missionId && m.DeletedAt == null);
            GoalMission goalMission = _db.GoalMissions.FirstOrDefault(m => m.MissionId == missionId && m.DeletedAt == null);
            if (mission == null)
                return null;
            AdminMissionVM missionEdit = new AdminMissionVM
            {
                MissionId = mission.MissionId,
                CountryId = mission.CountryId,
                CityId = mission.CityId,
                ThemeId = mission.ThemeId,
                Title = mission.Title,
                ShortDescription = mission.ShortDescription,
                Description = WebUtility.HtmlDecode(mission.Description),
                StartDate = mission.StartDate,
                EndDate = mission.EndDate,
                RegistrationDeadline = mission.RegistrationDeadline,
                GoalValue = goalMission != null ? goalMission.GoalValue : 0,
                GoalObjectiveText = goalMission?.GoalObjectiveText,
                MissionType = mission.MissionType,
                Status = mission.Status,
                OrganizationName = mission.OrganizationName,
                OrganizationDetail = WebUtility.HtmlDecode(mission.OrganizationDetail),
                Availability = mission.Availability,
                SkillIds = mission.MissionSkills.Select(m => m.SkillId).ToList(),
            };
            return missionEdit;
        }

        public MissionMedium GetMissionImage(long missionImageId)
        {
            var missionImage = _db.MissionMedia.Find(missionImageId);
            return missionImage;
        }

        public bool RemoveMissionImage(MissionMedium missionMediumObj)
        {
            try
            {
                missionMediumObj.DeletedAt = DateTime.Now;
                _db.MissionMedia.Update(missionMediumObj);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public MissionDocument GetMissionDocument(long missionDocumentId)
        {
            var missionDocument = _db.MissionDocuments.Find(missionDocumentId);
            return missionDocument;
        }

        public bool RemoveMissionDocument(MissionDocument missionDocumentObj)
        {
            try
            {
                missionDocumentObj.DeletedAt = DateTime.Now;
                _db.MissionDocuments.Update(missionDocumentObj);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveMission(long missionId)
        {
            try
            {
                var _Mission = _db.Missions.Find(missionId);
                if (_Mission == null)
                    throw new Exception("Mission not found!");
                _Mission.DeletedAt = DateTime.Now;
                _Mission.UpdatedAt = DateTime.Now;
                _db.Missions.Update(_Mission);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //--- Mission Themes ---//
        public DataTableVM<MissionTheme> GetMissionThemesDT(DataTableFilterVM missionThemesDTFilter)
        {
            var _MissionThemes = _db.MissionThemes
                .Where(ma => ma.DeletedAt == null)
                .AsQueryable();
            if (!string.IsNullOrEmpty(missionThemesDTFilter.Search))
            {
                _MissionThemes = _MissionThemes.Where(ma => ma.Title.ToLower().Contains(missionThemesDTFilter.Search.ToLower().Trim()));
            }
            switch (missionThemesDTFilter.SortBy)
            {
                default:
                    _MissionThemes = missionThemesDTFilter.SortOrder == "desc" ? _MissionThemes.OrderByDescending(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt) : _MissionThemes.OrderBy(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt);
                    break;
            }
            int totalRecords = _MissionThemes.Count();
            _MissionThemes = _MissionThemes.Skip(missionThemesDTFilter.PageStart).Take(missionThemesDTFilter.PageLength);
            DataTableVM<MissionTheme> MissionThemesDT = new DataTableVM<MissionTheme>(_MissionThemes.ToList(), totalRecords);
            return MissionThemesDT;
        }

        public string AddOrUpdateMissionTheme(AdminMissionThemeVM missionThemeDataObj)
        {
            try
            {
                MissionTheme? missionThemeObj;
                string response;
                if (missionThemeDataObj.MissionThemeId != null)
                {
                    missionThemeObj = _db.MissionThemes.Find(missionThemeDataObj.MissionThemeId);
                    if (missionThemeObj == null)
                        return null;
                    response = "Mission Theme Updated!";
                }
                else
                {
                    missionThemeObj = new MissionTheme();
                    missionThemeObj.CreatedAt = DateTime.Now;
                    response = "Mission Theme Created!";
                }
                missionThemeObj.Title = missionThemeDataObj.ThemeTitle;
                missionThemeObj.Status = missionThemeDataObj.Status;
                missionThemeObj.UpdatedAt = DateTime.Now;
                _db.MissionThemes.Update(missionThemeObj);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool RemoveMissionTheme(long missionThemeId)
        {
            MissionTheme? missionTheme = _db.MissionThemes.Find(missionThemeId);
            if (missionTheme != null)
            {
                missionTheme.UpdatedAt = DateTime.Now;
                missionTheme.DeletedAt = DateTime.Now;
                _db.MissionThemes.Update(missionTheme);
                return true;
            }
            return false;
        }


        //--- Mission Skills ---//
        public DataTableVM<Skill> GetMissionSkillsDT(DataTableFilterVM missionSkillsDTFilter)
        {
            var _MissionSkills = _db.Skills
                .Where(ma => ma.DeletedAt == null)
                .AsQueryable();
            if (!string.IsNullOrEmpty(missionSkillsDTFilter.Search))
            {
                _MissionSkills = _MissionSkills.Where(ms => ms.SkillName != null ? ms.SkillName.ToLower().Contains(missionSkillsDTFilter.Search.ToLower().Trim()) : false);
            }
            switch (missionSkillsDTFilter.SortBy)
            {
                default:
                    _MissionSkills = missionSkillsDTFilter.SortOrder == "desc" ? _MissionSkills.OrderByDescending(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt) : _MissionSkills.OrderBy(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt);
                    break;
            }
            int totalRecords = _MissionSkills.Count();
            _MissionSkills = _MissionSkills.Skip(missionSkillsDTFilter.PageStart).Take(missionSkillsDTFilter.PageLength);
            DataTableVM<Skill> MissionSkillsDT = new DataTableVM<Skill>(_MissionSkills.ToList(), totalRecords);
            return MissionSkillsDT;
        }

        public string AddOrUpdateMissionSkill(AdminMissionSkillVM skillDataObj)
        {
            try
            {
                Skill skillObj;
                string response;
                if (skillDataObj.SkillId != null)
                {
                    skillObj = _db.Skills.Find(skillDataObj.SkillId);
                    if (skillObj == null)
                        return null;
                    response = "Skill Updated Successfully!";
                }
                else
                {
                    skillObj = new Skill();
                    skillObj.CreatedAt = DateTime.Now;
                    response = "Skill Created Successfully!";
                }
                skillObj.SkillName = skillDataObj.SkillName;
                skillObj.Status = skillDataObj.Status;
                skillObj.UpdatedAt = DateTime.Now;
                _db.Skills.Update(skillObj);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool RemoveSkill(long skillId)
        {
            Skill? skill = _db.Skills.Find(skillId);
            if (skill != null)
            {
                skill.UpdatedAt = DateTime.Now;
                skill.DeletedAt = DateTime.Now;
                _db.Skills.Update(skill);
                return true;
            }
            return false;
        }

        //--- Handling Mission Applications ---//
        public DataTableVM<AdminMissionApplicationsVM> GetMissionApplicationsDT(DataTableFilterVM missionApplicationsDTFilter)
        {
            var _MissionApplications = _db.MissionApplications
                .Where(ma => ma.ApprovalStatus == "pending" && ma.DeletedAt == null)
                .Include(ma => ma.Mission)
                .Include(ma => ma.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(missionApplicationsDTFilter.Search))
            {
                _MissionApplications = _MissionApplications.Where(ma => ma.Mission.Title.ToLower().Contains(missionApplicationsDTFilter.Search.ToLower().Trim()));
            }
            switch (missionApplicationsDTFilter.SortBy)
            {
                case "appliedDate":
                    _MissionApplications = missionApplicationsDTFilter.SortOrder == "desc" ?
                        _MissionApplications.OrderByDescending(ma => ma.AppliedAt) :
                        _MissionApplications.OrderBy(ma => ma.AppliedAt);
                    break;
                default:
                    _MissionApplications = _MissionApplications.OrderBy(ma => ma.CreatedAt);
                    break;
            }
            int totalRecords = _MissionApplications.Count();
            _MissionApplications = _MissionApplications.Skip(missionApplicationsDTFilter.PageStart).Take(missionApplicationsDTFilter.PageLength);
            List<AdminMissionApplicationsVM> adminMissionApplications = new List<AdminMissionApplicationsVM>();
            foreach (var missionApplication in _MissionApplications)
            {
                AdminMissionApplicationsVM adminMissionApplicationObj = new AdminMissionApplicationsVM
                {
                    MissionApplicationId = missionApplication.MissionApplicationId,
                    MissionId = missionApplication.MissionId,
                    UserId = missionApplication.UserId,
                    MissionTitle = missionApplication.Mission.Title,
                    UserName = missionApplication.User.FirstName + " " + missionApplication.User.LastName,
                    AppliedAt = missionApplication.AppliedAt
                };
                adminMissionApplications.Add(adminMissionApplicationObj);
            }
            DataTableVM<AdminMissionApplicationsVM> MissionApplicationsDT = new DataTableVM<AdminMissionApplicationsVM>(adminMissionApplications, totalRecords);
            return MissionApplicationsDT;
        }

        public string MissionApplicationAction(long missionApplicationId, string action)
        {
            try
            {
                var missionApplication = _db.MissionApplications.Include(ma=>ma.Mission).FirstOrDefault(ma=>ma.MissionApplicationId.Equals(missionApplicationId));
                if (missionApplication == null)
                    return null;
                string notificationFor = string.Empty;
                string responseMessage = string.Empty;
                switch (action)
                {
                    case "approve":
                        missionApplication.ApprovalStatus = Status.Approved;
                        responseMessage = $"User({missionApplication.UserId})'s Application for Mission({missionApplication.MissionId}) has been Approved!";
                        notificationFor = Notification.MissionApproved;
                        break;
                    case "reject":
                        missionApplication.ApprovalStatus = Status.Rejected;
                        responseMessage = $"User({missionApplication.UserId})'s Application for Mission({missionApplication.MissionId}) has been Rejected!";
                        notificationFor = Notification.MissionRejected;
                        break;
                    default:
                        missionApplication.ApprovalStatus = Status.Pending;
                        break;

                }
                _db.MissionApplications.Update(missionApplication);

                var userNotificationSetting = _db.NotificationSettings.FirstOrDefault(setting=>setting.UserId.Equals(missionApplication.UserId));
                if (userNotificationSetting != null && userNotificationSetting.NewMessages == 1 && !string.IsNullOrEmpty(notificationFor)) 
                {
                    UserNotificationVM missionApplicationApprovedNotification = new UserNotificationVM
                    {
                        UserId = missionApplication.UserId,
                        NotificationFor = notificationFor,
                        NotificationLink = Notification.GetMissionPath(missionApplication.MissionId),
                        NotificationMessage = Notification.GetNotificationMessage(notificationFor, missionApplication.Mission.Title)
                    };
                    AddOrUpdateNotification(missionApplicationApprovedNotification);
                }
                
                return responseMessage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //--- Manage Stories ---//
        public DataTableVM<AdminStoryVM> GetStoriesDT(DataTableFilterVM storiesDTFilter)
        {
            var _Stories = _db.Stories
                .Where(ma => ma.Status == "pending" && ma.DeletedAt == null)
                .Include(ma => ma.Mission)
                .Include(ma => ma.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(storiesDTFilter.Search))
            {
                _Stories = _Stories.Where(story => story.Title.ToLower().Contains(storiesDTFilter.Search.ToLower().Trim()) || story.Mission.Title.ToLower().Contains(storiesDTFilter.Search.ToLower().Trim()));
            }
            _Stories = _Stories.OrderByDescending(story => story.UpdatedAt ?? story.CreatedAt);

            int totalRecords = _Stories.Count();
            _Stories = _Stories.Skip(storiesDTFilter.PageStart).Take(storiesDTFilter.PageLength);
            List<AdminStoryVM> adminStories = new List<AdminStoryVM>();
            foreach (var story in _Stories)
            {
                AdminStoryVM adminMissionApplicationObj = new AdminStoryVM
                {
                    StoryId = story.StoryId,
                    StoryTitle = story.Title,
                    MissionTitle = story.Mission.Title,
                    UserName = story.User.FirstName + " " + story.User.LastName
                };
                adminStories.Add(adminMissionApplicationObj);
            }
            DataTableVM<AdminStoryVM> StoriesDT = new DataTableVM<AdminStoryVM>(adminStories, totalRecords);
            return StoriesDT;
        }

        public string StoryAction(long storyId, string action)
        {
            try
            {
                var story = _db.Stories.Find(storyId);
                if (story == null)
                    return null;
                string notificationFor = string.Empty;
                string responseMessage = string.Empty;
                switch (action)
                {
                    case "approve":
                        story.Status = Status.Published;
                        responseMessage = $"User({story.UserId})'s Story for Mission({story.MissionId}) has been Approved!";
                        notificationFor = Notification.StoryApproved;
                        break;
                    case "reject":
                        story.Status = Status.Rejected;
                        responseMessage = $"User({story.UserId})'s Story for Mission({story.MissionId}) has been Rejected!";
                        notificationFor = Notification.StoryRejected;
                        break;
                    default:
                        story.Status = Status.Pending;
                        break;

                }
                _db.Stories.Update(story);

                var userNotificationSetting = _db.NotificationSettings.FirstOrDefault(setting => setting.UserId.Equals(story.UserId));
                if (userNotificationSetting != null && userNotificationSetting.UserStories == 1 && !string.IsNullOrEmpty(notificationFor))
                {
                    UserNotificationVM storyNotification = new UserNotificationVM
                    {
                        UserId = story.UserId,
                        NotificationFor = notificationFor,
                        NotificationLink = Notification.GetStoryPath(story.StoryId, notificationFor),
                        NotificationMessage = Notification.GetNotificationMessage(notificationFor, story.Title)
                    };
                    AddOrUpdateNotification(storyNotification);
                }

                return responseMessage;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool RemoveStory(long storyId)
        {
            Story? story = _db.Stories.Find(storyId);
            if (story != null)
            {
                story.UpdatedAt = DateTime.Now;
                story.DeletedAt = DateTime.Now;
                _db.Stories.Update(story);
                return true;
            }
            return false;
        }
    }
}
