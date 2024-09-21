using CommunityInvestment.DataAccess.Repository;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using Microsoft.EntityFrameworkCore;
using CommunityInvestment.Models.ViewModels;
using CummunityInvestmentWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;
using System.Drawing.Printing;
using CommunityInvestment.Application.Utilities;
using System.Net;
using CommunityInvestment.Application.Common;

namespace CummunityInvestmentWeb.Users.Controllers
{
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync()
        {
            if (User.IsInRole("Admin"))
                return Forbid();
            int pg = 1;
            IEnumerable<Country> Countries = _unitOfWork.Filter.GetAllCountry();
            ViewBag.Countries = Countries;
            IEnumerable<MissionTheme> MissionThemes = _unitOfWork.Filter.GetAllMissionTheme();
            ViewBag.MissionThemes = MissionThemes;
            IEnumerable<Skill> MissionSkills = _unitOfWork.Filter.GetAllSkills();
            ViewBag.MissionSkills = MissionSkills;

            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            const int pageSize = 3;
            PageList<MissionsDetailsVM> MissionsDetails = _unitOfWork.Mission.GetAllMissions(userId, pg, pageSize);
            return View(MissionsDetails);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> FilterMissions(string SearchInput, string[] CountryFilter, string[] CityFilter, string[] MissionThemeFilter, string[] MissionSkillFilter, string MissionSort = "", int pg = 1)
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            const int pageSize = 3;
            var filter = new MissionFilter
            {
                SearchInput = SearchInput,
                Country = CountryFilter,
                City = CityFilter,
                MissionThemes = MissionThemeFilter,
                MissionSkills = MissionSkillFilter
            };
            PageList<MissionsDetailsVM> MissionsDetails = _unitOfWork.Mission.GetAllMissions(userId, pg, pageSize, filter, MissionSort);
            return PartialView("Index", MissionsDetails);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GetCitiesByCountry(string[] CountryFilter)
        {
            IEnumerable<City> Cities = _unitOfWork.Filter.GetCitiesByCountry(CountryFilter);
            return PartialView("../Shared/_ViewList", Cities);
        }

        [HttpPost]
        //[Authorize]
        public IActionResult MissionFavorite(long MissionId, long UserId)
        {
            var missionFavoriteResult = _unitOfWork.Mission.SetFavoriteMission(MissionId, UserId);
            _unitOfWork.Save();
            return Ok(missionFavoriteResult);
        }


        [Route("Users/Home/VolunteeringMission/{missionId}")]
        public async Task<IActionResult> VolunteeringMission(long missionId)
        {
            int recentVolunteerPg = 1;
            int pageSize = 9;
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var MissionDetails = _unitOfWork.Mission.GetMissionById(missionId, userId);
            int missionDetailsCount = MissionDetails.MissionApplications.Count();
            // Pagination
            if (recentVolunteerPg < 1)
                recentVolunteerPg = 1;
            int totalPages = Pager.getTotalPages(MissionDetails.MissionApplications.Count(), pageSize);
            if (recentVolunteerPg > totalPages)
                recentVolunteerPg = totalPages;
            int recSkip = (recentVolunteerPg - 1) * pageSize;
            var pager = new Pager(missionDetailsCount, recentVolunteerPg, pageSize);
            // Setting Start and End item for current page
            pager.ItemFrom = recSkip + 1;
            if (pager.ItemFrom < 0)
                pager.ItemFrom = 0;
            if (pager.CurrentPage >= totalPages)
                pager.ItemTo = missionDetailsCount;
            else
                pager.ItemTo = recSkip + pageSize;
            // Recent Volunteers on Current page
            MissionDetails.MissionApplications = MissionDetails.MissionApplications.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;
            return View("~/Areas/Users/Views/Home/VolunteeringMission.cshtml", MissionDetails);
        }

        [HttpPost]
        public IActionResult GetRecentVolunteers(long missionId, int recentVolunteersPage = 1)
        {
            int pageSize = 9;
            var MissionApplications = _unitOfWork.Mission.GetMissionApplications(missionId);
            int missionApplicationCounts = MissionApplications.Count();
            // Pagination
            if (recentVolunteersPage < 1)
                recentVolunteersPage = 1;
            int totalPages = Pager.getTotalPages(MissionApplications.Count(), pageSize);
            if (recentVolunteersPage > totalPages)
                recentVolunteersPage = totalPages;
            int recSkip = (recentVolunteersPage - 1) * pageSize;
            var pager = new Pager(missionApplicationCounts, recentVolunteersPage, pageSize);
            // Setting Start and End item for current page
            pager.ItemFrom = recSkip + 1;
            if (pager.ItemFrom < 0)
                pager.ItemFrom = 0;
            if (pager.CurrentPage >= totalPages)
                pager.ItemTo = missionApplicationCounts;
            else
                pager.ItemTo = recSkip + pageSize;
            // Recent Volunteers on Current page
            MissionApplications = MissionApplications.Skip(recSkip).Take(pager.PageSize).ToList();
            MissionRecentVolunteersVM missionRecentVolunteersVM = new MissionRecentVolunteersVM
            {
                MissionApplications = MissionApplications,
                Pager = pager
            };

            //PageList<MissionApplication> pageListVM = new PageList<MissionApplication>(recentVolunteersPage, 3, MissionApplications);
            return PartialView("../Shared/_MissionRecentVolunteers", missionRecentVolunteersVM);
        }

        [HttpPost]
        [Route("Users/VolunteeringMission/PostComment")]
        public IActionResult PostComment(long MissionId, string UserId, string CommentText)
        {
            long _UserId = Convert.ToInt64(UserId);
            _unitOfWork.Mission.PostComment(MissionId, _UserId, CommentText);
            _unitOfWork.Save();
            List<Comment> comments = _unitOfWork.Mission.GetMissionComments(MissionId);
            return PartialView("~/Areas/Users/Views/Shared/_VolunteeringMissionComments.cshtml", comments);
        }

        [HttpPost]
        [Route("Users/VolunteeringMission/UserRate")]
        //[Authorize]
        public IActionResult UserRate(long missionId, string userId, string rating)
        {
            long _UserId = Convert.ToInt64(userId);
            _unitOfWork.Mission.SetUserRatings(missionId, _UserId, rating);
            _unitOfWork.Save();
            return Ok(new ToastrNotificationVM
            {
                Title = UserMessages.MissionRatings_SuccessTitle,
                Message = UserMessages.MissionRatings_SuccessMessage
            });
        }

        [Route("Users/VolunteeringMission/GetCoWorkers")]
        public IActionResult GetCoWorkers(string userId, string searchFilter)
        {
            long _UserId = Convert.ToInt64(userId);
            List<User> coWorkers = _unitOfWork.User.GetCoWorkers(_UserId, searchFilter);
            return PartialView("~/Areas/Users/Views/Shared/_RecommendToCoWorkers.cshtml", coWorkers);
        }

        [HttpPost]
        [Route("Users/VolunteeringMission/RecommendToCoWorker")]
        //[Authorize]
        public IActionResult RecommendToCoWorker(long missionId, long userId, long coWorkerId)
        {
            _unitOfWork.Mission.RecommendToCoWorker(missionId, userId, coWorkerId);
            _unitOfWork.Save();
            return Ok(new ToastrNotificationVM
            {
                Title = UserMessages.MissionRecommendation_SuccessTitle,
                Message = UserMessages.MissionRecommendation_SuccessMessage
            });
        }

        [HttpPost]
        [Route("Users/VolunteeringMission/Apply")]
        //[Authorize]
        public IActionResult ApplyMission(long MissionId)
        {
            long UserId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            string StatusMA = _unitOfWork.Mission.ApplyMission(UserId, MissionId);
            _unitOfWork.Save();
            return Ok(StatusMA);
        }

        public IActionResult Policy()
        {
            List<CmsPage> CMSPolicyPages = _unitOfWork.Admin.GetAllCMSPolicies("1");
            CMSPolicyPages.ForEach(contentPolicyPage => contentPolicyPage.Description = WebUtility.HtmlDecode(contentPolicyPage.Description));
            return View(CMSPolicyPages);
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUsVM contactUsData)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if (userId != 0)
            {
                contactUsData.UserId = userId;
                bool IsMessageSent = _unitOfWork.User.SendQueryMessage(contactUsData);
                if (IsMessageSent)
                {
                    _unitOfWork.Save();
                    return Ok("Message received!");
                }
            }
            return BadRequest();
        }

