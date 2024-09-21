using CommunityInvestment.Application.Utilities;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.DependencyResolver;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CommunityInvestment.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace CommunityInvestment.DataAccess.Repository
{
    public class AdminRepository : IAdminRepository
    {
        public readonly CommunityInvestmentContext _db;
        public readonly IConfiguration _configuration;
        public AdminRepository(CommunityInvestmentContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Admin GetAdminById(long adminId)
        {
            return _db.Admins.Find(adminId);
        }

        public Admin GetByEmail(string email)
        {
            return _db.Admins.FirstOrDefault(admin => admin.Email == email);
        }

        public bool Authenticate(string AdminEmail, string AdminPassword)
        {
            Admin adminResult = this.GetByEmail(AdminEmail);
            if ((adminResult != null) && BCrypt.Net.BCrypt.Verify(AdminPassword, adminResult.Password))
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

        public Admin AddOrUpdateAdmin(AdminProfileVM adminProfileDataObj)
        {
            try
            {
                var _Admins = _db.Admins;
                Admin? adminObj;
                string? response;

                if (adminProfileDataObj.AdminId != null)
                {
                    adminObj = _Admins.Find(adminProfileDataObj.AdminId);
                    if (adminObj == null)
                        throw new Exception("Admin not found!");
                    response = "Profile Updated Successfully!";
                }
                else
                {
                    adminObj = new Admin();
                    adminObj.Password = GeneralUtility.GetHashedPassword(adminProfileDataObj.Password);
                    adminObj.CreatedAt = DateTime.Now;
                    response = "New Admin Created Successfully!";
                }
                adminObj.FirstName = adminProfileDataObj.AdminFirstName;
                adminObj.LastName = adminProfileDataObj.AdminLastName;
                adminObj.Avatar = adminProfileDataObj.AdminAvatar;
                adminObj.UpdatedAt = DateTime.Now;
                _db.Admins.Update(adminObj);
                return adminObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //--- Change Password
        public bool CheckOldPassword(long adminId, string oldPassword)
        {
            Admin? admin = GetAdminById(adminId);
            if (admin != null && BCrypt.Net.BCrypt.Verify(oldPassword, admin.Password))
            {
                return true;
            }
            return false;
        }

        public bool UpdatePassword(long adminId, string newPassword)
        {
            Admin? admin = this.GetAdminById(adminId);
            if (admin != null)
            {
                admin.Password = GeneralUtility.GetHashedPassword(newPassword);
                _db.Admins.Update(admin);
                return true;
            }
            else
                throw new ApplicationException("Admin not found!");
        }
        
        public bool UpdatePassword(string adminEmail, string newPassword)
        {
            Admin? admin = this.GetByEmail(adminEmail);
            if (admin != null)
            {
                admin.Password = GeneralUtility.GetHashedPassword(newPassword);
                _db.Admins.Update(admin);
                return true;
            }
            else
                throw new ApplicationException("Admin not found!");
        }

        public List<CmsPage> GetAllCMSPolicies(string statusFilter = "")
        {
            var CMSPolicies = _db.CmsPages.Where(contentPage => contentPage.DeletedAt == null).AsQueryable();
            if (!string.IsNullOrEmpty(statusFilter))
            {
                CMSPolicies = CMSPolicies.Where(contentPage => contentPage.Status.Equals(statusFilter));
            }
            return CMSPolicies.ToList();
        }

        //--- User Manage Page
        public DataTableVM<User> GetUsersDT(DataTableFilterVM usersDTFilter)
        {
            var users = _db.Users.Where(u => u.DeletedAt == null).AsQueryable();
            if (!string.IsNullOrEmpty(usersDTFilter.Search))
            {
                users = users.Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(usersDTFilter.Search.ToLower().Trim()) || u.Email.ToLower().Contains(usersDTFilter.Search.ToLower().Trim()));
            }
            switch (usersDTFilter.SortBy)
            {
                case "department":
                    users = usersDTFilter.SortOrder == "desc" ? users.OrderByDescending(u => u.Department) : users.OrderBy(u => u.Department);
                    break;
                default:
                    users = usersDTFilter.SortOrder == "desc" ? users.OrderByDescending(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt) : users.OrderBy(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt);
                    break;
            }
            int totalRecords = users.Count();
            users = users.Skip(usersDTFilter.PageStart).Take(usersDTFilter.PageLength);
            DataTableVM<User> usersDT = new DataTableVM<User>(users.ToList(), totalRecords);
            return usersDT;
        }

        public AdminUserManageVM GetUserEdit(long userId)
        {
            User user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return null;
            AdminUserManageVM userEdit = new AdminUserManageVM
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                ProfileText = user.ProfileText,
                CountryId = user.CountryId,
                CityId = user.CityId,
                Avatar = user.Avatar,
                Status = user.Status
            };
            return userEdit;
        }

        public string AddOrUpdateUser(AdminUserManageVM adminUserManageObj)
        {
            var _Users = _db.Users;
            User userObj = new User();
            try
            {
                if (adminUserManageObj?.UserId != null && _Users.Any(u => u.UserId == adminUserManageObj.UserId))
                {
                    userObj = _Users.FirstOrDefault(u => u.UserId == adminUserManageObj.UserId);
                    if (userObj == null)
                        return null;
                    userObj.FirstName = adminUserManageObj.FirstName;
                    userObj.LastName = adminUserManageObj.LastName;
                    userObj.Email = adminUserManageObj.Email;
                    userObj.PhoneNumber = (int)adminUserManageObj.PhoneNumber;
                    userObj.EmployeeId = adminUserManageObj.EmployeeId;
                    userObj.Department = adminUserManageObj.Department;
                    userObj.ProfileText = adminUserManageObj.ProfileText;
                    userObj.CountryId = adminUserManageObj.CountryId;
                    userObj.CityId = adminUserManageObj.CityId;
                    userObj.Avatar = adminUserManageObj.Avatar;
                    userObj.UpdatedAt = DateTime.Now;
                    userObj.Status = adminUserManageObj.Status;
                    _db.Users.Update(userObj);
                    return "User Profile Updated Successfully!";
                }
                else
                {
                    userObj = new User
                    {
                        FirstName = adminUserManageObj.FirstName,
                        LastName = adminUserManageObj.LastName,
                        Email = adminUserManageObj.Email,
                        PhoneNumber = (int)adminUserManageObj.PhoneNumber,
                        EmployeeId = adminUserManageObj.EmployeeId,
                        Department = adminUserManageObj.Department,
                        ProfileText = adminUserManageObj.ProfileText,
                        CountryId = adminUserManageObj.CountryId,
                        CityId = adminUserManageObj.CityId,
                        Avatar = adminUserManageObj.Avatar,
                        Password = GeneralUtility.GetHashedPassword(adminUserManageObj.Password),
                        CreatedAt = DateTime.Now,
                        Status = adminUserManageObj.Status,
                    };
                    _db.Users.Add(userObj);
                    return "User Added Successfully!";
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool RemoveUser(long userId)
        {
            User? user = _db.Users.Find(userId);
            if (user != null)
            {
                user.DeletedAt = DateTime.Now;
                _db.Users.Update(user);
                return true;
            }
            return false;
        }

        //---- CMS Page
        public DataTableVM<CmsPage> GetCMS_DT(DataTableFilterVM cmsDTFilter)
        {
            var CmsPages = _db.CmsPages.Where(cms => cms.DeletedAt == null).AsQueryable();
            if (!string.IsNullOrEmpty(cmsDTFilter.Search))
            {
                CmsPages = CmsPages.Where(cms => cms.Title.ToLower().Contains(cmsDTFilter.Search.ToLower().Trim()));
            }
            switch (cmsDTFilter.SortBy)
            {
                default:
                    CmsPages = cmsDTFilter.SortOrder == "desc" ? CmsPages.OrderByDescending(cms => cms.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt) : CmsPages.OrderBy(u => u.Status).ThenByDescending(cms => cms.UpdatedAt ?? cms.CreatedAt);
                    break;
            }
            int totalRecords = CmsPages.Count();
            CmsPages = CmsPages.Skip(cmsDTFilter.PageStart).Take(cmsDTFilter.PageLength);
            DataTableVM<CmsPage> CmsPagesDT = new DataTableVM<CmsPage>(CmsPages.ToList(), totalRecords);
            return CmsPagesDT;
        }

        public AdminCmsPageVM GetCMSPageById(long cmsPageId)
        {
            var _CmsPage = _db.CmsPages.Find(cmsPageId);
            if (_CmsPage == null)
                return null;
            AdminCmsPageVM cmsPageVMObj = new AdminCmsPageVM
            {
                CmsPageId = _CmsPage.CmsPageId,
                Title = _CmsPage.Title,
                Description = WebUtility.HtmlDecode(_CmsPage.Description),
                Slug = _CmsPage.Slug,
                Status = _CmsPage.Status,
            };
            return cmsPageVMObj;
        }

        public bool RemoveCMSPage(long cmsPageId)
        {
            CmsPage? cmsPage = _db.CmsPages.Find(cmsPageId);
            if (cmsPage != null)
            {
                cmsPage.DeletedAt = DateTime.Now;
                _db.CmsPages.Update(cmsPage);
                return true;
            }
            return false;
        }

        public string SaveCMSPage(AdminCmsPageVM cmsPageData)
        {
            try
            {
                string response = string.Empty;
                CmsPage? cmsPage = new CmsPage();
                if (cmsPageData.CmsPageId != null)
                    cmsPage = _db.CmsPages.Find(cmsPageData.CmsPageId) ?? new CmsPage();

                cmsPage.Title = cmsPageData.Title;
                cmsPage.Description = WebUtility.HtmlEncode(cmsPageData.Description);
                cmsPage.Slug = cmsPageData.Slug;
                cmsPage.Status = cmsPageData.Status;
                if (cmsPage.CmsPageId != 0)
                {
                    cmsPage.UpdatedAt = DateTime.Now;
                    response = "CMS Page Updated!";
                }
                else
                {
                    cmsPage.CreatedAt = DateTime.Now;
                    response = "CMS Page Added!";
                }
                _db.CmsPages.Update(cmsPage);
                return response;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        //---- Banner
        public List<Banner> GetAllBanners()
        {
            var _Banners = _db.Banners.Where(banner => banner.DeletedAt == null).AsQueryable();
            _Banners = _Banners.OrderBy(banner => banner.SortOrder);
            return _Banners.ToList();
        }
        public DataTableVM<Banner> GetBannerDT(DataTableFilterVM bannerDTFilter)
        {
            var _Banners = _db.Banners.Where(cms => cms.DeletedAt == null).AsQueryable();
            if (!string.IsNullOrEmpty(bannerDTFilter.Search))
            {
                _Banners = _Banners.Where(cms => cms.Title.ToLower().Contains(bannerDTFilter.Search.ToLower().Trim()));
            }
            switch (bannerDTFilter.SortBy)
            {
                case "sortOrder":
                    _Banners = bannerDTFilter.SortOrder == "desc" ? _Banners.OrderByDescending(cms => cms.SortOrder) : _Banners.OrderBy(u => u.SortOrder);
                    break;
                default:
                    _Banners = bannerDTFilter.SortOrder == "desc" ? _Banners.OrderByDescending(cms => cms.UpdatedAt) : _Banners.OrderBy(u => u.UpdatedAt);
                    break;
            }
            int totalRecords = _Banners.Count();
            _Banners = _Banners.Skip(bannerDTFilter.PageStart).Take(bannerDTFilter.PageLength);
            DataTableVM<Banner> BannersDT = new DataTableVM<Banner>(_Banners.ToList(), totalRecords);
            return BannersDT;
        }

        public Banner GetBannerById(long bannerId)
        {
            return _db.Banners.Find(bannerId);
        }

        public string AddOrUpdateBanner(AdminBannerVM bannerDataObj)
        {
            try
            {
                Banner bannerObj;
                string response;
                if (bannerDataObj.BannerId != null)
                {
                    bannerObj = _db.Banners.Find(bannerDataObj.BannerId);
                    if (bannerObj == null)
                        return null;
                    response = "Banner Updated Successfully!";
                }
                else
                {
                    bannerObj = new Banner();
                    bannerObj.CreatedAt = DateTime.Now;
                    response = "Banner Added Successfully!";
                }
                bannerObj.Title = bannerDataObj.Title;
                bannerObj.Text = bannerDataObj.Text;
                if (bannerDataObj.ImagePath != null)
                    bannerObj.Image = bannerDataObj.ImagePath;
                bannerObj.SortOrder = bannerDataObj.SortOrder;
                bannerObj.UpdatedAt = DateTime.Now;
                _db.Banners.Update(bannerObj);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool RemoveBanner(long bannerId)
        {
            Banner? banner = _db.Banners.Find(bannerId);
            if (banner != null)
            {
                banner.DeletedAt = DateTime.Now;
                banner.UpdatedAt = DateTime.Now;
                _db.Banners.Update(banner);
                return true;
            }
            return false;
        }
    }
}
