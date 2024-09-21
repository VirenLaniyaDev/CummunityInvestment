using CummunityInvestmentWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CummunityInvestmentWeb.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                StatusCode = statusCode
            };
            return View(errorViewModel);
        }

        [Route("Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                StatusCode = 401
            };
            return View("~/Views/Shared/Error.cshtml", errorViewModel);
        }
    }
}
