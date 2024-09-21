using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;

namespace CommunityInvestment.Application.Services
{
    public class SendMail
    {
        public string _apiKey;
        public SendMail(string apiKey)
        {
            _apiKey = apiKey;
        }
        public async Task ResetPassword(string EmailAddress, string link, string UserName = "")
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("test4purposes@gmail.com", "Community Investment");
            var to = new EmailAddress(EmailAddress);
            var message = new SendGridMessage();
            
            message.SetFrom(from);
            message.AddTo(to);
            var data = new
            {
                name = UserName,
                reset_password_url = link
            };
            message.SetTemplateId("d-c86648b9191d4c95b8346e61f74ea602");
            message.SetTemplateData(data);
            var response = client.SendEmailAsync(message);
        }
        
        public async Task RecommendToCoWorker(MissionDetailsVM mission, User user, User coWorker)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("test4purposes@gmail.com", "Community Investment");
            var to = new EmailAddress(coWorker.Email);
            var message = new SendGridMessage();
            
            message.SetFrom(from);
            message.AddTo(to);
            var data = new
            {
                coworker_name = coWorker.FirstName+" "+coWorker.LastName,
                user_name = user.FirstName+" "+user.LastName,
                mission_title = mission.Title,
                mission_description = mission.ShortDescription,
                start_date = mission.StartDate,
                end_date = mission.EndDate,
                availability = mission.Availability,
                mission_url = "https://localhost:44302/Users/Home/VolunteeringMission/" + mission.MissionId
            };
            message.SetTemplateId("d-c82fa20f6e8a449b88561a12ff5e441a");
            message.SetTemplateData(data);
            var response = client.SendEmailAsync(message);
        }
        
        public async Task RecommendStoryToCoWorker(Story storyDetail, User user, User coWorker)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("test4purposes@gmail.com", "Community Investment");
            var to = new EmailAddress(coWorker.Email);
            var message = new SendGridMessage();
            
            message.SetFrom(from);
            message.AddTo(to);
            var data = new
            {
                coworker_name = coWorker.FirstName+" "+coWorker.LastName,
                user_name = user.FirstName+" "+user.LastName,
                story_title = storyDetail.Title,
                mission_title = storyDetail.Mission.Title,
                storyWriter_name = storyDetail.User.FirstName+" "+storyDetail.User.LastName,
                published_at = storyDetail.PublishedAt,
                story_url = "https://localhost:44302/Users/VolunteerStory/Story/" + storyDetail.StoryId
            };
            message.SetTemplateId("d-119cf40080e044edb8eea0b60297e869");
            message.SetTemplateData(data);
            var response = client.SendEmailAsync(message);
        }


    }
}