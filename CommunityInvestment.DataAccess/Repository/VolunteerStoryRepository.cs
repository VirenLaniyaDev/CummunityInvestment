using CommunityInvestment.Application.Services;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CommunityInvestment.Application.Common;

namespace CommunityInvestment.DataAccess.Repository
{
    public class VolunteerStoryRepository : Repository<Story>, IVolunteerStoryRepository
    {
        public readonly CommunityInvestmentContext _db;
        public readonly IConfiguration _configuration;
        public VolunteerStoryRepository(CommunityInvestmentContext db, IConfiguration configuration) : base(db)
        {
            _db = db;
            _configuration = configuration;
        }

        public List<Story> GetAllStories()
        {
            var StoryMedia = _db.StoryMedia;
            List<Story> stories = _db.Stories
                .Include(s => s.Mission)
                .Include(s => s.Mission.Theme)
                .Include(s => s.User)
                .Include(s => s.StoryMedia.Where(sm=>sm.DeletedAt == null))
                .Where(s => s.Status.Equals("published"))
                .ToList();
            return stories;
        }

        public Story GetStoryById(long storyId)
        {
            try
            {
                Story? story = _db.Stories
                    .Include(s => s.Mission)
                    .Include(s => s.User)
                    .Include(s => s.StoryMedia.Where(sm => sm.DeletedAt == null))
                    .Include(s => s.StoryInvites)
                    .FirstOrDefault(s => s.StoryId == storyId);
                return story;
            }
            catch (ApplicationException ex)
            {
                return null;
            }
        }

