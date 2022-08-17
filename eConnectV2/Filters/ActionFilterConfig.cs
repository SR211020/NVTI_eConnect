using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;

namespace eConnectV2.Filters
{
    public class ActionFilterConfig : IActionFilter
    {
        private readonly ICommonService _commonService;

        public ActionFilterConfig(ICommonService commonService)
        {
            _commonService = commonService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var _controller = "";
            var _action = "";
            var _adId = "";
            var _requestUrl = "";
            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            Claim claimAdId = claimsIdentity.FindFirst(Common.SK_ADId);
            if (string.IsNullOrEmpty(claimAdId.ToString()))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Index" } });
            }
            else
            {
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    //no need to validate
                }
                else
                {
                    _controller = context.RouteData.Values["controller"].ToString();
                    _action = context.RouteData.Values["action"].ToString();
                    _requestUrl = "/" + _controller + "/" + _action;
                    _adId = claimAdId.Value;
                    var dt = _commonService.usp_LogIn(new LoginViewModel() { Activity = "CHECK_PERMISSION", Param1 = _requestUrl, UserId = _adId });
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["FLAG"]) || _requestUrl == "/KPI/Download")
                        {
                            //no need to take any action
                        }
                        else
                        {
                            context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Dashboard" }, { "msg", "m1" } });
                        }
                    }
                }
            }
        }
    }
}
