using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Application.Common
{
    public class UserMessages
    {
        //---- Authentication Messages ----
        public static readonly string EmailRegistered = "Email is Already Registered!";
        public static readonly string EmailNotRegistered = "Email is not Registered!";
        public static readonly string LoginSuccess = "Welcome to, C.V. Network!";
        public static readonly string InvalidPassword = "Invalid Password!";
        public static readonly string PasswordsNotSame = "Passwords not match!";
        //-- Authentication Tostr Messages
        public static readonly string ResetPassLink_Success = "Link to Reset Password has been sent to ";

        //-- User Profile Tostr Messages
        public static readonly string UserProfileUpdated_Success = "Your Profile Updated successfully!";
        public static readonly string UserProfileUpdated_SuccessTitle = "Profile Updated";

        //-- Volunteering Mission
        public static readonly string MissionRatings_SuccessTitle = "Thanks for Ratings!";
        public static readonly string MissionRatings_SuccessMessage = "Your Ratings and Feedback is successfully submitted!";
        public static readonly string MissionRecommendation_SuccessTitle = "Mission Recommendation Sent!";
        public static readonly string MissionRecommendation_SuccessMessage = "Your recommendation for this mission has been sent via email!";

        //---- User Profile 
        // Change Password
        public static readonly string OldAndNewPasswordsMatch_ErrorMessage = "New password should not be same as Current password!";
        public static readonly string InvalidOldPassword_ErrorMessage = "Invalid Current Password!";
        public static readonly string NewAndConfirmPasswordsNotMatch_ErrorMessage = "NewPassword and ConfirmPassword not Matched!";
        public static readonly string PasswordUpdated_SuccessTitle = "Password Updated!";
        public static readonly string PasswordUpdated_SuccessMessage = "Your password has been successfully updated!";

    }
}
