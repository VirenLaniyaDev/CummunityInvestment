using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IUserRepository
    {
        User GetUserById(long id);
        User GetByEmail(string email);
        void Register(User user);
        bool Authenticate(string UEmail, string UPassword);
        void ForgotPassword(string UEmail);
        PasswordReset getPR_RecordByToken(string token);
        void UpdatePassword(string UEmail, string UPassword);
        public List<User> GetCoWorkers(long userId, string searchFilter);
        public UserProfileVM GetUserProfileById(long userId);
        public List<UserSkill> GetUserSkills(long userId);
        public Task<User> UpdateUserProfileAsync(UserProfileVM userProfile);
        public bool CheckOldPassword(long userId, string oldPassword);
        public bool UpdatePassword(long userId, string newPassword);
        public bool SendQueryMessage(ContactUsVM contactUs);
        void Save();
    }
}
