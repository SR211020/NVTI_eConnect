using eConnectV2.Filters;
using eConnectV2.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eConnectV2.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ActionFilterConfig))]
    public class HomeController : Controller
    {
        public IActionResult Dashboard(string msg = null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return View();
            }
            else
            {
                if (msg == "m1")
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.ResultUnauthorizedMessage;
                }
                return View();
            }
        }

        public IActionResult EISDashboard()
        {
            return View();
        }
    }
}
