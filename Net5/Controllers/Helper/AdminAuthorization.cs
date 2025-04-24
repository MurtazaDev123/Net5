using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;



namespace SmartEcommerce.Controllers.Helper
{
    public class AdminAuthorization : System.Web.Mvc.AuthorizeAttribute
    {
        public string[] Exemption { get; set; }
        
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Exemption.Contains(filterContext.RouteData.GetRequiredString("action").ToLower()))
                return;

            if (!BusinessLogic.clsCookie.Exists)
            {
                //BusinessLogic.clsCookie.Logout();
                //prevent execution of the action method
                filterContext.Result = new RedirectResult("/admin/login");
                //filterContext.Result = new RedirectResult("/Account/Login?returnurl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
            }
            else
            {
                if (BusinessLogic.clsSession.LoginId == 0)
                {
                    filterContext.Result = new RedirectResult("/admin/logout");
                }

            }
        }
    }
}