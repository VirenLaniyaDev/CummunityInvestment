using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Application.Utilities
{
    public static class HtmlHelperExtensions
    {
        //public static string ActiveLinkClass(this IHtmlHelper htmlHelper, string route)
        //{
        //    var routeData = htmlHelper?.ViewContext.RouteData;
        //    var pageRoute = routeData?.Values["page"].ToString();
        //    return route == pageRoute ? "active" : "";
        //}

        public static string ActiveLinkClass(this IHtmlHelper htmlHelper, string controllers = null, string actions = null, string cssClass = "active")
        {
            var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper?.ViewContext.RouteData.Values["action"] as string;

            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
            var acceptedActions = (actions ?? currentAction ?? "").Split(',');

            return acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction)
                ? cssClass
                : "";
        }
    }
}