        public Story GetUserStory(long storyId, long userId)
        {
            try
            {
                Story userStory = _db.Stories.FirstOrDefault(s => s.StoryId == storyId && s.UserId == userId && s.DeletedAt == null);
                if (userStory != null)
                {
                    userStory = GetStoryById(userStory.StoryId);
                }
                return userStory;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<MissionApplication> GetUserVolunteeredMA(long userId)
        {
            List<MissionApplication> userMissionApplications = _db.MissionApplications
                .Include(ma => ma.Mission)
                .Where(ma => ma.UserId.Equals(userId) && ma.ApprovalStatus == "approved")
                .ToList();
            return userMissionApplications;
        }

        public async Task<bool> CreateStory(StoryCreateVM storyCreateObj)
        {
            try
            {
                Story? storyObj;
                if (storyCreateObj.StoryId != null)
                {
                    storyObj = _db.Stories.Find(storyCreateObj.StoryId);
                    if (storyObj == null)
                        return false;
                }
                else
                {
                    storyObj = new Story();
                    storyObj.CreatedAt = DateTime.Now;
                }
                storyObj.MissionId = storyCreateObj.MissionId;
                storyObj.UserId = storyCreateObj.UserId;
                storyObj.Title = storyCreateObj.Title;
                storyObj.Description = storyCreateObj.Description;
                storyObj.PublishedAt = storyCreateObj.PublishedAt;
                storyObj.Status = storyCreateObj.StoryAction;
                storyObj.UpdatedAt = DateTime.Now;
                _db.Stories.Update(storyObj);
                await _db.SaveChangesAsync();

                //Story storyObj = new Story
                //{
                //    MissionId = storyCreateObj.MissionId,
                //    UserId = storyCreateObj.UserId,
                //    Title = storyCreateObj.Title,
                //    Description = storyCreateObj.Description,
                //    PublishedAt = storyCreateObj.PublishedAt,
                //    Status = storyCreateObj.StoryAction
                //};
                //if (storyCreateObj.StoryId != null)
                //{
                //    storyObj.StoryId = (long)storyCreateObj.StoryId;
                //    storyObj.UpdatedAt = DateTime.Now;
                //    _db.Stories.Update(storyObj);
                //}
                //else
                //{
                //    storyObj.CreatedAt = DateTime.Now;
                //    storyObj.UpdatedAt = DateTime.Now;
                //    _db.Stories.Add(storyObj);
                //}
                //await _db.SaveChangesAsync();

                if (!string.IsNullOrEmpty(storyCreateObj.StoryImagesUniqueNames))
                {
                    List<string> uniqueFileNamesList = storyCreateObj.StoryImagesUniqueNames?.Split(',').ToList();
                    for (int i = 0; i < uniqueFileNamesList.Count; i++)
                    {
                        StoryMedium storyImageObj = new StoryMedium
                        {
                            StoryId = storyObj.StoryId,
                            //StoryType = storyCreateObj.StoryImages[i].ContentType.Split("/")[0],
                            StoryType = "image",
                            StoryPath = "/data/Images/Story/" + uniqueFileNamesList[i],
                            CreatedAt = DateTime.Now
                        };
                        _db.StoryMedia.Add(storyImageObj);
                    }
                }

                if (!string.IsNullOrEmpty(storyCreateObj.StoryVideoURLs))
                {
                    //var _StoryMedia = _db.StoryMedia.AsQueryable();
                    //var deleteAllVideos = _StoryMedia
                    //    .Where(sm => sm.StoryId.Equals(storyCreateObj.StoryId) && sm.StoryType.Equals("video"));
                    List<StoryMedium> storyVideos = GetExistingVideos(storyObj.StoryId);
                    foreach (var videoMediaObj in storyVideos)
                    {
                        videoMediaObj.DeletedAt = DateTime.Now;
                    }
                    _db.StoryMedia.UpdateRange(storyVideos);
                    List<string> storyVideoURLsList = storyCreateObj.StoryVideoURLs?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (storyVideoURLsList?.Count > 0)
                    {
                        foreach (string storyVideoURL in storyVideoURLsList)
                        {
                            StoryMedium storyVideoObj = new StoryMedium();
                            if (storyVideos.Any(sm => sm.StoryPath == storyVideoURL))
                            {
                                storyVideoObj = storyVideos.FirstOrDefault(smv => smv.StoryPath == storyVideoURL);
                                storyVideoObj.DeletedAt = null;
                                storyVideoObj.UpdatedAt = DateTime.Now;
                                _db.StoryMedia.Update(storyVideoObj);
                            }
                            else
                            {
                                storyVideoObj = new StoryMedium
                                {
                                    StoryId = storyObj.StoryId,
                                    StoryType = "video",
                                    StoryPath = storyVideoURL,
                                    CreatedAt = DateTime.Now
                                };
                                _db.StoryMedia.Add(storyVideoObj);
                            }
                        };
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<StoryMedium> GetExistingImages(long storyId)
        {
            List<StoryMedium> storyImages = _db.StoryMedia.Where(sm => sm.StoryId == storyId && sm.StoryType == "image" && sm.DeletedAt == null).ToList();
            return storyImages;
        }

        public List<StoryMedium> GetExistingVideos(long storyId)
        {
            List<StoryMedium> storyVideos = _db.StoryMedia.Where(sm => sm.StoryId == storyId && sm.StoryType == "video" && sm.DeletedAt == null).ToList();
            return storyVideos;
        }

        public StoryMedium GetStoryImage(long storyId, string fileName)
        {
            string filePath = $"/data/Images/Story/{fileName}";
            List<StoryMedium> storyMediaList = _db.StoryMedia.ToList();
            StoryMedium storyImage = storyMediaList.FirstOrDefault(sm => sm.StoryId == storyId && sm.StoryPath.Equals(filePath) && sm.DeletedAt == null);
            return storyImage;
        }

        public bool RemoveStoryImage(StoryMedium storyMediumObj)
        {
            try
            {
                //_db.StoryMedia.Remove(storyMediumObj);
                storyMediumObj.DeletedAt = DateTime.Now;
                _db.StoryMedia.Update(storyMediumObj);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void RecommendToCoWorker(long storyId, long userId, long coWorkerId)
        {
            Story storyDetail = GetStoryById(storyId);
            User userDetails = _db.Users.FirstOrDefault(u => u.UserId.Equals(userId));
            User coWorkerDetails = _db.Users.FirstOrDefault(u => u.UserId.Equals(coWorkerId));
            StoryInvite storyRecommendObj = new StoryInvite
            {
                StoryId = storyId,
                FromUserId = userId,
                ToUserId = coWorkerId,
                CreatedAt = DateTime.Now
            };
            SendMail sendMail = new SendMail(_configuration.GetSection("EmailService:API_Key").Value);
            _db.Add(storyRecommendObj);
            sendMail.RecommendStoryToCoWorker(storyDetail, userDetails, coWorkerDetails);

            //-- Recommendation for story user notification
            var userNotificationSetting = _db.NotificationSettings.FirstOrDefault(setting => setting.UserId.Equals(userId));
            if (userNotificationSetting != null && userNotificationSetting.RecommendedStory == 1)
            {
                UserNotificationVM storyRecommendationNotification = new UserNotificationVM
                {
                    UserId = coWorkerDetails.UserId,
                    NotificationFor = Notification.StoryRecommendation,
                    NotificationLink = Notification.GetStoryPath(storyDetail.MissionId),
                    NotificationMessage = Notification.GetNotificationMessage(Notification.StoryRecommendation, storyDetail.Title)
                };
                AddOrUpdateNotification(storyRecommendationNotification);
            }
        }

        public List<Story> GetUserStories(long userId, Dictionary<string, string> filters = null)
        {
            //Deleted not shown
            var userStories = _db.Stories.OrderByDescending(us => us.UpdatedAt).Where(s => s.UserId == userId && s.DeletedAt == null);
            if (filters != null)
            {
                if (!string.IsNullOrEmpty((string?)filters["SearchString"]))
                {
                    userStories = userStories.Where(us => us.Title.Contains(filters["SearchString"]));
                }
                if (!string.IsNullOrEmpty(filters["StoryStatus"]))
                {
                    userStories = userStories.Where(us => us.Status.Equals(filters["StoryStatus"]));
                }
                if (!string.IsNullOrEmpty(filters["StorySortBy"]))
                {
                    switch (filters["StorySortBy"])
                    {
                        case "Latest":
                            userStories = userStories.OrderByDescending(us => us.UpdatedAt);
                            break;
                        case "Oldest":
                            userStories = userStories.OrderBy(us => us.UpdatedAt);
                            break;
                        default:
                            userStories = userStories.OrderByDescending(us => us.UpdatedAt);
                            break;
                    }
                }
            }

            return userStories.ToList();
        }

        public PageList<Story> GetUserStoriesPage(long userId, int pageNo, int pageSize, Dictionary<string, string> filters = null)
        {
            List<Story> userStories = GetUserStories(userId, filters);
            PageList<Story> userStoriesPage = new PageList<Story>(pageNo, pageSize, userStories);
            return userStoriesPage;
        }

        public Story GetUserPreviewStory(long userId, long storyId)
        {
            Story userStory = GetUserStory(storyId, userId);
            return userStory;
        }

        public int GetStoryViewCounts(long storyId)
        {
            int storyViewCount = _db.StoryViewCounts.Where(svc => svc.StoryId == storyId).Count();
            return storyViewCount;
        }

        public void IncrementStoryViewCount(long storyId, long userId)
        {
            StoryViewCount storyViewCountObj = _db.StoryViewCounts.FirstOrDefault(svc => svc.StoryId == storyId && svc.UserId == userId);
            if(storyViewCountObj != null)
            {
                storyViewCountObj.UpdatedAt = DateTime.Now;
                _db.StoryViewCounts.Update(storyViewCountObj);
            }
            else
            {
                storyViewCountObj = new StoryViewCount
                {
                    StoryId = storyId,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                _db.StoryViewCounts.Add(storyViewCountObj);
            }
        }

        public Story RemoveStory(long StoryId, long userId)
        {
            Story userStory = GetUserStory(StoryId, userId);
            if (userStory == null)
                throw new Exception("Story not exists!");
            var storyMediaQuery = userStory.StoryMedia.ToList();
            storyMediaQuery.ForEach(sm => sm.DeletedAt = DateTime.Now);
            _db.StoryMedia.UpdateRange(storyMediaQuery);
            userStory.DeletedAt = DateTime.Now;
            _db.Stories.Update(userStory);
            return userStory;
        }
    }
}
