using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Application.Utilities
{
    public class GeneralUtility
    {
        public static long GetClaimIdentifier(ClaimsPrincipal User)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                string userId_Str = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Convert.ToInt64(userId_Str);
            }
            return 0;
        }

        public static string UploadFile(string fileUploadPath, IFormFile file, string returnPath = "")
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(fileUploadPath, uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));
            return returnPath + uniqueFileName;
        }

        public static FileInfo GetFileInfo(string rootPath, string filePath, char pathSeparator = '/')
        {
            string[] filePathAttributes = filePath.Split('/');
            string[] pathAttributes = new[] { rootPath }.Concat(filePathAttributes).ToArray();
            var fileInfo = new FileInfo(Path.Combine(pathAttributes));
            return fileInfo;
        }

        public static string GetHashedPassword(string Password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(16);
            return BCrypt.Net.BCrypt.HashPassword(Password, salt);
        }
    }
}
