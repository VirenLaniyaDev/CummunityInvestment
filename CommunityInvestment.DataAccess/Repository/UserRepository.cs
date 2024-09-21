using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using NuGet.Common;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using CommunityInvestment.Models.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using System.Data;

namespace CommunityInvestment.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public readonly CommunityInvestmentContext _db;
        public readonly IConfiguration _configuration;
        public UserRepository(CommunityInvestmentContext db, IConfiguration configuration) : base(db)
        {
            _db = db;
            _configuration = configuration;
        }
        public User GetUserById(long id)
        {
            return _db.Users.FirstOrDefault(user => user.UserId == id && user.Status == "1");
        }

        public User GetByEmail(string email)
        {
            return _db.Users.FirstOrDefault(user => user.Email == email);
        }

        private string getHashedPassword(string Password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(16);
            return BCrypt.Net.BCrypt.HashPassword(Password, salt);
        }

        public void Register(User user)
        {
            user.Password = getHashedPassword(user.Password);
            _db.Users.Add(user);
        }

        public bool Authenticate(string UEmail, string UPassword)
        {
            User userResult = this.GetByEmail(UEmail);
            if ((userResult != null) && BCrypt.Net.BCrypt.Verify(UPassword, userResult.Password))
            {
                return true;
            }
            return false;
        }

        public void ForgotPassword(string UEmail)
        {
            var emailCheckResult = _db.PasswordResets.FirstOrDefault(pr => pr.Email == UEmail);
            if (this.GetByEmail(UEmail) != null)
            {
                string _token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                                    .Replace("+", "-")
                                    .Replace("/", "_")
                                    .TrimEnd('=');
                if (emailCheckResult != null)
                {
                    emailCheckResult.Token = _token;
                    emailCheckResult.CreatedAt = DateTime.Now;
                    _db.PasswordResets.Update(emailCheckResult);
                }
                else
                {
                    PasswordReset passwordResetObj = new PasswordReset
                    {
                        Email = UEmail,
                        Token = _token,
                        CreatedAt = DateTime.Now
                    };
                    _db.PasswordResets.Add(passwordResetObj);
                }
                string resetPassLink = "https://localhost:44302/Users/Authentication/ResetPassword/" + _token;
                SendMail sendMail = new SendMail(_configuration.GetSection("EmailService:API_Key").Value);
                sendMail.ResetPassword(UEmail, resetPassLink);
            }
            else
            {
                throw new ApplicationException("User not found!");
            }
        }

        public PasswordReset getPR_RecordByToken(string token)
        {
            PasswordReset result = _db.PasswordResets.FirstOrDefault(pr => pr.Token == token);
            if (result != null)
            {
                DateTime start = result.CreatedAt;
                DateTime end = DateTime.Now;
                TimeSpan duration = end - start;
                double hours = duration.TotalHours;
                if (hours < 4)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void UpdatePassword(string UEmail, string UPassword)
        {
            User user = this.GetByEmail(UEmail);
            if (user != null)
            {
                user.Password = getHashedPassword(UPassword);
                this.Save();
            }
            else
            {
                throw new ApplicationException("User not found!");
            }
        }

        public List<User> GetCoWorkers(long userId, string searchFilter)
        {
            var coWorkers = _db.Users.Where(u => u.UserId != userId && u.Status == "1");
            if (!String.IsNullOrEmpty(searchFilter))
            {
                coWorkers = coWorkers.Where(cw => (cw.FirstName + " " + cw.LastName).Contains(searchFilter));
            }
            return coWorkers.ToList();
        }

        public UserProfileVM GetUserProfileById(long userId)
        {
            User user = _db.Users
                .Include(u => u.City)
                .Include(u => u.Country)
                .Include(u => u.UserSkills.Where(us => us.DeletedAt == null))
                .FirstOrDefault(u => u.UserId == userId);
            UserProfileVM userProfile = new UserProfileVM();
            if (user != null)
            {
                userProfile = new UserProfileVM
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CountryId = user.CountryId,
                    CityId = user.CityId,
                    Avatar = user.Avatar,
                    WhyIVolunteer = user.WhyIVolunteer,
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    ProfileText = user.ProfileText,
                    Availability = user.Availability,
                    LinkedInUrl = user.LinkedInUrl,
                    Title = user.Title,
                    SkillIds = user.UserSkills.Select(us => us.SkillId).ToList(),
                    //City = user.City,
                    //Country = user.Country,
                    //CreatedAt = user.CreatedAt,
                    //UpdatedAt = user.UpdatedAt
                };
                return userProfile;
            }
            return userProfile;
        }

        public List<UserSkill> GetUserSkills(long userId)
        {
            var UserSkills = _db.UserSkills.Where(us => us.UserId == userId);
            var Skills = _db.Skills;
            UserSkills = UserSkills.Join(Skills, us => us.SkillId, s => s.SkillId, (us, s) => us);
            return UserSkills.ToList();
        }

        public async Task<User> UpdateUserProfileAsync(UserProfileVM userProfile)
        {
            try
            {
                User user = GetUserById(userProfile.UserId);
                if (user == null)
                    throw new Exception("User not found!");
                user.FirstName = userProfile.FirstName;
                user.LastName = userProfile.LastName;
                user.CountryId = userProfile.CountryId;
                user.CityId = userProfile.CityId;
                user.Avatar = userProfile.Avatar;
                user.Availability = userProfile.Availability;
                user.WhyIVolunteer = userProfile.WhyIVolunteer;
                user.EmployeeId = userProfile.EmployeeId;
                user.Department = userProfile.Department;
                user.ProfileText = userProfile.ProfileText;
                user.LinkedInUrl = userProfile.LinkedInUrl;
                user.Title = userProfile.Title;
                user.UpdatedAt = DateTime.Now;
                _db.Users.Update(user);

                // Updating User skills
                var _UserSkills = _db.UserSkills.Where(us => us.UserId == userProfile.UserId);
                DateTime dateTime = DateTime.Now;
                await _UserSkills.ForEachAsync(us => us.DeletedAt = dateTime);
                foreach (long skillId in userProfile.SkillIds)
                {
                    UserSkill userSkillObj = new UserSkill();
                    if (_UserSkills.Any(us => us.SkillId == skillId))
                    {
                        userSkillObj = _UserSkills.FirstOrDefault(us => us.SkillId == skillId);
                        userSkillObj.UpdatedAt = DateTime.Now;
                        userSkillObj.DeletedAt = null;
                        _db.UserSkills.Update(userSkillObj);
                    }
                    else
                    {
                        userSkillObj.SkillId = skillId;
                        userSkillObj.UserId = userProfile.UserId;
                        userSkillObj.CreatedAt = DateTime.Now;
                        _db.UserSkills.Add(userSkillObj);
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        //--- Change Password
        public bool CheckOldPassword(long userId, string oldPassword)
        {
            User user = GetUserById(userId);
            if (BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                return true;
            }
            return false;
        }

        public bool UpdatePassword(long userId, string newPassword)
        {
            User user = this.GetUserById(userId);
            if (user != null)
            {
                user.Password = getHashedPassword(newPassword);
                _db.Users.Update(user);
                return true;
            }
            else
                throw new ApplicationException("User not found!");
        }

        //--- Contact Us
        public bool SendQueryMessage(ContactUsVM contactUs)
        {
            try
            {
                ContactUs ContactUsObj = new ContactUs
                {
                    UserId = contactUs.UserId,
                    Subject = contactUs.Subject,
                    Message = contactUs.Message,
                    Status = "pending",
                    CreatedAt = DateTime.Now
                };
                _db.ContactUs.Add(ContactUsObj);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        ///---- Volunteering Timesheet 
        private IQueryable<Timesheet> GetAllUserTimesheets(long userId)
        {
            var userTimesheets = _db.Timesheets
                .Where(ts => ts.UserId == userId && ts.DeletedAt == null);
                //.Include(ts => ts.Mission);
            return userTimesheets;
        }

        public Timesheet GetTimesheetById(long timesheetId)
        {
            Timesheet timesheet = _db.Timesheets
                                .Include(ts => ts.Mission)
                                .FirstOrDefault(ts => ts.TimesheetId.Equals(timesheetId));
            return timesheet;
        }

        private bool IsTimesheetEditable(Timesheet timesheet)
        {
            bool IsTimesheetEditable = false;
            DateTime dateToday = DateTime.Today;
            DateTime dateVolunteered = ((DateTime)timesheet.DateVolunteered).Date;
            if (timesheet.Status != "approved" && dateVolunteered == dateToday)
            {
                if (timesheet.Mission?.MissionType == "time")
                {
                    DateTime? missionStartDate = timesheet.Mission?.StartDate != null ? ((DateTime)timesheet.Mission?.StartDate).Date : null;
                    DateTime? missionEndDate = timesheet.Mission?.StartDate != null ? ((DateTime)timesheet.Mission?.EndDate).Date : null;
                    if (missionStartDate != null && missionEndDate != null && missionStartDate < dateToday && missionEndDate > dateToday)
                        IsTimesheetEditable = true;
                }
                else
                    IsTimesheetEditable = true;
            }
            return IsTimesheetEditable;
        }

        private List<TimesheetVM> GetTimesheetVMList(IQueryable<Timesheet> timesheets)
        {
            if (timesheets != null)
            {
                //var timesheetsVMs = timesheets.Select(timesheet => new TimesheetVM
                //{
                //    TimesheetId = timesheet.TimesheetId,
                //    UserId = timesheet.UserId,
                //    MissionId = timesheet.MissionId,
                //    MissionTitle = timesheet.Mission.Title,
                //    MissionType = timesheet.Mission.MissionType,
                //    TimespanHours = (int)((TimeSpan)timesheet.TimesheetTime).Hours,
                //    TimespanMinutes = (int)((TimeSpan)timesheet.TimesheetTime).Minutes,
                //    //DateVolunteered = DateOnly.FromDateTime(timesheet.DateVolunteered),
                //    DateVolunteered = timesheet.DateVolunteered,
                //    Action = timesheet.Action,
                //    Notes = timesheet.Notes,
                //    IsEditable = IsTimesheetEditable(timesheet),
                //});
                //return timesheetsVMs.ToList();

                List<long> timesheetIds = timesheets.Select(ts => (long)ts.TimesheetId).ToList();
                var timesheetsVMs = new List<TimesheetVM>();
                foreach (var timesheetId in timesheetIds)
                {
                    timesheetsVMs.Add(GetTimesheetVM(timesheetId));
                }
                return timesheetsVMs;
            }
            return new List<TimesheetVM>();
        }

        public DataTableVM<TimesheetVM> GetUserTimesheets(long userId, TimesheetFilterVM timesheetFilter = null)
        {
            var userTimesheets = GetAllUserTimesheets(userId);
            int totalRecords = 0;
            if (userTimesheets != null)
            {
                if (timesheetFilter == null || string.IsNullOrEmpty(timesheetFilter.MissionType))
                    return new DataTableVM<TimesheetVM>(GetTimesheetVMList(userTimesheets), totalRecords);
                if (!string.IsNullOrEmpty(timesheetFilter.MissionType))
                    userTimesheets = userTimesheets.Where(ut => ut.Mission.MissionType == timesheetFilter.MissionType);
                if (!string.IsNullOrEmpty(timesheetFilter.SearchString))
                {
                    userTimesheets = userTimesheets.Where(ts => ts.Mission.Title.ToLower().Contains(timesheetFilter.SearchString.ToLower()));
                }
                if (!string.IsNullOrEmpty(timesheetFilter.SortBy))
                {
                    switch (timesheetFilter.SortBy)
                    {
                        case "timespanHours":
                            userTimesheets = (timesheetFilter.SortOrder == "asc") ? userTimesheets.OrderBy(ts => ((TimeSpan)ts.TimesheetTime).Hours) : userTimesheets.OrderByDescending(ts => ((TimeSpan)ts.TimesheetTime).Hours);
                            break;
                        case "timespanMinutes":
                            userTimesheets = (timesheetFilter.SortOrder == "asc") ? userTimesheets.OrderBy(ts => ((TimeSpan)ts.TimesheetTime).Minutes) : userTimesheets.OrderByDescending(ts => ((TimeSpan)ts.TimesheetTime).Minutes);
                            break;
                        case "action":
                            userTimesheets = (timesheetFilter.SortOrder == "asc") ? userTimesheets.OrderBy(ts => ts.Action) : userTimesheets.OrderByDescending(ts => ts.Action);
                            break;
                        default:
                            userTimesheets = (timesheetFilter.SortOrder == "asc") ? userTimesheets.OrderBy(ts => ts.DateVolunteered) : userTimesheets.OrderByDescending(ts => ts.DateVolunteered);
                            break;
                    }
                }
                totalRecords = userTimesheets.Count();
                userTimesheets = userTimesheets.Skip(timesheetFilter.startPage).Take(timesheetFilter.pageLength);
            }
            var userTimesheetVMList = GetTimesheetVMList(userTimesheets);
            DataTableVM<TimesheetVM> userTimesheetDT = new DataTableVM<TimesheetVM>(userTimesheetVMList, totalRecords);
            return userTimesheetDT;
        }

        public TimesheetVM GetTimesheetVM(long timesheetId)
        {
            var timesheet = GetTimesheetById(timesheetId);
            if (timesheet != null)
            {
                var timesheetsVM = new TimesheetVM
                {
                    TimesheetId = timesheet.TimesheetId,
                    UserId = timesheet.UserId,
                    MissionId = timesheet.MissionId,
                    MissionTitle = timesheet.Mission?.Title,
                    MissionType = timesheet.Mission?.MissionType,
                    TimespanHours = timesheet.TimesheetTime != null ? (int)((TimeSpan)timesheet.TimesheetTime).Hours : null,
                    TimespanMinutes = timesheet.TimesheetTime != null ? (int)((TimeSpan)timesheet.TimesheetTime).Minutes : null,
                    //DateVolunteered = DateOnly.FromDateTime(timesheet.DateVolunteered),
                    DateVolunteered = timesheet.DateVolunteered,
                    Action = timesheet.Action,
                    Notes = timesheet.Notes,
                    IsEditable = IsTimesheetEditable(timesheet)
                };
                return timesheetsVM;
            }
            else
            {
                return null;
            }
        }

        public List<MissionApplication> GetUserVolunteeredMAForTimesheet(long userId, string missionType)
        {
            var userMissionApplications = _db.MissionApplications
                .Include(ma => ma.Mission).AsQueryable();
            if(missionType == "time")
            {
                userMissionApplications = userMissionApplications.Where(ma => ma.UserId.Equals(userId) && ma.ApprovalStatus == "approved" && ma.Mission.StartDate < DateTime.Now && ma.Mission.EndDate > DateTime.Now && ma.Mission.MissionType.Equals(missionType));
            }
            else
            {
                userMissionApplications = userMissionApplications.Where(ma => ma.UserId.Equals(userId) && ma.ApprovalStatus == "approved" && ma.Mission.MissionType.Equals(missionType));
            }
            return userMissionApplications.ToList();
        }

        public bool AddOrUpdateTimesheet(TimesheetVM userTimesheet)
        {
            try
            {
                var _Timesheets = _db.Timesheets.Where(ts => ts.DeletedAt == null).AsQueryable();
                TimeSpan? timeSpent = null;
                if (userTimesheet.TimespanHours != null && userTimesheet.TimespanMinutes != null)
                    timeSpent = new TimeSpan((int)userTimesheet.TimespanHours, (int)userTimesheet.TimespanMinutes, 0);

                Timesheet timesheetObj = new Timesheet();

                if (userTimesheet?.TimesheetId != null && _Timesheets.Any(ts => ts.TimesheetId == userTimesheet.TimesheetId))
                {
                    timesheetObj = _db.Timesheets.FirstOrDefault(ts => ts.TimesheetId == userTimesheet.TimesheetId);
                    timesheetObj.TimesheetTime = timeSpent;
                    timesheetObj.DateVolunteered = userTimesheet.DateVolunteered;
                    timesheetObj.Action = userTimesheet.Action;
                    timesheetObj.Notes = userTimesheet.Notes;
                    timesheetObj.Status = "pending";
                    timesheetObj.UpdatedAt = DateTime.Now;
                    _db.Timesheets.Update(timesheetObj);
                }
                else
                {
                    timesheetObj = new Timesheet
                    {
                        UserId = userTimesheet.UserId,
                        MissionId = userTimesheet.MissionId,
                        TimesheetTime = timeSpent,
                        Action = userTimesheet.Action,
                        DateVolunteered = userTimesheet.DateVolunteered,
                        Notes = userTimesheet.Notes,
                        Status = "pending",
                        CreatedAt = DateTime.Now
                    };
                    _db.Timesheets.Add(timesheetObj);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveUserTimesheetEntry(long timesheetId)
        {
            try
            {
                Timesheet timesheet = _db.Timesheets.FirstOrDefault(ts => ts.TimesheetId.Equals(timesheetId));
                if (timesheet == null)
                    throw new Exception("Timesheet not found!");
                timesheet.DeletedAt = DateTime.Now;
                _db.Timesheets.Update(timesheet); 
                return true;
            } catch(Exception ex)
            {
                return false;
            }
        }


        #region User Notification
        /// <summary>
        /// Fetches User notifications from Database.
        /// </summary>
        /// <param name="userId">User Identifier</param>
        /// <returns></returns>
        public List<UserNotification> GetUserNotifications(long userId)
        {
            var userNotifications = _db.UserNotifications.Where(un => un.UserId.Equals(userId) && un.DeletedAt == null).AsQueryable();
            userNotifications = userNotifications.OrderByDescending(un => un.CreatedAt);
            return userNotifications.ToList();
        }

        /// <summary>
        /// Mark as read an user notification.
        /// </summary>
        /// <param name="userNotificationId">User Notification Identifier</param>
        /// <returns></returns>
        public bool NotificationMarkAsRead(long userNotificationId)
        {
            try
            {
                UserNotification? userNotification = _db.UserNotifications.Find(userNotificationId);
                if (userNotification == null)
                    throw new Exception("User notification not found!");
                userNotification.IsRead = 1;
                userNotification.UpdatedAt = DateTime.Now;
                _db.UserNotifications.Update(userNotification);
                return true;
            } 
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearAllNotifications(long userId)
        {
            try
            {
                var userNotifications = _db.UserNotifications.Where(un => un.UserId == userId && un.DeletedAt == null).AsQueryable();
                foreach(var userNotification in userNotifications)
                    userNotification.DeletedAt = DateTime.Now;
                _db.UserNotifications.UpdateRange(userNotifications);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool SaveNotificationSettings(NotificationSettingVM notificationSettings)
        {
            try
            {
                NotificationSetting userNotificationSettings = null;
                if(notificationSettings.UserId != null)
                    userNotificationSettings = _db.NotificationSettings.FirstOrDefault(ns => ns.UserId.Equals(notificationSettings.UserId));
                if (userNotificationSettings == null)
                {
                    userNotificationSettings = new NotificationSetting
                    {
                        UserId = (long)notificationSettings.UserId,
                        CreatedAt = DateTime.Now
                    };
                }
                userNotificationSettings.RecommendedMissions = notificationSettings.RecommendedMissions;
                userNotificationSettings.VolunteeringHours = notificationSettings.VolunteeringHours;
                userNotificationSettings.VolunteeringGoals = notificationSettings.VolunteeringGoals;
                userNotificationSettings.UserComments = notificationSettings.UserComments;
                userNotificationSettings.UserStories  = notificationSettings.UserStories;
                userNotificationSettings.NewMissions = notificationSettings.NewMissions;
                userNotificationSettings.NewMessages = notificationSettings.NewMessages;
                userNotificationSettings.RecommendedStory= notificationSettings.RecommendedStory;
                userNotificationSettings.NotificationByEmail = notificationSettings.NotificationByEmail;
                userNotificationSettings.UpdatedAt = DateTime.Now;
                _db.NotificationSettings.Update(userNotificationSettings);
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }

        public NotificationSetting GetNotificationSetting(long userId)
        {
            var userNotificationSetting = _db.NotificationSettings.FirstOrDefault(ns=>ns.UserId.Equals(userId));
            return userNotificationSetting;
        }

        
        #endregion


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