        #region User Notifications
        /// <summary>
        /// This method is used to get user notifications.
        /// </summary>
        /// <returns></returns>
        [Route("Users/Notification/GetNotifications")]
        public IActionResult GetUserNotifications()
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            List<UserNotification> userNotificationList = _unitOfWork.User.GetUserNotifications(userId);
            return Json(userNotificationList);
        }

        /// <summary>
        /// Controller method responsible for mark as read notification.
        /// </summary>
        /// <param name="userNotificationId">User Notification Identifier</param>
        /// <returns></returns>
        [Route("Users/Notification/MarkAsRead")]
        public IActionResult NotificationMarkAsRead(long userNotificationId)
        {
            bool isRead = _unitOfWork.User.NotificationMarkAsRead(userNotificationId);
            if(isRead)
            {
                _unitOfWork.Save();
                return Ok(isRead);
            }
            return BadRequest();
        }

        /// <summary>
        /// This Controller method responsible for clear all the notifications.
        /// </summary>
        /// <returns></returns>
        [Route("Users/Notification/ClearAllNotifications")]
        public IActionResult ClearAllNotifications()
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if(userId != 0)
            {
                bool isNotificationsRemoved = _unitOfWork.User.ClearAllNotifications(userId);
                if (isNotificationsRemoved)
                {
                    _unitOfWork.Save();
                    List<UserNotification> userNotificationList = _unitOfWork.User.GetUserNotifications(userId);
                    return Json(userNotificationList);
                }
            }
            return BadRequest();
        }

        [Route("Users/Notification/SaveNotificationSettings")]
        public IActionResult SaveNotificationSettings(NotificationSettingVM notificationSettings)
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if(userId != 0)
            {
                notificationSettings.UserId = userId;
                bool isNotificationSettingsSaved = _unitOfWork.User.SaveNotificationSettings(notificationSettings);
                if (isNotificationSettingsSaved)
                {
                    _unitOfWork.Save();
                    return Ok("Notification settings saved!");
                }
            }
            return BadRequest();
        }

        [Route("Users/Notification/GetUserNotificationSetting")]
        public IActionResult GetUserNotificationSetting()
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if(userId != 0)
            {
                NotificationSetting userNotificationSetting = _unitOfWork.User.GetNotificationSetting(userId);
                if (userNotificationSetting != null)
                {
                    return Json(userNotificationSetting);
                }
            }
            return BadRequest();
        }
        #endregion
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}