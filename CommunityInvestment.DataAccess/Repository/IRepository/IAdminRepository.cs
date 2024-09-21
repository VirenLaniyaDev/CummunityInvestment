using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IAdminRepository
    {
        public Admin GetAdminById(long adminId);
        public Admin GetByEmail(string email);
        public bool Authenticate(string AdminEmail, string AdminPassword);
        public void ForgotPassword(string UEmail);
        public Admin AddOrUpdateAdmin(AdminProfileVM adminProfileDataObj);


        public bool CheckOldPassword(long adminId, string oldPassword);
        public bool UpdatePassword(long adminId, string newPassword);
        public bool UpdatePassword(string adminEmail, string newPassword);
        public List<CmsPage> GetAllCMSPolicies(string statusFilter = "");


        public DataTableVM<User> GetUsersDT(DataTableFilterVM usersDTFilter);
        public AdminUserManageVM GetUserEdit(long userId);
        public string AddOrUpdateUser(AdminUserManageVM adminUserManageObj);
        public bool RemoveUser(long userId);


        public DataTableVM<CmsPage> GetCMS_DT(DataTableFilterVM cmsDTFilter);
        public AdminCmsPageVM GetCMSPageById(long cmsPageId);
        public bool RemoveCMSPage(long cmsPageId);
        public string SaveCMSPage(AdminCmsPageVM cmsPageData);


        public List<Banner> GetAllBanners();
        public DataTableVM<Banner> GetBannerDT(DataTableFilterVM bannerDTFilter);
        public Banner GetBannerById(long bannerId);
        public string AddOrUpdateBanner(AdminBannerVM bannerDataObj);
        public bool RemoveBanner(long bannerId);
    }
}
