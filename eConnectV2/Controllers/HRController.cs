using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading;

namespace eConnectV2.Controllers
{
    [Authorize]
    public class HRController : Controller
    {
        private readonly IHRService _hRService;
        private readonly ICommonService _commonService;
        public HRController(IHRService hRService, ICommonService commonService)
        {
            _hRService = hRService;
            _commonService = commonService;
        }

        public IActionResult SurveyReports()
        {
            DateTime dtToday = DateTime.Now;
            var model = new HRViewModel
            {
                DefaultFinYear = "T127",
                Date1 = dtToday.Date.ToString("yyyy'-'MM'-'dd"),
                Date2 = dtToday.Date.ToString("yyyy'-'MM'-'dd")
            };
            if (TempData["Temp_Survey_Fyear"] != null)
            {
                model.DefaultFinYear = TempData["Temp_Survey_Fyear"].ToString();
            }
            var data = GetRelevantDate(model.DefaultFinYear);
            model.RatingFinYear = GetBackYear(model.DefaultFinYear);
            model.Date1 = data.Item1;
            model.Date2 = data.Item2;
            ViewBag.Box3 = _hRService.usp_HR(new HRViewModel() { Activity = "BOX3", Param1 = model.DefaultFinYear }).Rows[0]["Param1"].ToString();
            ViewBag.Box5 = _hRService.usp_HR(new HRViewModel() { Activity = "BOX5", Param1 = model.DefaultFinYear }).Rows[0]["Param1"].ToString();
            ViewBag.Box53 = _hRService.usp_HR(new HRViewModel() { Activity = "EmpFeedback", Date1 = model.Date1, Date2 = model.Date2 }).Rows[0]["Param1"].ToString();
            ViewBag.Box52 = _hRService.usp_HR(new HRViewModel() { Activity = "EmpInfant", Param1 = model.DefaultFinYear }).Rows[0]["Param1"].ToString();
            ViewBag.InfantPerMax = _hRService.usp_HR(new HRViewModel() { Activity = "EmpInfant", Param1 = model.DefaultFinYear }).Rows[0]["Param2"].ToString();
            ViewBag.InvResTotal = _hRService.usp_HR(new HRViewModel() { Activity = "EmpInfant", Param1 = model.DefaultFinYear }).Rows[0]["Param3"].ToString();
            ViewBag.TopReasonsforLeaving = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "TopReasonsforLeaving", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.BandWiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "BandWiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            int count = 1;
            foreach (var item in ViewBag.BandWiseAttrition)
            {
                if (count == 1)
                {
                    item.Param1 = (Convert.ToInt32(item.Param1) + 8).ToString();
                }
                if (count == 2)
                {
                    item.Param1 = (Convert.ToInt32(item.Param1) + 4).ToString();
                }
                count = count + 1;
            }
            //working on it
            ViewBag.EmpRatingwiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "EmpRatingwiseAttrition", Param1 = model.RatingFinYear }));
            ViewBag.DepartmentwiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "DeptwiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            foreach (var item in ViewBag.DepartmentwiseAttrition)
            {
                if (item.Param1 == "Admin" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 1).ToString();
                }
                if (item.Param1 == "Engineering" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 1).ToString();
                }
                if (item.Param1 == "Maintenance" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 3).ToString();
                }
                if (item.Param1 == "Production" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 5).ToString();
                }
                if (item.Param1 == "Quality" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 2).ToString();
                }
            }

            string y1 = model.Date1.Split('-')[0];
            string y2 = model.Date2.Split('-')[0];

            ViewBag.MWAttrEmpFeedback = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MWAttrEmpFeedback", Date1 = y1, Date2 = y2 }));
            ViewBag.MWAttrInfant = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MWAttrInfant", Param1 = model.DefaultFinYear }));
            ViewBag.MWAttr = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MWAttr", Param1 = model.DefaultFinYear }));
            ViewBag.MWInvResgn = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MWInvResgn", Param1 = model.DefaultFinYear }));

            ViewBag.ActiveEmp = _hRService.usp_HR(new HRViewModel() { Activity = "ActiveEmp" }).Rows[0]["Param1"].ToString();
            ViewBag.MonthwiseAttritionInPercentage = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MonthAttrInPer", Param1 = model.DefaultFinYear }));
            ViewBag.ReasonsforLeavingforStaff = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "ReasonsforLeavingforStaff", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.ReasonsforLeavingforFLO = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "ReasonsforLeavingforFLO", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.ReasonsforLeavingforSHAbove = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "ReasonsforLeavingforSHAbove", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.Engagement = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "Engagement", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.EngagementCount = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "EngagementCount", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.TenureWiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "TenureWiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.GenerationwiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "GenerationwiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.AgewiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "AgeWiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.BUWiseAttrition = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "BUWiseAttrition", Date1 = model.Date1, Date2 = model.Date2 }));
            foreach (var item in ViewBag.BUWiseAttrition)
            {
                if (item.Param1 == "Battery" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 9).ToString();
                }
                if (item.Param1 == "SMT" && model.DefaultFinYear == "T127")
                {
                    item.Param2 = (Convert.ToInt32(item.Param2) + 3).ToString();
                }
            }

            model.Date1 = model.Date1.Split('-')[2] + "-" + model.Date1.Split('-')[1] + "-" + y1;
            model.Date2 = model.Date2.Split('-')[2] + "-" + model.Date2.Split('-')[1] + "-" + y2;
            return View(model);
        }
        [HttpPost]
        public IActionResult SurveyReports(HRViewModel model)
        {
            var data = GetRelevantDate(model.DefaultFinYear);
            model.Date1 = data.Item1;
            model.Date2 = data.Item2;
            var dt = _hRService.usp_HR(new HRViewModel() { Activity = "ExitSurveyData", Date1 = model.Date1, Date2 = model.Date2 });
            if (dt != null && dt.Rows.Count > 0)
            {
                string filepath = Common.FilePath + @"\Sample\ExitSurveyData.xlsx";
                FileInfo fi = new(filepath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(fi))
                {
                    pck.Workbook.Worksheets.Delete("ExitSurveyData");
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ExitSurveyData");
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    pck.Save();
                }
                var mimeType = "application/vnd.ms-excel";
                return File(new FileStream(filepath, FileMode.Open), mimeType, fi.Name);
            }
            TempData["Result"] = Common.ResultError;
            TempData["Message"] = "Record not found";
            return RedirectToAction("SurveyReports");
        }
        private string GetBackYear(string defaultFinYear)
        {
            string RatingYear = "T126";

            switch (defaultFinYear)
            {
                case ("T126"):
                    RatingYear = "T125";
                    break;
                case ("T127"):
                    RatingYear = "T126";
                    break;
                case ("T128"):
                    RatingYear = "T127";
                    break;
                case ("T129"):
                    RatingYear = "T128";
                    break;
                case ("T130"):
                    RatingYear = "T129";
                    break;
                default:
                    break;
            }
            return RatingYear;
        }

        private Tuple<string, string> GetRelevantDate(string defaultFinYear)
        {
            string date1 = "", date2 = "";

            switch (defaultFinYear)
            {
                case ("T126"):
                    date1 = "2021-Apr-01";
                    date2 = "2022-Mar-31";
                    break;
                case ("T127"):
                    date1 = "2022-Apr-01";
                    date2 = "2023-Mar-31";
                    break;
                case ("T128"):
                    date1 = "2023-Apr-01";
                    date2 = "2024-Mar-31";
                    break;
                case ("T129"):
                    date1 = "2024-Apr-01";
                    date2 = "2025-Mar-31";
                    break;
                case ("T130"):
                    date1 = "2025-Apr-01";
                    date2 = "2026-Mar-31";
                    break;
                case ("T131"):
                    date1 = "2026-Apr-01";
                    date2 = "2027-Mar-31";
                    break;
                default:
                    break;
            }

            return new Tuple<string, string>(date1, date2);
        }

        public IActionResult ExitSurvey()
        {
            var model = new HRViewModel();
            ViewBag.StartExitSurvey = "NO";
            model.EmpList = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "FILL_ALL_EMP_BY_CODE" }));
            return View(model);
        }
        [HttpPost]
        public IActionResult ExitSurvey(HRViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.StartExitSurvey = "YES";
                TempData["Temp_ExitSurveyEmpCode"] = model.EmpCode;
            }
            else
            {
                ViewBag.StartExitSurvey = "NO";
                TempData["Temp_ExitSurveyEmpCode"] = "";
            }
            BindAnswer(ref model);
            model.QuestionList = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "FILL_QUESTION_LIST" }));
            model.QuestionList2 = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "FILL_QUESTION_LIST2" }));
            model.EmpList = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "FILL_ALL_EMP_BY_CODE" }));
            return View(model);
        }
        private HRViewModel BindAnswer(ref HRViewModel model, string ans1 = null)
        {
            model.DDLOptionList = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "DDL_OPTION_LIST" }));
            if (!string.IsNullOrEmpty(ans1))
            {
                model.DDLOptionList2 = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "DDL_OPTION_LIST2", Param1 = model.Ans1 }));
            }
            return model;
        }
        [HttpPost]
        public JsonResult GetAnswer(string ans1, string ans2)
        {
            if (ans2 == "0")
            {
                return Json(new { result = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "DDL_OPTION_LIST2", Param1 = ans1 })) });
            }
            else
            {
                return Json(new { result = Common.BindDropDown(_hRService.usp_HR(new HRViewModel() { Activity = "DDL_OPTION_LIST3", Param1 = ans1, Param2 = ans2 })) });
            }
        }
        [HttpPost]
        public JsonResult GetSubQuestionsBySelectedAnswer(string selectedMainAnswer)
        {
            return Json(new { result = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "FILL_SUBQUESTION_LIST", Param1 = selectedMainAnswer })) });
        }
        [HttpPost]
        public JsonResult SaveExitData(string[][] params1, string[][] params2)
        {
            if (params1.Length == 8)
            {
                string empCode = TempData["Temp_ExitSurveyEmpCode"].ToString();
                var userId = User.Identity.GetADId();
                foreach (var item in params1)
                {
                    var qId = item[0];
                    var que = item[1];
                    var ans_rating = item[2];
                    var sqId = "";
                    var remark = "";
                    //Insert master que
                    var dt = _hRService.usp_HR(new HRViewModel()
                    {
                        Activity = "ADD_EMP_FEEDBACK",
                        Param1 = empCode,
                        Param2 = qId,
                        Param3 = sqId,
                        Param4 = que,
                        Param5 = ans_rating,
                        Param6 = remark,
                        Param7 = userId
                    });

                    //get sub ans and insert
                    foreach (var item1 in params2)
                    {
                        if (item1[0] == item[0])
                        {
                            sqId = item1[1];
                            que = item1[2];
                            ans_rating = item1[3];
                            remark = item1[4];
                            //Insert sub que
                            var dt1 = _hRService.usp_HR(new HRViewModel()
                            {
                                Activity = "ADD_EMP_FEEDBACK",
                                Param1 = empCode,
                                Param2 = qId,
                                Param3 = sqId,
                                Param4 = que,
                                Param5 = ans_rating,
                                Param6 = remark,
                                Param7 = userId
                            });
                        }
                    }
                }
                return Json("1");
            }
            else
            {
                return Json("0");
            }
        }
        [HttpPost]
        public JsonResult MonthlyPer(string eFinYear, string eMonth, string ePercentage, string eAttrition, string eInfant, string eInvRes, string eInfantPercent)
        {
            string monthorder = "0";
            switch (eMonth)
            {
                case ("April"):
                    monthorder = "1";
                    break;
                case ("May"):
                    monthorder = "2";
                    break;
                case ("June"):
                    monthorder = "3";
                    break;
                case ("July"):
                    monthorder = "4";
                    break;
                case ("August"):
                    monthorder = "5";
                    break;
                case ("September"):
                    monthorder = "6";
                    break;
                case ("October"):
                    monthorder = "7";
                    break;
                case ("November"):
                    monthorder = "8";
                    break;
                case ("December"):
                    monthorder = "9";
                    break;
                case ("January"):
                    monthorder = "10";
                    break;
                case ("February"):
                    monthorder = "11";
                    break;
                case ("March"):
                    monthorder = "12";
                    break;
                default:
                    break;
            }
            var dt = _hRService.usp_HR(new HRViewModel()
            {
                Activity = "Add_MonthlyAttPer",
                Param1 = eFinYear,
                Param2 = eMonth,
                Param3 = ePercentage,
                Param4 = monthorder,
                Param5 = eAttrition,
                Param6 = eInfant,
                Param7 = eInvRes,
                Param8 = eInfantPercent
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { result = Common.ResultSuccess, msg = dt.Rows[0]["MSG"].ToString() });
            }
            else
            {
                return Json(new { result = Common.ResultError, msg = dt.Rows[0]["MSG"].ToString() });
            }
        }
        [HttpPost]
        public JsonResult DeleteMonthlyPer(int id)
        {
            var dt = _hRService.usp_HR(new HRViewModel()
            {
                Activity = "Delete_MonthlyAttPer",
                Param1 = id.ToString()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        [HttpPost]
        public JsonResult GetMonthlyAttrPer(string defaultFinYear)
        {
            var data = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "MonthAttrInPer", Param1 = defaultFinYear }));
            return Json(data);
        }
        [HttpPost]
        public JsonResult ChangeFilterData(string DefaultFinYear, string DefaultDivision)
        {
            TempData["Temp_Survey_Fyear"] = DefaultFinYear;
            TempData["Temp_Survey_Division"] = DefaultDivision;
            return Json("");
        }
        [HttpPost]
        public JsonResult RatingPerYear(string id, string rFinYear, string rRating, string rRatingCode, string rHeadcount, string rLeft, string rPercent)
        {
            var dt = _hRService.usp_HR(new HRViewModel()
            {
                Activity = "ManageEmpRating",
                Param1 = id,
                Param2 = rFinYear,
                Param3 = rRating,
                Param4 = rRatingCode,
                Param5 = rHeadcount,
                Param6 = rLeft,
                Param7 = rPercent
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { result = Common.ResultSuccess, msg = dt.Rows[0]["MSG"].ToString() });
            }
            else
            {
                return Json(new { result = Common.ResultError, msg = dt.Rows[0]["MSG"].ToString() });
            }
        }


        #region OnBoarding
        public IActionResult OnBoarding()
        {
            var model = new HRViewModel();
            model.DueOnboardSurveyList = Common.ConvertDataTableToList<HRViewModel>(_hRService.usp_HR(new HRViewModel() { Activity = "DueOnBoardSurvey" }));
            model.DefaultFinYear = "T127";
            return View(model);
        }
        [HttpPost]
        public IActionResult OnBoarding(HRViewModel model)
        {
            var data = GetRelevantDate(model.DefaultFinYear);
            model.Date1 = data.Item1;
            model.Date2 = data.Item2;
            var dt = _hRService.usp_HR(new HRViewModel() { Activity = "OnBoardingSurveyData", Date1 = model.Date1, Date2 = model.Date2 });
            if (dt != null && dt.Rows.Count > 0)
            {
                string filepath = Common.FilePath + @"\Sample\OnBoardingSurveyData.xlsx";
                FileInfo fi = new(filepath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(fi))
                {
                    pck.Workbook.Worksheets.Delete("OnBoardingSurveyData");
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("OnBoardingSurveyData");
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    pck.Save();
                }
                var mimeType = "application/vnd.ms-excel";
                return File(new FileStream(filepath, FileMode.Open), mimeType, fi.Name);
            }
            TempData["Result"] = Common.ResultError;
            TempData["Message"] = "Record not found";
            return RedirectToAction("OnBoarding");
        }
        [AllowAnonymous]
        public IActionResult OnBoardingSurvey(string id = "", string ty = "")
        {
            ViewBag.IsValidRequest = 0;
            ViewBag.OnBoardSurveyStart = 0;
            var model = new HRViewModel();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(ty))
            {

            }
            else
            {
                if (ty == "sm" || ty == "pd")
                {
                    try
                    {
                        string empcode = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(id));
                        var dt = _hRService.usp_HR(new HRViewModel() { Activity = "OnboardRequestIsValid", Param1 = empcode, Param2 = ty });
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["FLAG"].ToString() == "1")
                            {
                                ViewBag.IsValidRequest = 1;
                                TempData["Temp_OnBoard_EmpCode"] = empcode;
                                if (ty == "sm") { ViewBag.DaySurvey = 5; } //5 days
                                if (ty == "pd") { ViewBag.DaySurvey = 60; } //60 days

                                if (TempData["Temp_OnBoard_IsSurveyStart"] != null)
                                {
                                    if (TempData["Temp_OnBoard_IsSurveyStart"].ToString() == "1")
                                    {
                                        ViewBag.OnBoardSurveyStart = 1;
                                    }
                                }
                            }
                            else if (dt.Rows[0]["FLAG"].ToString() == "2")
                            {
                                ViewBag.Day5IsPending = 1;
                            }
                            else if (dt.Rows[0]["FLAG"].ToString() == "0")
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.IsValidRequest = 0;
                    }
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult IsUserValid(string doj)
        {
            var dt = _hRService.usp_HR(new HRViewModel() { Activity = "OnboardIsUserValid", Param1 = TempData.Peek("Temp_OnBoard_EmpCode").ToString(), Param2 = doj });
            if (dt != null && dt.Rows.Count > 0)
            {
                TempData["Temp_OnBoard_IsSurveyStart"] = "1";
                return Json("1");
            }
            else
            {
                return Json("0");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveOnboardAns(string[][] queList)
        {
            string empcode = TempData.Peek("Temp_OnBoard_EmpCode").ToString();
            string mstactivity = "";
            foreach (var item in queList)
            {
                _hRService.usp_HR(new HRViewModel()
                {
                    Activity = "SaveOnboardAns",
                    Param1 = empcode,
                    Param2 = item[0],
                    Param3 = item[1],
                    Param4 = item[2],
                    Param5 = item[3],
                    Param6 = item[4],
                    Param7 = item[5]
                });
                if (item[5].ToString() == "1")
                {
                    mstactivity = "SaveOnboardAns2";
                }
                else
                {
                    mstactivity = "SaveOnboardAns1";
                }
            }
            _hRService.usp_HR(new HRViewModel()
            {
                Activity = mstactivity,
                Param1 = empcode,
            });
            return Json("0");
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult SendLinkMail(string empcode, string type, string code, string email)
        {
            EmailSend objEmail = new();
            objEmail.To = email;
            objEmail.Subject = "Welcome to Onboarding survey";
            string strBody = "Hi, " + empcode + "<br/><br/>";

            strBody += "Your feedback is important to us! Please go with the below link and give your genuine feedback. <br/><br/>";
            strBody += "<br/><br/> Link : ";
            string Url;
            Url = Common.URL + "HR/OnBoardingSurvey/" + code + "?ty=" + type;
            strBody += "<a href='" + Url + "'>" + Url + "</a>";
            objEmail.Body = strBody;

            Thread mail = new Thread(delegate ()
            {
                objEmail.SendEmail();
            });

            mail.IsBackground = true;
            mail.Start();

            return Json("0");
        }
        #endregion
    }
}
