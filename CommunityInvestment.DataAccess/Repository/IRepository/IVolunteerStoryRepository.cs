using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IVolunteerStoryRepository
    {
        public List<Story> GetAllStories();
        public Story GetStoryById(long storyId);
        public Story GetUserStory(long storyId, long userId);
        public List<MissionApplication> GetUserVolunteeredMA(long userId);
        public Task<bool> CreateStory(StoryCreateVM storyCreateObj);
        public List<StoryMedium> GetExistingImages(long storyId);
        public List<StoryMedium> GetExistingVideos(long storyId);
        public StoryMedium GetStoryImage(long storyId, string fileName);
        public bool RemoveStoryImage(StoryMedium storyMediumObj);
        public void RecommendToCoWorker(long storyId, long userId, long coWorkerId);
        public List<Story> GetUserStories(long userId, Dictionary<string, string> filters = null);
        public PageList<Story> GetUserStoriesPage(long userId, int pageNo, int pageSize, Dictionary<string, string> filters = null);
        public Story GetUserPreviewStory(long userId, long storyId);
        public int GetStoryViewCounts(long storyId);
        public void IncrementStoryViewCount(long storyId, long userId);
        public Story RemoveStory(long StoryId, long userId);
    }
}
