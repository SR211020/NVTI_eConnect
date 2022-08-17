using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class KaizenController : Controller
    {
        private readonly IKaizenService _kaizenService;
        private readonly ICommonService _commonService;
        public KaizenController(IKaizenService kaizenService, ICommonService commonService)
        {
            _kaizenService = kaizenService;
            _commonService = commonService;
        }

        #region Kaizen
        public IActionResult KaizenAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            KaizenViewModel model = new KaizenViewModel();
            {
                model.DDLDeptList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" }));
                model.DDLApproverList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD_ALL", UserId = User.Identity.GetADId() }));
                model.DDLMonthList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "GETMONTH" }));
                model.DDLFinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "4" }));
                model.DDLImpactList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "32" }));
                model.DDLKaizenTypeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "33" }));
                model.DDLPillarList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "34" }));
                model.HistoryList = Common.ConvertDataTableToList<KaizenViewModel>(_kaizenService.usp_Kaizen(new KaizenViewModel() { Activity = "FILL_HISTORY", DocNo = DocNo }));
            }

            DataTable dt = _kaizenService.usp_Kaizen(new KaizenViewModel { Activity = "GET_KAIZEN_BY_DOCNO", DocNo = DocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.RegNo = Convert.ToString(dt.Rows[0]["REG_NO"]);
                model.KaizenName = Convert.ToString(dt.Rows[0]["KAIZEN_NAME"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["EMPCODE"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["EMPNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.CircleNo = Convert.ToString(dt.Rows[0]["CIRCLENO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.SelectedImpact = Convert.ToString(dt.Rows[0]["IMPACT"]);
                model.FinYearCode = Convert.ToString(dt.Rows[0]["FINYEAR"]);
                model.Year = Convert.ToString(dt.Rows[0]["YR"]);
                model.Month = Convert.ToString(dt.Rows[0]["MON"]);
                model.MacArea = Convert.ToString(dt.Rows[0]["MACHINE_AREA"]);
                model.HDDone = Convert.ToString(dt.Rows[0]["HD_DONE"]);
                model.RespPillar = Convert.ToString(dt.Rows[0]["RESP_PILLAR"]);
                model.KaizenType = Convert.ToString(dt.Rows[0]["KAIZEN_TYPE"]);
                model.MPWorthy = Convert.ToString(dt.Rows[0]["MP_WORTHY"]);
                model.CostSaving = Convert.ToString(dt.Rows[0]["COST_SAVING"]);
                model.DocName = Convert.ToString(dt.Rows[0]["FILENAME"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.UserId = Convert.ToString(dt.Rows[0]["USERID"]);
                model.ApprId = Convert.ToString(dt.Rows[0]["APPRID"]);

                if (model.Status == 1 && model.UserId == User.Identity.GetADId())
                {
                    ViewBag.TagAttribute = "E";
                }
                else
                {
                    ViewBag.TagAttribute = "V";
                }
            }
            else
            {
                model.DocNo = 0;
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = User.Identity.GetEmpName();
                model.DeptCode = User.Identity.GetDeptCode();
                model.Month = DateTime.Now.ToString("MMM");
                ViewBag.TagAttribute = "E";
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult KaizenAdd(KaizenViewModel model, IFormFile DocName)
        {
            string strFilePath = Common.FilePath + @"\Kaizen\";
            string strFileName = "";
            if (ModelState.IsValid)
            {
                if (DocName != null)
                {
                    string strDocName = Path.GetFileName(DocName.FileName);
                    string strExt = strDocName.Split(".")[1];
                    strFileName = User.Identity.GetADId() + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + strExt;
                    if (model.Param1 != null)
                    {
                        string strPath = strFilePath + model.Param1;
                        if (System.IO.File.Exists(strPath))
                        {
                            System.IO.File.Delete(strPath);
                        }
                    }
                    bool save = Common.SaveFile1(DocName, strFilePath, strFileName);
                }
                else
                {
                    strFileName = model.Param1;
                }
                model.Activity = "ADD_UPD_KAIZEN";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.KaizenName = model.KaizenName;
                model.EmpCode = model.EmpCode;
                model.EmpName = model.EmpName;
                model.DeptCode = model.DeptCode;
                model.CircleNo = model.CircleNo;
                model.Plant = model.Plant;
                model.SelectedImpact = model.SelectedImpact;
                model.FinYearCode = model.FinYearCode;
                model.Year = model.Year;
                model.Month = model.Month;
                model.MacArea = model.MacArea;
                model.HDDone = model.HDDone;
                model.RespPillar = model.RespPillar;
                model.KaizenType = model.KaizenType;
                model.MPWorthy = model.MPWorthy.Trim();
                model.CostSaving = model.CostSaving;
                model.DocName = strFileName;
                model.Status = 2;
                model.ApprId = model.ApprId;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _kaizenService.usp_Kaizen(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    int iDocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                    if (iFlag > 0)
                    {
                        if (iFlag == 1)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultSuccessMessage;
                        }
                        else if (iFlag == 2)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultUpdateMessage;
                        }

                        var mail = Task.Run(() => SendKaizenMail(iDocNo, model.KaizenName, model.EmpCode, model.EmpName, model.DeptName, "REQUESTOR", model.ApprId, "Posted by User"));
                        return RedirectToAction(nameof(KaizenView));
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(KaizenAdd));
        }

        public JsonResult BindApprName(string dept)
        {
            var data = _commonService.usp_Master(new CommonViewModel() { Activity = "GET_HOD", Plant = User.Identity.GetPlant(), Param1 = dept });
            var result = new KaizenViewModel
            {
                ApprId = data.Rows[0]["HOD_ADID"].ToString(),
                ApprName = data.Rows[0]["HOD_NAME"].ToString(),
                DeptName = data.Rows[0]["DEPT_NAME"].ToString(),
            };
            return Json(result);
        }

        public IActionResult KaizenView()
        {
            var model = new KaizenViewModel()
            {
                StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "31" })),
                Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy"),
                Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy")
            };
            return View(model);
        }
        public JsonResult FilterKaizenView(string date1, string date2, int status)
        {
            var data = new List<KaizenViewModel>();
            DataTable dt = _kaizenService.usp_Kaizen(new KaizenViewModel { Activity = "FILL_KAIZEN_LIST", Date1 = date1, Date2 = date2, Status = status, UserId = User.Identity.GetADId() });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new KaizenViewModel
                {
                    DocNo = Convert.ToInt32(datarow["DOCNO"]),
                    RegNo = Convert.ToString(datarow["REG_NO"]),
                    KaizenName = Convert.ToString(datarow["KAIZEN_NAME"]),
                    EmpCode = Convert.ToString(datarow["EMPCODE"]),
                    EmpName = Convert.ToString(datarow["EMPNAME"]),
                    DeptCode = Convert.ToString(datarow["DEPTCODE"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    CircleNo = Convert.ToString(datarow["CIRCLENO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Impact = Convert.ToString(datarow["IMPACT"]),
                    FinYearCode = Convert.ToString(datarow["FINYEAR"]),
                    Year = Convert.ToString(datarow["YR"]),
                    Month = Convert.ToString(datarow["MON"]),
                    MacArea = Convert.ToString(datarow["MACHINE_AREA"]),
                    HDDone = Convert.ToString(datarow["HD_DONE"]),
                    RespPillar = Convert.ToString(datarow["RESP_PILLAR"]),
                    KaizenType = Convert.ToString(datarow["KAIZEN_TYPE"]),
                    MPWorthy = Convert.ToString(datarow["MP_WORTHY"]),
                    CostSaving = Convert.ToString(datarow["COST_SAVING"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUS_DESCR"]),
                    UserId = Convert.ToString(datarow["USERID"]),
                    UserName = Convert.ToString(datarow["USERNAME"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    ApprId = Convert.ToString(datarow["APPRID"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Date1 = Convert.ToString(datarow["APPRDATE"]),
                    UserType = Convert.ToString(datarow["USERTYPE"]),

                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"])),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    Param3 = Common.Encrypt(Convert.ToString(datarow["USERTYPE"]))
                });
            }
            return Json(data);
        }

        public JsonResult KaizenCancel(int docNo)
        {
            _kaizenService.usp_Kaizen(new KaizenViewModel() { Activity = "KAIZEN_CANCEL", DocNo = docNo, UserId = User.Identity.GetADId() });
            return Json("OK");
        }

        public IActionResult KaizenApprAdd()
        {
            int iDocNo = Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"]));
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            string strUserId = "";
            string strUserType = "";
            if (Convert.ToString(HttpContext.Request.Query["Uid"]) != null)
            {
                strUserId = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Uid"]));
            }
            if (Convert.ToString(HttpContext.Request.Query["Utp"]) != null)
            {
                strUserType = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            }

            var model = new KaizenViewModel();
            model.HistoryList = Common.ConvertDataTableToList<KaizenViewModel>(_kaizenService.usp_Kaizen(new KaizenViewModel() { Activity = "FILL_HISTORY", DocNo = iDocNo }));
            DataTable dt = _kaizenService.usp_Kaizen(new KaizenViewModel { Activity = "GET_KAIZEN_BY_DOCNO", DocNo = iDocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.RegNo = Convert.ToString(dt.Rows[0]["REG_NO"]);
                model.KaizenName = Convert.ToString(dt.Rows[0]["KAIZEN_NAME"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["EMPCODE"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["EMPNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.CircleNo = Convert.ToString(dt.Rows[0]["CIRCLENO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.SelectedImpact = Convert.ToString(dt.Rows[0]["IMPACT"]);
                model.FinYearCode = Convert.ToString(dt.Rows[0]["FINYEAR"]);
                model.Year = Convert.ToString(dt.Rows[0]["YR"]);
                model.Month = Convert.ToString(dt.Rows[0]["MON"]);
                model.MacArea = Convert.ToString(dt.Rows[0]["MACHINE_AREA"]);
                model.HDDone = Convert.ToString(dt.Rows[0]["HD_DONE"]);
                model.RespPillar = Convert.ToString(dt.Rows[0]["RESP_PILLAR"]);
                model.KaizenType = Convert.ToString(dt.Rows[0]["KAIZEN_TYPE"]);
                model.MPWorthy = Convert.ToString(dt.Rows[0]["MP_WORTHY"]);
                model.CostSaving = Convert.ToString(dt.Rows[0]["COST_SAVING"]);
                model.DocName = Convert.ToString(dt.Rows[0]["FILENAME"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.UserId = Convert.ToString(dt.Rows[0]["USERID"]);
                string strApprId = Convert.ToString(dt.Rows[0]["APPRID"]);
                model.ApprName = Convert.ToString(dt.Rows[0]["APPRNAME"]);
                model.ApprId = strUserId;          //From Querystring

                if (model.Status == 2 && strApprId == strUserId && strUserType == "HOD")
                {
                    ViewBag.TagAttribute = "E";
                }
                else
                {
                    ViewBag.TagAttribute = "V";
                }
            }
            else
            {
                ViewBag.TagAttribute = "V";     //Default Disabled Everything
            }
            ViewBag.Login = User.Identity.IsLoggedIn() == true ? "Y" : "N";
            return View(model);
        }

        [HttpPost]
        public IActionResult KaizenApprAdd(KaizenViewModel model)
        {
            string strStatusDescr = "";
            if (model.Status == 3)
            {
                strStatusDescr = "Approved";
            }
            else if (model.Status == 1)
            {
                strStatusDescr = "Review Back";
            }
            else
            {
                strStatusDescr = "Rejected";
            }
            if (ModelState.IsValid)
            {
                model.Activity = "APPROVE_KAIZEN";
                model.DocNo = Convert.ToInt32(model.DocNo);
                model.Status = model.Status;
                model.StatusDescr = strStatusDescr;
                model.Remarks = model.Remarks;
                if (User.Identity.IsLoggedIn())
                {
                    model.UserId = User.Identity.GetADId();
                }
                else
                {
                    model.UserId = model.ApprId;
                }
                DataTable dt = _kaizenService.usp_Kaizen(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag > 0)
                    {
                        if (iFlag == 1)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultSuccessMessage;
                            var mail = Task.Run(() => SendKaizenMail(model.DocNo, model.KaizenName, model.EmpCode, model.EmpName, model.DeptName, "HOD", model.UserId, strStatusDescr));
                        }
                        if (User.Identity.IsLoggedIn())
                        {
                            return RedirectToAction(nameof(KaizenView));
                        }
                        else
                        {
                            return RedirectToAction("Index", "Account");
                        }
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            if (User.Identity.IsLoggedIn())
            {
                return RedirectToAction(nameof(KaizenView));
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KaizenReports()
        {
            var model = new KaizenViewModel()
            {
                StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "31" })),
            };
            DateTime dtToday = DateTime.Now;
            if (dtToday.Month == 1 || dtToday.Month == 2 || dtToday.Month == 3)
            {
                model.Date3 = model.Date1 = new DateTime(dtToday.Year - 1, 4, 1).ToString("dd'-'MMM'-'yyyy");
            }
            else
            {
                model.Date3 = model.Date1 = new DateTime(dtToday.Year, 4, 1).ToString("dd'-'MMM'-'yyyy");
            }
            model.Date4 = model.Date2 = dtToday.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }

        public JsonResult GetKaizenList(string date1, string date2, int status)
        {
            var data = new List<KaizenViewModel>();
            DataTable dt = _kaizenService.usp_Kaizen(new KaizenViewModel { Activity = "GET_ALL_KAIZEN", DeptCode = User.Identity.GetDeptCode(), Date1 = date1, Date2 = date2, Status = status });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new KaizenViewModel
                {
                    DocNo = Convert.ToInt32(datarow["DOCNO"]),
                    RegNo = Convert.ToString(datarow["REG_NO"]),
                    KaizenName = Convert.ToString(datarow["KAIZEN_NAME"]),
                    EmpCode = Convert.ToString(datarow["EMPCODE"]),
                    EmpName = Convert.ToString(datarow["EMPNAME"]),
                    DeptCode = Convert.ToString(datarow["DEPTCODE"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    CircleNo = Convert.ToString(datarow["CIRCLENO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Impact = Convert.ToString(datarow["IMPACT"]),
                    FinYearCode = Convert.ToString(datarow["FINYEAR"]),
                    Year = Convert.ToString(datarow["YR"]),
                    Month = Convert.ToString(datarow["MON"]),
                    MacArea = Convert.ToString(datarow["MACHINE_AREA"]),
                    HDDone = Convert.ToString(datarow["HD_DONE"]),
                    RespPillar = Convert.ToString(datarow["RESP_PILLAR"]),
                    KaizenType = Convert.ToString(datarow["KAIZEN_TYPE"]),
                    MPWorthy = Convert.ToString(datarow["MP_WORTHY"]),
                    CostSaving = Convert.ToString(datarow["COST_SAVING"]),
                    DocName = Convert.ToString(datarow["FILENAME"]),
                    StatusDescr = Convert.ToString(datarow["STATUS_DESCR"]),
                    UserId = Convert.ToString(datarow["USERID"]),
                    UserName = Convert.ToString(datarow["USERNAME"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    ApprId = Convert.ToString(datarow["APPRID"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Date1 = Convert.ToString(datarow["APPRDATE"]),

                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"]))
                });
            }
            return Json(data);
        }

        public JsonResult GetKaizenDeptData(string date3, string date4, int status)
        {
            var model = new List<KaizenViewModel>();
            DataTable dt = _kaizenService.usp_Kaizen(new KaizenViewModel() { Activity = "GET_KAIZEN_DASHBOARD_DEPT", Date1 = date3, Date2 = date4, Status = status });
            foreach (DataRow dataRow in dt.Rows)
            {
                model.Add(
                    new KaizenViewModel
                    {
                        DeptName = dataRow["DEPTNAME"].ToString().Trim(),
                        Total = Convert.ToInt32(dataRow["TOTAL"])
                    });
            }
            var data = Common.ConvertDataTableToList<KaizenViewModel>(dt);
            return Json(data);
        }

        public FileResult DownloadKaizenFile(string fileName)
        {
            string path = Common.FilePath + @"\Kaizen\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        protected void SendKaizenMail(int iDocNo, string strKaizenName, string strEmpCode, string strEmpName, string strDept, string strUserType, string strApprId, string strStatus)
        {
            DataTable dt = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strApprId });
            string strEmailId;
            if (dt != null && dt.Rows.Count > 0)
            {
                strEmailId = dt.Rows[0]["EMAILID"].ToString();
            }
            else
            {
                return;
            }

            string strSubject = "";
            if (strUserType == "REQUESTOR")
            {
                strSubject = "Kaizen for Approval. ";
            }
            else if (strUserType == "HOD")
            {
                strSubject = "Kaizen Status from HOD. ";
            }

            //strEmailId = "pramodkumar.yadav@nvtpower.com";
            EmailSend objEmail = new();
            objEmail.To = strEmailId;

            objEmail.Subject = strSubject + "Kaizen No : " + iDocNo;
            string strBody = "<b>Kaizen Name : </b>" + strKaizenName + "<br/><br/>";
            strBody += "<b>Emp Name : </b>" + strEmpName + " (" + strEmpCode + ")" + "<br/><br/>";
            strBody += "<b>Department : </b>" + strDept + "<br/><br/>";
            strBody += "<b>Status : </b>" + strStatus + "<br/><br/>";

            if (strUserType == "REQUESTOR")
            {
                string Url = Common.URL + "Kaizen/KaizenApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iDocNo)) + "&Uid=" + Common.Encrypt(Convert.ToString(strApprId)) + "&Utp=" + Common.Encrypt("HOD");

                strBody += "<b>For Approve/Reject, click on below link </b><br/><br/>";
                strBody += "<a href='" + Url + "'>" + Url + "</a>";
            }

            objEmail.Body = strBody;
            objEmail.SendEmail();
        }

        #endregion

    }
}
