using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Application.Common
{
    public class Notification
    {
        #region NotificationFor
        public static readonly string NewMission = "New Mission";
        public static readonly string MissionApproved = "Mission Approved";
        public static readonly string MissionRejected = "Mission Rejected";
        public static readonly string StoryApproved = "Story Approved";
        public static readonly string StoryRejected = "Story Rejected";
        public static readonly string MissionRecommendation = "Mission Recommendation";
        public static readonly string StoryRecommendation = "Story Recommendation";
        #endregion

        #region NotificationLinkPath
        public static readonly string MissionPath = "/Users/Home/VolunteeringMission/";
        public static readonly string StoryPath = "/Users/VolunteerStory/Story/";
        public static readonly string StoryRejectedPath = "/Users/VolunteerStory/Preview/";
        public static string GetMissionPath(long missionId)
        {
            return MissionPath + missionId;
        }

        public static string GetStoryPath(long storyId, string? notificationFor = null)
        {
            if (string.IsNullOrEmpty(notificationFor) && notificationFor == Notification.StoryRejected)
                return StoryRejectedPath + storyId;
            else 
                return StoryPath + storyId;
        }
        #endregion

        #region NotificationMessage
        /// <summary>
        /// Returns Notification Message for User.
        /// </summary>
        /// <param name="notificationFor">Which kind of notification user need.</param>
        /// <param name="titleString">Text which will be included in notification message.</param>
        /// <returns></returns>
        public static string GetNotificationMessage(string notificationFor, string? titleString)
        {
            string notificationMessage = string.Empty;
            switch(notificationFor)
            {
                case "New Mission":
                    notificationMessage = "New Mission : " + titleString;
                    break;
                case "Mission Approved":
                    notificationMessage = "Your Mission Application for '" + titleString + "' has been approved.";
                    break;
                case "Story Approved":
                    notificationMessage = "Your Story '" + titleString + "' has been published.";
                    break;
                case "Mission Rejected":
                    notificationMessage = "Your Mission Application for '" + titleString + "' has been rejected!";
                    break;
                case "Story Rejected":
                    notificationMessage = "Your Story '" + titleString + "' has been rejected and can not be published!";
                    break;
                case "Mission Recommendation":
                    notificationMessage = "You got a Mission recommendation : '" + titleString;
                    break;
                case "Story Recommendation":
                    notificationMessage = "You got a Story recommendation : '" + titleString;
                    break;
                default:
                    break;
            }
            return notificationMessage;
        }
        #endregion
    }
}
