using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace eConnectV2.Filters
{
    public class DecryptQueryStringParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dataProtectionProvider = DataProtectionProvider.Create("WebQuery");
            var protector = dataProtectionProvider.CreateProtector("WebQuery.QueryStrings");
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (filterContext.HttpContext.Request.Query["q"].ToString() != null)
            {
                string decrptedString = protector.Unprotect(filterContext.HttpContext.Request.Query["q"].ToString());
                string[] getRandom = decrptedString.Split('[');
                var format = new CultureInfo("en-GB");
                var dateCheck = Convert.ToDateTime(getRandom[2], format);
                TimeSpan diff = Convert.ToDateTime(DateTime.Now, format) - dateCheck;
                /* For Development it is been commented */
                if (diff.Minutes > 30)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Error", controller = "Error" }));
                }
                string[] paramsArrs = getRandom[1].Split(',');
                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');
                    decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));
                }
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionArguments[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
        }
    }
}
