using CommunityInvestment.Application.Utilities;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CommunityInvestment.Application.Common;
using NuGet.Protocol;
using Microsoft.VisualBasic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CummunityInvestmentWeb.Areas.Users.Controllers
{
    [Authorize(Roles = "User")]
    public class UserActivityController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IWebHostEnvironment webHostEnvironment;
        public UserActivityController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        //--- User Profile---//
        //[Authorize]
        public IActionResult UserProfile()
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            UserProfileVM userProfile = new UserProfileVM();
            if (userId != 0)
            {
                ViewBag.Countries = _unitOfWork.Filter.GetAllCountry();
                userProfile = _unitOfWork.User.GetUserProfileById(userId);
                ViewBag.CountryCities = _unitOfWork.Filter.GetCitiesByCountryId(userProfile.CountryId);
                ViewBag.Skills = _unitOfWork.Filter.GetAllSkills();
                return View(userProfile);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UserProfileAsync([FromForm] UserProfileVM userProfileData)
        {
            if (ModelState.IsValid)
            {
                long userId = GeneralUtility.GetClaimIdentifier(User);
                userProfileData.UserId = userId;
                if (userId != 0)
                {
                    string oldUserAvatarPath = userProfileData.Avatar;
                    if (userProfileData.NewUserAvatar != null)
                    {
                        string path = "/data/Images/UserAvatar/";
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "UserAvatar");
                        string uploadedAvatarPath = GeneralUtility.UploadFile(uploadsFolder, userProfileData.NewUserAvatar, path);
                        userProfileData.Avatar = uploadedAvatarPath;
                    };
                    User updatedUser = await _unitOfWork.User.UpdateUserProfileAsync(userProfileData);
                    if (updatedUser != null)
                    {
                        _unitOfWork.Save();
                        if (userProfileData.NewUserAvatar != null && !string.IsNullOrEmpty(oldUserAvatarPath))
                        {
                            FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, oldUserAvatarPath);
                            if (storyImageInfo.Exists)
                                storyImageInfo.Delete();
                        }

                        // Create a new identity with the updated claims
                        var newIdentity = (ClaimsIdentity)User.Identity;
                        newIdentity.RemoveClaim(User.FindFirst(ClaimTypes.Name)); // Remove the current Name claim
                        newIdentity.AddClaim(new Claim(ClaimTypes.Name, updatedUser.FirstName + " " + updatedUser.LastName)); // Add the new Name claim
                        newIdentity.RemoveClaim(User.FindFirst("avatar_url"));
                        newIdentity.AddClaim(new Claim("avatar_url", updatedUser.Avatar ?? "/assets/user-profile-avatar.svg"));
                        newIdentity.RemoveClaim(User.FindFirst("countryId"));
                        newIdentity.AddClaim(new Claim("countryId", updatedUser.CountryId.ToString()));
                        // Create a new principal with the updated identity
                        var newPrincipal = new ClaimsPrincipal(newIdentity);
                        // Update the current user session with the new principal
                        HttpContext.User = newPrincipal;
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal);

                        TempData["SuccessMessage"] = UserMessages.UserProfileUpdated_Success;
                        TempData["SuccessMessageTitle"] = UserMessages.UserProfileUpdated_SuccessTitle;
                        return RedirectToAction("UserProfile", "UserActivity");
                    }
                }
            }
            return View(userProfileData);
        }

        public IActionResult GetCitiesByCountryId(long countryId)
        {
            var Cities = _unitOfWork.Filter.GetCitiesByCountryId(countryId);
            return Json(Cities);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM changePassword)
        {
            if (changePassword.NewPassword.Equals(changePassword.OldPassword))
                return ValidationProblem(UserMessages.OldAndNewPasswordsMatch_ErrorMessage);
            if (!changePassword.NewPassword.Equals(changePassword.ConfirmPassword))
                return ValidationProblem(UserMessages.NewAndConfirmPasswordsNotMatch_ErrorMessage);
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if (userId != 0)
            {
                bool IsOldPasswordValid = _unitOfWork.User.CheckOldPassword(userId, changePassword.OldPassword);
                if (!IsOldPasswordValid)
                    return ValidationProblem(UserMessages.InvalidOldPassword_ErrorMessage);
                bool IsPasswordUpdated = _unitOfWork.User.UpdatePassword(userId, changePassword.NewPassword);
                if (IsPasswordUpdated)
                {
                    _unitOfWork.Save();
                    return Ok(new ToastrNotificationVM
                    {
                        Title = UserMessages.PasswordUpdated_SuccessTitle,
                        Message = UserMessages.PasswordUpdated_SuccessMessage
                    });
                }
            }
            return BadRequest("401");
        }

        //--- Volunteering Timesheet ---//
        public IActionResult VolunteeringTimesheet()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetUserTimesheets()
        {
            TimesheetFilterVM timesheetFilter = new TimesheetFilterVM
            {
                startPage = Convert.ToInt32(Request.Form["start"]),
                pageLength = Convert.ToInt32(Request.Form["length"]),
                SearchString = Request.Form["search[value]"],
                SortBy = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"],
                SortOrder = Request.Form["order[0][dir]"],
                MissionType = Request.Form["MissionType"]
            };
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if (userId != 0)
            {
                DataTableVM<TimesheetVM> userTimeMissionTimesheets = _unitOfWork.User.GetUserTimesheets(userId, timesheetFilter);
                var result = Json(userTimeMissionTimesheets);
                return result;
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult AddOrUpdateTimesheetEntry(TimesheetVM userTimesheetData)
        {
            if (userTimesheetData.MissionType == "goal")
            {
                ModelState.Remove("TimespanHours");
                ModelState.Remove("TimespanMinutes");
            }
            else if (userTimesheetData.MissionType == "time")
            {
                ModelState.Remove("Action");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if (userId != 0)
            {
                userTimesheetData.UserId = userId;
                bool IsTimesheetAddedOrUpdated = _unitOfWork.User.AddOrUpdateTimesheet(userTimesheetData);
                if (IsTimesheetAddedOrUpdated)
                {
                    _unitOfWork.Save();
                    return Ok("Timesheet updated!");
                }
            }
            return BadRequest();
        }

        public IActionResult GetTimesheetModalForm(string missionType, long timesheetId)
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            if (userId != 0)
            {
                ViewBag.TimesheetMissions = _unitOfWork.User.GetUserVolunteeredMAForTimesheet(userId, missionType);
                TimesheetVM timesheetVM = new TimesheetVM();
                timesheetVM.MissionType = missionType;
                if (timesheetId != 0)
                {
                    timesheetVM = _unitOfWork.User.GetTimesheetVM(timesheetId);
                }
                return PartialView("~/Areas/Users/Views/Shared/_TimesheetFormModal.cshtml", timesheetVM);
            }
            return BadRequest();
        }

        public IActionResult RemoveUserTimesheetEntry(long timesheetId)
        {
            bool IsTimesheetEntryRemoved = _unitOfWork.User.RemoveUserTimesheetEntry(timesheetId);
            if (IsTimesheetEntryRemoved)
            {
                _unitOfWork.Save();
                return Ok("Request received!");
            }
            return BadRequest();
        }
    }
}
