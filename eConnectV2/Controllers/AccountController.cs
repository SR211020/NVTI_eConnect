using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICommonService _commonService;
        public AccountController(ICommonService commonService)
        {
            _commonService = commonService;
        }
        public IActionResult Index()
        {
            string food = "";
            string snacks = "";
            var food_dt = _commonService.usp_Master(new CommonViewModel() { Activity = "GetTodayFoodMenu" });
            if (food_dt != null)
            {
                if (food_dt.Rows.Count > 0)
                {
                    food = food_dt.Rows[0]["Menu"].ToString();
                    snacks = food_dt.Rows[0]["Snacks"].ToString();
                }
            }
            ViewBag.TodayFoodMenu = food;
            ViewBag.TodaySnacksMenu = snacks;

            string winUserName = "", empcode = "", IsUserLoggedIn = "0", EnableChatBot = "0";//, hodcode = "";
            try
            {
                if (User.Identity.IsLoggedIn())
                {
                    winUserName = User.Identity.GetEmpName();
                    empcode = User.Identity.GetEmpCode();
                    IsUserLoggedIn = "1";
                }
            }
            catch (Exception)
            {
                winUserName = empcode = "";
                IsUserLoggedIn = "0";
            }
            var dt = _commonService.usp_LogIn(new LoginViewModel() { Activity = "IS_CHATBOT_ENABLE" });
            if (dt != null && dt.Rows.Count > 0)
            {
                EnableChatBot = dt.Rows[0]["VALUE"].ToString();
            }
            ViewBag.WinUserName = winUserName;
            ViewBag.EmpCode = empcode;
            ViewBag.IsUserLoggedIn = IsUserLoggedIn;
            ViewBag.EnableChatBot = EnableChatBot;
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account");
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public JsonResult Login(string userId, string password)
        {
            var result = "1";
            var msg = Common.SomethingWentWrong;
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    return Json(new { result = "0", msg = "UserId and Password is required" });
                }
                else
                {
                    PrincipalContext pc = new(ContextType.Domain, "NVTPOWER");
                    var data = _commonService.usp_LogIn(new LoginViewModel() { Activity = "GET_EMP_DETAILS", UserId = userId.Trim() });
                    if (data != null && data.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(data.Rows[0]["ISACTIVE"]))
                        {
                            string strADId = data.Rows[0]["ADID"].ToString();
                            string strEmpCode = data.Rows[0]["EMPCODE"].ToString();
                            if (CheckPassword(pc, strADId, password))
                            {
                                if (AddSessionData(strADId, strEmpCode,
                                data.Rows[0]["EMPNAME"].ToString(),
                                data.Rows[0]["PLANT"].ToString(),
                                data.Rows[0]["DESIGNATION"].ToString(),
                                data.Rows[0]["DEPTCODE"].ToString(),
                                data.Rows[0]["DEPTNAME"].ToString(),
                                data.Rows[0]["CONTACTNO_O"].ToString(),
                                data.Rows[0]["LANDLINE"].ToString(),
                                data.Rows[0]["EMAILID_O"].ToString()))
                                {
                                    UpdateLoginTime(strADId, strEmpCode, data.Rows[0]["EMPNAME"].ToString());
                                    return Json(new { result, msg = "Log in successfully" });
                                }
                                else
                                {
                                    return Json(new { result = "0", msg });
                                }
                            }
                            else
                            {
                                return Json(new { result = "0", msg = "Invalid Password" });
                            }
                        }
                        else
                        {
                            return Json(new { result = "0", msg = "Your account is disabled" });
                        }
                    }
                    else
                    {
                        DirectoryEntry entry = new DirectoryEntry("WinNT://nvtpower.com");
                        UserPrincipal user = UserPrincipal.FindByIdentity(pc, userId.Trim().ToUpper());
                        foreach (DirectoryEntry child in entry.Children)
                        {
                            if (child.SchemaClassName.Trim().ToUpper() == "USER" && child.Name.ToUpper() == userId.Trim().ToUpper())
                            {
                                if (user.Enabled == false)
                                {
                                    return Json(new { result = "0", msg = "User Id is Disabled" });
                                }
                                if (user.IsAccountLockedOut() == true)
                                {
                                    return Json(new { result = "0", msg = "User Id is Locked" });
                                }
                                if (CheckPassword(pc, userId, password))
                                {
                                    string adId = user.SamAccountName != null ? user.SamAccountName.ToString().Trim().ToUpper() : "";
                                    string empCode = user.Description != null ? user.Description.ToString().Trim().ToUpper() : "";
                                    string email = user.EmailAddress != null ? user.EmailAddress.ToString().Trim() : "";
                                    //string contact_no = user.VoiceTelephoneNumber != null ? user.VoiceTelephoneNumber.ToString().Trim() : "";
                                    var dt = _commonService.usp_LogIn(new LoginViewModel() { Activity = "UPDATE_ADID", UserId = adId, Param1 = empCode });
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        if (AddSessionData(
                                            dt.Rows[0]["ADID"].ToString(),
                                            dt.Rows[0]["EMPCODE"].ToString(),
                                            dt.Rows[0]["EMPNAME"].ToString(),
                                            dt.Rows[0]["PLANT"].ToString(),
                                            dt.Rows[0]["DESIGNATION"].ToString(),
                                            dt.Rows[0]["DEPTCODE"].ToString(),
                                            dt.Rows[0]["DEPTNAME"].ToString(),
                                            dt.Rows[0]["CONTACTNO_O"].ToString(),
                                            dt.Rows[0]["LANDLINE"].ToString(),
                                            dt.Rows[0]["EMAILID_O"].ToString()))
                                        {
                                            UpdateLoginTime(dt.Rows[0]["ADID"].ToString(), dt.Rows[0]["EMPCODE"].ToString(), dt.Rows[0]["EMPNAME"].ToString());
                                            return Json(new { result, msg = "Log in successfully" });
                                        }
                                        else
                                        {
                                            return Json(new { result = "0", msg });
                                        }
                                    }
                                    else
                                    {
                                        //Fill through LDAP Properties
                                        if (AddSessionData(adId, empCode, user.DisplayName, "", "", "", "", "", "", email))
                                        {
                                            UpdateLoginTime(adId, empCode, user.DisplayName);
                                            return Json(new { result, msg = "Log in successfully" });
                                        }
                                        else
                                        {
                                            return Json(new { result = "0", msg });
                                        }
                                    }
                                }
                                else
                                {
                                    return Json(new { result = "0", msg = "Invalid Password" });
                                }
                            }
                        }
                        return Json(new { result = "0", msg = "Invalid User Id" });
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { result = "0", msg });
            }
        }
        private bool AddSessionData(string adId, string empCode, string empName, string plant, string designation, string deptCode, string deptName, string contactno, string extno, string emailId)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(Common.SK_ADId, adId is null ? "" : adId),
                    new Claim(Common.SK_EmpCode, empCode is null ? "" : empCode),
                    new Claim(Common.SK_EmpName, empName is null ? "" : empName),
                    new Claim(Common.SK_Plant, plant is null ? "" : plant),
                    new Claim(Common.SK_EmailId, emailId is null ? "" : emailId),
                    new Claim(Common.SK_Designation, designation is null ? "" : designation),
                    new Claim(Common.SK_DeptCode, deptCode is null ? "": deptCode),
                    new Claim(Common.SK_ContactNo, contactno is null ? "" : contactno),
                    new Claim(Common.SK_ExtNo, extno is null ? "" : extno),
                    new Claim(Common.SK_DeptName, deptName is null ? "" : deptName)
                };
                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties authProperties = new()
                {
                    IsPersistent = false,
                };
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static bool CheckPassword(PrincipalContext pc, string userId, string password) => pc.ValidateCredentials(userId.Trim().ToUpper(), password);

        private void UpdateLoginTime(string adid, string empcode, string empname)
        {
            string ipAddress = string.Empty;
            string pcName = Dns.GetHostName();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }
            _commonService.usp_LogIn(new LoginViewModel()
            {
                Activity = "UPDATE_LOGIN_DETAILS",
                UserId = adid,
                Param1 = empcode,
                Name = empname,
                Param2 = pcName,
                Param4 = ipAddress,
                Param5 = pcName
            });
        }

        [HttpPost]
        public JsonResult Phonebook(string request)
        {
            List<string> data = new();
            if (!string.IsNullOrEmpty(request))
            {
                var data1 = _commonService.usp_LogIn(new LoginViewModel() { Activity = "GET_PHONEBOOK", Param1 = request });
                if (data1 != null && data1.Rows.Count > 0)
                {
                    foreach (DataRow row in data1.Rows)
                    {
                        data.Add(item: $"{row["EMPNAME"]}-{row["DEPTNAME"]}-{row["EMAILID_O"]}-{row["CONTACTNO"]}-{row["LANDLINE"]}-{row["EMPCODE"]}-{row["ADID"]}");
                    }
                }
            }
            return Json(data.ToArray());
        }

        public IActionResult Error404() => View();

        [HttpPost]
        public JsonResult OpenBIS()
        {
            try
            {
                System.Diagnostics.Process.Start("\\\\svbavmbis\\MyShare\\BIS-NVT.exe");
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex.ToString());
            }
        }

        [HttpPost]
        public JsonResult OpenMES()
        {
            try
            {
                System.Diagnostics.Process.Start(@"C:\Users\NI00005\AppData\Local\NVT MES\NVTMES\RTDMain.exe");
                return Json("OK");
            }
            catch(Exception ex)
            {
                return Json(ex.ToString());
            }
            //return Json("1");
        }
    }
}
