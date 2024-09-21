using CommunityInvestment.Application.Common;
using CommunityInvestment.Application.Services;
using CommunityInvestment.Application.Utilities;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository
{
    public class MissionRepository : Repository<Mission>, IMissionRepository
    {
        private readonly CommunityInvestmentContext _db;
        public readonly IConfiguration _configuration;
        public MissionRepository(CommunityInvestmentContext db, IConfiguration configuration) : base(db)
        {
            _db = db;
            _configuration = configuration;
        }

        public Country GetCountryById(long countryId)
        {
            return _db.Countries.FirstOrDefault(country => country.CountryId == countryId);
        }
            
        public City GetCityById(long cityId)
        {
            return _db.Cities.FirstOrDefault(city => city.CityId == cityId);
        }

        private IQueryable<MissionsDetailsVM> GetAllMissions(string? userId)
        {
            var Missions = _db.Missions.Include(m => m.Country)
                            .Include(m => m.City)
                            .Include(m => m.Theme)
                            .Include(m => m.MissionSkills)
                            .Include(m => m.MissionMedia.Where(mm=>mm.DeletedAt == null))
                            .Where(m => m.DeletedAt == null);
            var Skills = _db.Skills.Where(m => m.DeletedAt == null);
            //var MissionIds = Missions.Select(m => m.MissionId).ToList();
            var _GoalMissions = _db.GoalMissions;
            var _TimeSheet = _db.Timesheets;
            var _MissionApplications = _db.MissionApplications;
            var _Ratings = _db.MissionRatings;
            var _FavoriteMissions = _db.FavoriteMissions;
            IQueryable<MissionsDetailsVM> MissionsDetailsQuery = Missions
                            .Select(m => new MissionsDetailsVM
                            {
                                MissionId = m.MissionId,
                                CityId = m.CityId,
                                CityName = m.City.Name,
                                CountryId = m.CountryId,
                                CountryName = m.Country.Name,
                                ThemeId = m.ThemeId,
                                ThemeTitle = m.Theme.Title,
                                Title = m.Title,
                                ShortDescription = m.ShortDescription,
                                Description = m.Description,
                                StartDate = m.StartDate,
                                EndDate = m.EndDate,
                                RegistrationDeadline = m.RegistrationDeadline,
                                MissionType = m.MissionType,
                                Status = m.Status,
                                OrganizationName = m.OrganizationName,
                                GoalObjectiveText = _GoalMissions.FirstOrDefault(gm => gm.MissionId == m.MissionId).GoalObjectiveText,
                                UserApplied = (userId != null) ? _MissionApplications.Any(ma => ma.MissionId == m.MissionId && ma.UserId.ToString() == userId && ma.ApprovalStatus != Status.Rejected) : false,
                                IsFavorite = (userId != null) ? _FavoriteMissions.Any(fm => fm.MissionId == m.MissionId && fm.UserId.ToString() == userId && fm.DeletedAt == null) : false,
                                TotalVolunteers = _MissionApplications.Count(ma => ma.MissionId == m.MissionId && ma.ApprovalStatus != Status.Rejected),
                                GoalValue = _GoalMissions.FirstOrDefault(gm => gm.MissionId == m.MissionId && gm.DeletedAt == null).GoalValue,
                                GoalAchieved = _TimeSheet.Where(ts => ts.MissionId == m.MissionId && ts.DeletedAt == null).Sum(ts => ts.Action),
                                Ratings = GetRatings(_Ratings.ToList(), m.MissionId),
                                CreatedAt = m.CreatedAt,
                                UpdatedAt = m.UpdatedAt,

                                Skills = m.MissionSkills.Join(Skills, ms => ms.SkillId, s => s.SkillId, (ms, s) => ms).ToList(),
                                //MissionCardMedia = m.MissionMedia.Any(mm => mm.Default == "1") ? m.MissionMedia.FirstOrDefault(mm => mm.Default == "1") : m.MissionMedia.FirstOrDefault()
                                MissionCardMedia = m.MissionMedia.FirstOrDefault(mm => mm.Default == "1") ?? m.MissionMedia.Where(mm => mm.DeletedAt == null).FirstOrDefault()
                            });
            return MissionsDetailsQuery;
        }

        public PageList<MissionsDetailsVM> GetAllMissions(string? userId, int pageNo, int pageSize, MissionFilter missionFilter = null, string MissionSort = null)
        {
            try
            {
                IQueryable<MissionsDetailsVM> MissionsDetailsQuery = GetAllMissions(userId);
                if (missionFilter != null)
                {
                    if (!string.IsNullOrEmpty(missionFilter.SearchInput))
                    {
                        MissionsDetailsQuery = MissionsDetailsQuery.Where(m =>   m.Title.Contains(missionFilter.SearchInput));
                    }
                    if (missionFilter.City != null && missionFilter.City.Length > 0)
                    {
                        MissionsDetailsQuery = MissionsDetailsQuery.Where(m => missionFilter.City.Contains(m.CityName));
                    }
                    if (missionFilter.Country != null && missionFilter.Country.Length > 0)
                    {
                        MissionsDetailsQuery = MissionsDetailsQuery.Where(m => missionFilter.Country.Contains(m.CountryName));
                    }
                    if (missionFilter.MissionThemes != null && missionFilter.MissionThemes.Length > 0)
                    {
                        MissionsDetailsQuery = MissionsDetailsQuery.Where(m => missionFilter.MissionThemes.Contains(m.ThemeTitle));
                    }
                    if (missionFilter.MissionSkills != null && missionFilter.MissionSkills.Length > 0)
                    {
                        MissionsDetailsQuery = MissionsDetailsQuery.Where(m => m.Skills.Any(s => missionFilter.MissionSkills.Contains(s.Skill.SkillName)));
                    }
                }
                if (!String.IsNullOrEmpty(MissionSort))
                {
                    switch (MissionSort)
                    {
                        case "Newest":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderByDescending(m => m.CreatedAt);
                            break;
                        case "Oldest":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderBy(m => m.CreatedAt);
                            break;
                        case "LowestSeats":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderBy(m => m.MissionType == "time" ? 0 : 1).ThenBy(m=> m.MissionType == "time" ? m.GoalValue- m.TotalVolunteers : m.GoalAchieved);
                            break;
                        case "HighestSeats":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderBy(m => m.MissionType == "time" ? 0 : 1).ThenByDescending(m => m.MissionType == "time" ? m.GoalValue - m.TotalVolunteers : m.GoalAchieved);
                            break;
                        case "MyFavourites":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderByDescending(m => m.IsFavorite);
                            break;
                        case "RegistrationDeadline":
                            MissionsDetailsQuery = MissionsDetailsQuery.OrderBy(m => m.RegistrationDeadline);
                            break;
                        default:
                            break;
                    }
                }
                PageList<MissionsDetailsVM> missionsDetailsPage = new PageList<MissionsDetailsVM>(pageNo, pageSize, MissionsDetailsQuery);
                return missionsDetailsPage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static float GetRatings(List<MissionRating> _Ratings, long _missionId)
        {
            var ratings = _Ratings
                            .Where(mr => mr.MissionId == _missionId)
                            .GroupBy(mr => mr.Rating)
                            .Select(g => new { rating = g.Key, count = g.Count() })
                            .ToList()
                            .Aggregate(0f, (avg, r) => avg + (r.count * int.Parse(r.rating)), avg => (float)avg / _Ratings.Count(r => r.MissionId == _missionId));
            return ratings;
        }

        public MissionDetailsVM GetMissionById(long missionId, string userId)
        {
            var missionComments = _db.Comments.Include(m => m.User).Where(mc => mc.ApprovalStatus == "published").ToList();
            var Users = _db.Users;
            var Skills = _db.Skills;
            var mission = _db.Missions
                .Include(m => m.City)
                .Include(m => m.Country)
                .Include(m => m.Theme)
                .Include(m => m.Comments)
                .Include(m => m.FavoriteMissions)
                .Include(m => m.GoalMissions)
                .Include(m => m.MissionApplications)
                .Include(m => m.MissionDocuments.Where(md=>md.DeletedAt == null))
                .Include(m => m.MissionInvites)
                .Include(m => m.MissionMedia.Where(mm=>mm.DeletedAt == null))
                .Include(m => m.MissionRatings)
                .Include(m => m.MissionSkills)
                //.Include(m => m.Stories)
                .Include(m => m.Timesheets)
                .FirstOrDefault(m => m.MissionId == missionId);
            var missionDetails = new MissionDetailsVM
            {
                MissionId = mission.MissionId,
                CityId = mission.CityId,
                CountryId = mission.CountryId,
                ThemeId = mission.ThemeId,
                Title = mission.Title,
                ShortDescription = mission.ShortDescription,
                Description = WebUtility.HtmlDecode(mission.Description),
                StartDate = mission.StartDate,
                EndDate = mission.EndDate,
                RegistrationDeadline = mission.RegistrationDeadline,
                MissionType = mission.MissionType,
                Status = mission.Status,
                OrganizationName = mission.OrganizationName,
                OrganizationDetail = WebUtility.HtmlDecode(mission.OrganizationDetail),
                Availability = mission.Availability,
                TotalVolunteers = mission.MissionApplications.Count(ma => ma.MissionId == mission.MissionId && ma.ApprovalStatus != Status.Rejected),
                UserApplied = (userId != null) ? mission.MissionApplications.Any(ma => ma.MissionId == missionId && ma.UserId.ToString() == userId && ma.ApprovalStatus != Status.Rejected) : false,
                GoalAchieved = mission.Timesheets.Where(ts => ts.MissionId == mission.MissionId).Sum(ts => ts.Action),
                IsFavorite = (userId != null) ? mission.FavoriteMissions.Any(fm => fm.MissionId == mission.MissionId && fm.UserId.ToString() == userId && fm.DeletedAt == null) : false,
                Ratings = GetRatings(mission.MissionRatings.ToList(), missionId),
                UserRating = (userId != null) ? mission.MissionRatings.FirstOrDefault(mr => mr.MissionId == missionId && mr.UserId.ToString() == userId) : null,
                RatedBy = mission.MissionRatings.Count(),
                CreatedAt = mission.CreatedAt,
                UpdatedAt = mission.UpdatedAt,
                DeletedAt = mission.DeletedAt,

                City = mission.City,
                Country = mission.Country,
                Theme = mission.Theme,
                GoalMission = mission.GoalMissions.FirstOrDefault(),
                //Comments = mission.Comments.ToList(),
                Comments = missionComments.Where(mc => mc.MissionId == mission.MissionId).OrderByDescending(c => c.CreatedAt).ToList(),
                MissionApplications = mission.MissionApplications.Where(ma => ma.ApprovalStatus == Status.Approved).Join(Users, ma => ma.UserId, u => u.UserId, (ma, u) => ma).ToList(),
                MissionDocuments = mission.MissionDocuments.ToList(),
                MissionInvites = mission.MissionInvites.ToList(),
                MissionMedia = mission.MissionMedia.ToList(),
                MissionSkills = mission.MissionSkills.Join(Skills, ms => ms.SkillId, s => s.SkillId, (ms, s) => ms).ToList(),
                Timesheets = mission.Timesheets.ToList(),

                //RelatedMissions = GetAllMissions(userId, new MissionFilter { MissionThemes = new string[] { mission.Theme.Title } }).Where(m => m.MissionId != missionId).Take(3).ToList(),
                RelatedMissions = GetRelatedMissions(userId, mission.MissionId, mission.CityId, mission.CountryId, mission.ThemeId),
                CoWorkers = Users.Where(u => u.UserId.ToString() != userId && u.Status == "1").ToList()
            };
            return missionDetails;
        }

        private List<MissionsDetailsVM> GetRelatedMissions(string? userId, long missionId, long cityId, long countryId, long themeId)
        {
            IQueryable<MissionsDetailsVM> MissionsDetailsQuery = GetAllMissions(userId);
            MissionsDetailsQuery = MissionsDetailsQuery.Where(m => m.MissionId != missionId);
            var relatedMissionsList = MissionsDetailsQuery.Where(m => m.CityId.Equals(cityId)).ToList();
            if (relatedMissionsList.Count < 3)
            {
                relatedMissionsList.AddRange(MissionsDetailsQuery.Where(m => m.CountryId.Equals(countryId) && !m.CityId.Equals(cityId)).Take(3));
                if (relatedMissionsList.Count < 3)
                    relatedMissionsList.AddRange(MissionsDetailsQuery.Where(m => m.ThemeId.Equals(themeId) && !m.CityId.Equals(cityId) && !m.CountryId.Equals(countryId)).Take(3));
            }
            return relatedMissionsList.Take(3).ToList();
        }

        public List<MissionApplication> GetMissionApplications(long missionId)
        {
            List<MissionApplication> MissionApplications = _db.MissionApplications.Where(ma => ma.MissionId == missionId && ma.ApprovalStatus == Status.Approved).ToList();
            var Users = _db.Users;
            List<MissionApplication> result = MissionApplications.Join(Users, ma => ma.UserId, u => u.UserId, (ma, u) => ma).ToList();
            return result;
        }

        public bool SetFavoriteMission(long MissionId, long UserId)
        {
            bool result;
            var _FavoriteMissions = _db.FavoriteMissions;
            var IsFavoriteAlready = _FavoriteMissions.FirstOrDefault(fm => fm.MissionId == MissionId && fm.UserId == UserId);
            if (IsFavoriteAlready != null && IsFavoriteAlready.DeletedAt == null)
            {
                IsFavoriteAlready.DeletedAt = DateTime.Now;
                _db.FavoriteMissions.Update(IsFavoriteAlready);
                result = false;
            }
            else if (IsFavoriteAlready != null && IsFavoriteAlready.DeletedAt != null)
            {
                IsFavoriteAlready.UpdatedAt = DateTime.Now;
                IsFavoriteAlready.DeletedAt = null;
                _db.FavoriteMissions.Update(IsFavoriteAlready);
                result = true;
            }
            else
            {
                _db.FavoriteMissions.Add(new FavoriteMission
                {
                    MissionId = MissionId,
                    UserId = UserId,
                    CreatedAt = DateTime.Now
                });
                result = true;
            }
            _db.SaveChanges();
            return result;
        }

        public void PostComment(long missionId, long userId, string commentText)
        {
            Comment comment = new Comment
            {
                MissionId = missionId,
                UserId = userId,
                CommentText = commentText,
                ApprovalStatus = "published",
                CreatedAt = DateTime.Now
            };
            _db.Comments.Add(comment);
        }

        public List<Comment> GetMissionComments(long missionId)
        {
            List<Comment> comments = _db.Comments.Include(c => c.User)
                .Where(c => c.MissionId == missionId && c.ApprovalStatus == "published")
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return comments;
        }

        public void SetUserRatings(long missionId, long userId, string ratings)
        {
            MissionRating UserMissionRating = _db.MissionRatings.FirstOrDefault(mr => mr.MissionId == missionId && mr.UserId == userId);
            if (UserMissionRating != null)
            {
                UserMissionRating.Rating = ratings;
                UserMissionRating.UpdatedAt = DateTime.Now;
                _db.MissionRatings.Update(UserMissionRating);
            }
            else
            {
                UserMissionRating = new MissionRating
                {
                    MissionId = missionId,
                    UserId = userId,
                    Rating = ratings,
                    CreatedAt = DateTime.Now
                };
                _db.MissionRatings.Add(UserMissionRating);
            }
        }

        public string ApplyMission(long userId, long missionId)
        {
            MissionApplication missionApplication = _db.MissionApplications.FirstOrDefault(ma => ma.MissionId == missionId && ma.UserId == userId);
            if (missionApplication != null)
            {
                switch (missionApplication.ApprovalStatus)
                {
                    case "approved":
                        return "Approved";
                    case "pending":
                        return "Pending";
                    case "rejected":
                        missionApplication.ApprovalStatus = Status.Pending;
                        missionApplication.AppliedAt = DateTime.Now;
                        _db.Update(missionApplication);
                        return "Rejected";
                    default: return "InvalidStatus";
                }
            }
            else
            {
                missionApplication = new MissionApplication
                {
                    MissionId = missionId,
                    UserId = userId,
                    ApprovalStatus = Status.Pending,
                    AppliedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                _db.Add(missionApplication);
                return "AppliedSuccess";
            }
        }

        public void RecommendToCoWorker(long missionId, long userId, long coWorkerId)
        {
            MissionDetailsVM missionDetails = GetMissionById(missionId, null);
            User userDetails = _db.Users.FirstOrDefault(u => u.UserId.Equals(userId));
            User coWorkerDetails = _db.Users.FirstOrDefault(u => u.UserId.Equals(coWorkerId));
            MissionInvite missionInviteObj = new MissionInvite
            {
                MissionId = missionId,
                FromUserId = userId,
                ToUserId = coWorkerId,
                CreatedAt = DateTime.Now
            };

            
            SendMail sendMail = new SendMail(_configuration.GetSection("EmailService:API_Key").Value);
            _db.MissionInvites.Add(missionInviteObj);
            sendMail.RecommendToCoWorker(missionDetails, userDetails, coWorkerDetails);

            //-- Recommendation for mission user notification
            var userNotificationSetting = _db.NotificationSettings.FirstOrDefault(setting => setting.UserId.Equals(userId));
            if (userNotificationSetting != null && userNotificationSetting.RecommendedMissions == 1)
            {
                UserNotificationVM missionRecommendationNotification = new UserNotificationVM
                {
                    UserId = coWorkerId,
                    NotificationFor = Notification.MissionRecommendation,
                    NotificationLink = Notification.GetMissionPath(missionDetails.MissionId),
                    NotificationMessage = Notification.GetNotificationMessage(Notification.MissionRecommendation, missionDetails.Title)
                };
                AddOrUpdateNotification(missionRecommendationNotification);
            }
        }
    }
}
