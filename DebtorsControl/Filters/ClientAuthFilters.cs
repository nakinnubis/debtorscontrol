using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.Filters
{
    public class ClientAuthFilters : System.Web.Mvc.ActionFilterAttribute

    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["ClientId"] == null)
            {
                filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"Controller", "Account"},
                    {"Action", "Login"}
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}