using CommunityInvestment.Models;
using CommunityInvestment.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommunityInvestment.Models.ViewModels;
using CommunityInvestment.DataAccess.Repository;
using CommunityInvestment.DataAccess.Repository.IRepository;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using CommunityInvestment.Application.Common;

namespace CummunityInvestmentWeb.Users.Controllers
{
    public class AuthenticationController : Controller
    {
        public IUnitOfWork _unitOfWork;
        public AuthenticationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            if (claimsUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginVM user, string returnUrl = "/")
        {
            var _admin = _unitOfWork.Admin.GetByEmail(user.Email);
            var _user = _unitOfWork.User.GetByEmail(user.Email);
            if (_user == null && _admin == null)
            {
                ModelState.AddModelError("Email", UserMessages.EmailNotRegistered);
            }
            else
            {
                if(_unitOfWork.Admin.Authenticate(user.Email, user.Password))
                {
                    // Perform Admin authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, _admin.FirstName +" "+_admin.LastName),
                        new Claim("FirstName", _admin.FirstName ?? ""),
                        new Claim("LastName", _admin.LastName ?? ""),
                        new Claim(ClaimTypes.NameIdentifier, _admin.AdminId.ToString()),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Email, _admin.Email),
                        new Claim("avatar_url", _admin.Avatar ?? "/assets/user-profile-avatar.svg")
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    returnUrl = "/Administrator/AdminActivity/ManageUsers";
                    return Redirect(returnUrl);
                }
                else if (_unitOfWork.User.Authenticate(user.Email, user.Password))
                {
                    // Perform user authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, _user.FirstName +" "+_user.LastName),
                        new Claim(ClaimTypes.NameIdentifier, _user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, _user.Email),
                        new Claim("avatar_url", _user.Avatar ?? "/assets/user-profile-avatar.svg"),
                        new Claim("countryId", _user.CountryId.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("Password", UserMessages.InvalidPassword);
                }
            }
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View(user);
        }

        public IActionResult Signup()
        {
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(UserRegisterVM user)
        {
            var _user = _unitOfWork.User.GetByEmail(user.Email);
            if (_user != null)
            {
                ModelState.AddModelError("Email", UserMessages.EmailRegistered);
            }
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", UserMessages.PasswordsNotSame);
            }
            if (ModelState.IsValid)
            {
                User userObj = new User();
                _unitOfWork.User.Register(new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Password = user.Password,
                    CityId = 2,
                    CountryId = 98
                });
                _unitOfWork.Save();
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View(user);
        }

        public IActionResult ForgotPassword()
        {
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(UserForgotPasswordVM user)
        {
            var _user = _unitOfWork.User.GetByEmail(user.Email);
            var _admin = _unitOfWork.Admin.GetByEmail(user.Email);
            if (_user == null && _admin == null)
            {
                ModelState.AddModelError("Email", UserMessages.EmailNotRegistered);
            }
            else
            {
                if (_admin != null)
                    _unitOfWork.Admin.ForgotPassword(user.Email);
                else
                    _unitOfWork.User.ForgotPassword(user.Email);
                _unitOfWork.Save();
                TempData["ResetLinkSent_Success"] = UserMessages.ResetPassLink_Success;
                TempData["ResetLinkEmail"] = user.Email;
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View(user);
        }

        [Route("Users/Authentication/ResetPassword/{token}")]
        public IActionResult ResetPassword(string token)
        {
            var result = _unitOfWork.User.getPR_RecordByToken(token);
            if (result != null)
            {
                ViewData["UserEmail"] = result.Email;
                ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
                return View("~/Areas/Users/Views/Authentication/ResetPassword.cshtml");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Users/Authentication/ResetPassword/{token}")]
        public IActionResult ResetPassword(UserResetPasswordVM user)
        {
            if (ModelState.IsValid)
            {
                var result = _unitOfWork.Admin.GetByEmail(user.Email);
                if(result != null)
                    _unitOfWork.Admin.UpdatePassword(user.Email, user.Password);
                else
                    _unitOfWork.User.UpdatePassword(user.Email, user.Password);
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.BannersList = _unitOfWork.Admin.GetAllBanners();
            return View("~/Areas/Users/Views/Authentication/ResetPassword.cshtml", user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
