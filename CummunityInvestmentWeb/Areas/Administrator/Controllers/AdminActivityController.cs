using Microsoft.AspNetCore.Mvc;
using CommunityInvestment.Models;
using CommunityInvestment.DataAccess.Repository.IRepository;
using System.Data;
using CommunityInvestment.Models.ViewModels;
using CommunityInvestment.Application.Common;
using CommunityInvestment.Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CummunityInvestmentWeb.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminActivityController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IWebHostEnvironment webHostEnvironment;
        public AdminActivityController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        //--- Admin Profile and Change password
        [HttpPost]
        public IActionResult SaveProfile(AdminProfileVM adminProfileData)
        {
            long adminId = GeneralUtility.GetClaimIdentifier(User);
            if (adminId == 0)
                return BadRequest();
            ModelState.Remove("Password");
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Uploading New Admin Avatar to Server
            string oldAdminAvatarPath = adminProfileData.AdminAvatar;
            if (adminProfileData.NewAdminAvatar != null)
            {
                string path = "/data/Images/AdminAvatar/";
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "AdminAvatar");
                string uploadedAvatarPath = GeneralUtility.UploadFile(uploadsFolder, adminProfileData.NewAdminAvatar, path);
                adminProfileData.AdminAvatar = uploadedAvatarPath;
            };
            // Add or Update User
            adminProfileData.AdminId = adminId;
            Admin updatedAdmin = _unitOfWork.Admin.AddOrUpdateAdmin(adminProfileData);
            if (updatedAdmin != null)
            {
                _unitOfWork.Save();
                // Delete old user avatar if exists
                if (adminProfileData.NewAdminAvatar != null && !string.IsNullOrEmpty(oldAdminAvatarPath))
                {
                    FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, oldAdminAvatarPath);
                    if (storyImageInfo.Exists)
                        storyImageInfo.Delete();
                }
                // Create a new identity with the updated claims
                var newIdentity = (ClaimsIdentity)User.Identity;
                newIdentity.RemoveClaim(User.FindFirst(ClaimTypes.Name)); // Remove the current Name claim
                newIdentity.AddClaim(new Claim(ClaimTypes.Name, updatedAdmin.FirstName + " " + updatedAdmin.LastName)); // Add the new Name claim
                newIdentity.RemoveClaim(User.FindFirst("FirstName"));
                newIdentity.AddClaim(new Claim("FirstName", updatedAdmin.FirstName ?? ""));
                newIdentity.RemoveClaim(User.FindFirst("LastName"));
                newIdentity.AddClaim(new Claim("LastName", updatedAdmin.LastName ?? ""));
                newIdentity.RemoveClaim(User.FindFirst("avatar_url"));
                newIdentity.AddClaim(new Claim("avatar_url", updatedAdmin.Avatar ?? "/assets/user-profile-avatar.svg"));
                newIdentity.RemoveClaim(User.FindFirst("avatar_url"));
                newIdentity.AddClaim(new Claim("avatar_url", updatedAdmin.Avatar ?? "/assets/user-profile-avatar.svg"));
                // Create a new principal with the updated identity
                var newPrincipal = new ClaimsPrincipal(newIdentity);
                // Update the current user session with the new principal
                HttpContext.User = newPrincipal;
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal);

                string returnMessage = "Profile Saved!";
                return Ok(returnMessage);
            }
            return BadRequest("Something went wrong!");

        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM changePassword)
        {
            if (changePassword.NewPassword.Equals(changePassword.OldPassword))
                return ValidationProblem("New and Old password Same");
            if (!changePassword.NewPassword.Equals(changePassword.ConfirmPassword))
                return ValidationProblem("NewPassword and ConfirmPassword not Matched!");
            long adminId = GeneralUtility.GetClaimIdentifier(User);
            if (adminId != 0)
            {
                bool IsOldPasswordValid = _unitOfWork.Admin.CheckOldPassword(adminId, changePassword.OldPassword);
                if (!IsOldPasswordValid)
                    return ValidationProblem("Invalid OldPassword");
                bool IsPasswordUpdated = _unitOfWork.Admin.UpdatePassword(adminId, changePassword.NewPassword);
                if (IsPasswordUpdated)
                {
                    _unitOfWork.Save();
                    return Ok("Password Updated!");
                }
            }
            return BadRequest("401");
        }


        //--- Manage Users
        public IActionResult ManageUsers()
        {
            ViewBag.Countries = _unitOfWork.Filter.GetAllCountry();
            return View();
        }

        [HttpPost]
        public IActionResult GetUsersDataTable()
        {
            DataTableFilterVM usersDTFilter = new DataTableFilterVM(Request);
            DataTableVM<User> userDataTable = _unitOfWork.Admin.GetUsersDT(usersDTFilter);
            return Json(userDataTable);
        }

        public IActionResult GetUserEdit(long? userId)
        {
            ViewBag.Countries = _unitOfWork.Filter.GetAllCountry();
            AdminUserManageVM userProfileModal = new AdminUserManageVM();
            if (userId != null)
            {
                userProfileModal = _unitOfWork.Admin.GetUserEdit((long)userId);
                ViewBag.CountryCities = _unitOfWork.Filter.GetCitiesByCountryId(userProfileModal.CountryId);
            }
            return PartialView("~/Areas/Administrator/Views/Shared/_AddOrUpdateUserModal.cshtml", userProfileModal);
        }

        [HttpPost]
        public IActionResult AddOrUpdateUser([FromForm] AdminUserManageVM adminUserManageObj)
        {
            if (adminUserManageObj.UserId == null)
            {
                User user = _unitOfWork.User.GetByEmail(adminUserManageObj.Email);
                if (user != null)
                    ModelState.AddModelError("Email", UserMessages.EmailRegistered);
            }
            if (adminUserManageObj.UserId != null)
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Uploading New User Avatar to Server
            string oldUserAvatarPath = adminUserManageObj.Avatar;
            if (adminUserManageObj.NewUserAvatar != null)
            {
                string path = "/data/Images/UserAvatar/";
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "UserAvatar");
                string uploadedAvatarPath = GeneralUtility.UploadFile(uploadsFolder, adminUserManageObj.NewUserAvatar, path);
                adminUserManageObj.Avatar = uploadedAvatarPath;
            };
            // Add or Update User
            string IsUserAddedOrUpdated = _unitOfWork.Admin.AddOrUpdateUser(adminUserManageObj);
            if (IsUserAddedOrUpdated != null)
            {
                _unitOfWork.Save();
                // Delete old user avatar if exists
                if (adminUserManageObj.NewUserAvatar != null && !string.IsNullOrEmpty(oldUserAvatarPath))
                {
                    FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, oldUserAvatarPath);
                    if (storyImageInfo.Exists)
                        storyImageInfo.Delete();
                }
                string returnMessage = IsUserAddedOrUpdated;
                return Ok(returnMessage);
            }
            return BadRequest("Something went wrong!");
        }

        public IActionResult RemoveUser(long userId)
        {
            bool IsUserRemoved = _unitOfWork.Admin.RemoveUser(userId);
            if (IsUserRemoved)
            {
                _unitOfWork.Save();
                return Ok("User Removed!");
            }
            return BadRequest();
        }


        //--- Manage CMS Pages
        public IActionResult ManageCMSPages()
        {
            return View();
        }
        public IActionResult GetCMSDataTable()
        {
            DataTableFilterVM cmsDTFilter = new DataTableFilterVM(Request);
            DataTableVM<CmsPage> cmsDataTable = _unitOfWork.Admin.GetCMS_DT(cmsDTFilter);
            return Json(cmsDataTable);
        }

        public IActionResult RemoveCMSPage(long cmsPageId)
        {
            bool IsCMSPageRemoved = _unitOfWork.Admin.RemoveCMSPage(cmsPageId);
            if (IsCMSPageRemoved)
            {
                _unitOfWork.Save();
                return Ok("CMS Page Removed Successfully!");
            }
            return BadRequest();
        }

        public IActionResult AddCMSPage()
        {
            return View("~/Areas/Administrator/Views/AdminActivity/CMSPage.cshtml");
        }

        [Route("Administrator/AdminActivity/EditCMSPage/{cmsPageId}")]
        public IActionResult EditCMSPage(long cmsPageId)
        {
            AdminCmsPageVM cmsPage = _unitOfWork.Admin.GetCMSPageById(cmsPageId);
            if (cmsPage != null)
            {
                return View("~/Areas/Administrator/Views/AdminActivity/CMSPage.cshtml", cmsPage);
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult SaveCMSPage(AdminCmsPageVM cmsPageData)
        {
            if (ModelState.IsValid)
            {
                string IsCMSPageSaved = _unitOfWork.Admin.SaveCMSPage(cmsPageData);
                if (!string.IsNullOrEmpty(IsCMSPageSaved))
                {
                    _unitOfWork.Save();
                    TempData["SuccessMessageTitle"] = IsCMSPageSaved;
                    TempData["SuccessMessage"] = "CMS Page Details Saved successfully!";
                    return Redirect("/Administrator/AdminActivity/ManageCMSPages");
                }
                TempData["ErrorMessageTitle"] = ErrorMessages.SomethingWentWrongTitle;
                TempData["ErrorMessage"] = ErrorMessages.SomethingWentWrongMessage;
            }
            return View("~/Areas/Administrator/Views/AdminActivity/CMSPage.cshtml", cmsPageData);
        }


        //-- Manage Banner
        public IActionResult ManageBanners()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetBannerDataTable()
        {
            DataTableFilterVM bannerDTFilter = new DataTableFilterVM(Request);
            DataTableVM<Banner> bannerDataTable = _unitOfWork.Admin.GetBannerDT(bannerDTFilter);
            return Json(bannerDataTable);
        }
        public IActionResult GetBannerEdit(long? bannerId)
        {
            AdminBannerVM bannerVM = new AdminBannerVM();
            if (bannerId != null)
            {
                var banner = _unitOfWork.Admin.GetBannerById((long)bannerId);
                if (banner == null)
                    return NotFound();
                bannerVM = new AdminBannerVM
                {
                    BannerId = banner.BannerId,
                    Title = banner.Title,
                    Text = banner.Text,
                    SortOrder = banner.SortOrder,
                    ImagePath = banner.Image
                };
            }
            return PartialView("~/Areas/Administrator/Views/Shared/_AddOrUpdateBannerModal.cshtml", bannerVM);
        }

        [HttpPost]
        public IActionResult AddOrUpdateBanner(AdminBannerVM bannerData)
        {
            if (bannerData.BannerId == null && bannerData.NewBannerImage == null)
                ModelState.AddModelError("NewBannerImage", "Please Upload Banner Image!");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Uploading New Banner to Server
            string oldBannerImagePath = string.Empty;
            if (bannerData.BannerId != null)
                oldBannerImagePath = bannerData.ImagePath;
            if (bannerData.NewBannerImage != null)
            {
                string path = "/data/Images/Banners/";
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "Banners");
                string uploadedImagePath = GeneralUtility.UploadFile(uploadsFolder, bannerData.NewBannerImage, path);
                bannerData.ImagePath = uploadedImagePath;
            };
            // Add or Update Banner
            string IsBannerAddedOrUpdated = _unitOfWork.Admin.AddOrUpdateBanner(bannerData);
            if (IsBannerAddedOrUpdated != null)
            {
                _unitOfWork.Save();
                // Delete old banner if exists
                if (bannerData.NewBannerImage != null && !string.IsNullOrEmpty(oldBannerImagePath))
                {
                    FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, oldBannerImagePath);
                    if (storyImageInfo.Exists)
                        storyImageInfo.Delete();
                }
                string returnMessage = IsBannerAddedOrUpdated;
                return Ok(returnMessage);
            }
            return BadRequest("Something went wrong!");
        }

        public IActionResult RemoveBanner(long bannerId)
        {
            bool IsBannerRemoved = _unitOfWork.Admin.RemoveBanner(bannerId);
            if (IsBannerRemoved)
            {
                _unitOfWork.Save();
                return Ok("Banner Removed successfully!");
            }
            return BadRequest();
        }
    }
}
