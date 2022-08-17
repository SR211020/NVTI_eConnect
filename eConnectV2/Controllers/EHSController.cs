using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace eConnectV2.Controllers
{
    public class EHSController : Controller
    {
        private readonly IEHSService _ehsService;
        private readonly ICommonService _commonService;
        private readonly ISaviorService _saviorService;

        public EHSController(IEHSService ehsService, ICommonService commonService, ISaviorService saviorService)
        {
            _ehsService = ehsService;
            _commonService = commonService;
            _saviorService = saviorService;
        }
        public IActionResult OHCAdd(string Dno)
        
        {
            int ReqNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;

            string strMode = "";
            if (Convert.ToString(HttpContext.Request.Query["Utp"]) != null)
            {
                strMode = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            }
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            var model = new EHSViewModel();
            model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "68" }));
            DataTable dt = _ehsService.usp_OHC(new EHSViewModel { Activity = "GET_OHC_BY_DOCNO", RequestNo = ReqNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.AadharNo = Convert.ToString(dt.Rows[0]["AADHARNO"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["EMPCODE"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["EMPNAME"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPT"]);
                model.Condition = Convert.ToString(dt.Rows[0]["PATIENT_CONDITION"]);
                model.Injury = Convert.ToString(dt.Rows[0]["INJURY"]);
                model.InjuryType = Convert.ToString(dt.Rows[0]["INJURY_TYPE"]);
                model.InjuryTypeOther = Convert.ToString(dt.Rows[0]["INJURY_TYPE_OTHER"]);
                model.NurshingStaff = Convert.ToString(dt.Rows[0]["NURSHING_STAFF"]);
                model.Quantity = Convert.ToString(dt.Rows[0]["MEDICINE_QTY"]);
                model.RestGiven = Convert.ToString(dt.Rows[0]["REST_GIVEN"]);
                model.Refer = Convert.ToString(dt.Rows[0]["REFER"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["REPORTING_DATE"]);
                model.Time1 = Convert.ToString(dt.Rows[0]["INTIME"]);
                model.Time2 = Convert.ToString(dt.Rows[0]["OUTTIME"]);
                model.Treatment = Convert.ToString(dt.Rows[0]["TREATMENT"]);
                model.Remarks = Convert.ToString(dt.Rows[0]["REMARKS"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);

                model.HistoryList = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "FILL_PATIENT_HISTORY", EmpCode = model.EmpCode, AadharNo = model.AadharNo }));
                if (strMode == "E")
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
                model.RequestNo = 0;
                model.Date1 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                model.Time1 = DateTime.Now.AddMinutes(-5).ToString("HH:mm");
                model.Time2 = DateTime.Now.ToString("HH:mm");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult OHCAdd(EHSViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_OHC";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.EmpCode = model.EmpCode.Trim().ToUpper();
                model.EmpName = model.EmpName;
                model.DeptName = model.DeptName;
                if (model.AadharNo != null)
                {
                    model.AadharNo = model.AadharNo.Trim();
                }
                else
                {
                    model.AadharNo = "";
                }
                
                model.Date1 = model.Date1;
                model.Time1 = model.Time1;
                model.Time2 = model.Time2;
                model.InjuryType = model.InjuryType;
                model.InjuryTypeOther = model.InjuryTypeOther;
                model.Condition = model.Condition;
                model.Injury = model.Injury;
                model.RestGiven = model.RestGiven;
                model.Refer = model.Refer;
                model.NurshingStaff = model.NurshingStaff;
                model.Quantity = model.Quantity;
                model.Treatment = model.Treatment;
                model.Remarks = model.Remarks;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _ehsService.usp_OHC(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);

                    if (iFlag > 0)
                    {
                        if (iFlag == 1)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultSuccessMessage;
                        }
                        if (iFlag == 2)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultUpdateMessage;
                        }
                        return RedirectToAction(nameof(OHCView));
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
            return RedirectToAction(nameof(OHCAdd));
        }
        public IActionResult OHCView()
        {
            EHSViewModel model = new();
            model.Date1 = DateTime.Now.AddMonths(-2).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.UserID = User.Identity.GetADId();

            #region OHC Status Chart
            ViewBag.DayWiseData = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_DATEWISE_COUNT", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.InjuryWiseData = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_INJURYWISE_COUNT", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.DeptWiseData = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_DEPTWISE_COUNT", Date1 = model.Date1, Date2 = model.Date2 }));
            ViewBag.ConditionWiseData = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_PATIENT_CONDITION_COUNT", Date1 = model.Date1, Date2 = model.Date2 }));

            #endregion

            return View(model);
        }
        public JsonResult GetDayWiseData(string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_DATEWISE_COUNT", Date1 = date1, Date2 = date2 }));
            return Json(data);
        }
        public JsonResult GetInjuryWiseData(string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_INJURYWISE_COUNT", Date1 = date1, Date2 = date2 }));
            return Json(data);
        }
        public JsonResult GetDeptWiseData(string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_DEPTWISE_COUNT", Date1 = date1, Date2 = date2 }));
            return Json(data);
        }
        public JsonResult GetConditionWiseData(string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "GET_PATIENT_CONDITION_COUNT", Date1 = date1, Date2 = date2 }));
            return Json(data);
        }
        public JsonResult FillOHCViewList(string date1, string date2)
        {
            var data = new List<EHSViewModel>();
            DataTable dt = _ehsService.usp_OHC(new EHSViewModel

            {
                Activity = "FILL_OHC_LIST",
                Date1 = date1,
                Date2 = date2,
            });

            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new EHSViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    AadharNo = Convert.ToString(datarow["AADHARNO"]),
                    EmpCode = Convert.ToString(datarow["EMPCODE"]),
                    EmpName = Convert.ToString(datarow["EMPNAME"]),
                    DeptName = Convert.ToString(datarow["DEPT"]),
                    Condition = Convert.ToString(datarow["PATIENT_CONDITION"]),
                    Injury = Convert.ToString(datarow["INJURY"]),
                    InjuryType = Convert.ToString(datarow["INJURY_TYPE"]),
                    InjuryTypeOther = Convert.ToString(datarow["INJURY_TYPE_OTHER"]),
                    NurshingStaff = Convert.ToString(datarow["NURSHING_STAFF"]),
                    Quantity = Convert.ToString(datarow["MEDICINE_QTY"]),
                    RestGiven = Convert.ToString(datarow["REST_GIVEN"]),
                    Refer = Convert.ToString(datarow["REFER"]),
                    Date1 = Convert.ToString(datarow["REPORTING_DATE"]),
                    Time1 = Convert.ToString(datarow["INTIME"]),
                    Time2 = Convert.ToString(datarow["OUTTIME"]),
                    Treatment = Convert.ToString(datarow["TREATMENT"]),
                    Remarks = Convert.ToString(datarow["REMARKS"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                    Param2 = Common.Encrypt(Convert.ToString("V")),
                });
            }
            return Json(data);
        }
        public JsonResult GetEmpDetails(string strEmpCode)
        {
            var model2 = new SaviorViewModel();
            var model = new EHSViewModel();
            if (strEmpCode != null)
            {
                if (strEmpCode.Trim().Length == 7 && strEmpCode.Trim().Contains("90"))
                {
                    model.Activity = "GET_EMP_DATA";
                    model.EmpCode = strEmpCode.Trim().ToUpper();
                    DataTable dt = _ehsService.usp_OHC(model);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.EmpName = Convert.ToString(dt.Rows[0]["EMPNAME"]);
                        model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                        model.AadharNo = "";
                    }
                    else
                    {
                        model.EmpName = "";
                        model.DeptName = "";
                        model.AadharNo = "";
                    }
                }
                else
                {
                    model2.Activity = "GET_EMP_DATA";
                    model2.EmpCode = strEmpCode.Trim().ToUpper();
                    DataSet ds = _saviorService.usp_Reports(model2);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        model.EmpName = Convert.ToString(ds.Tables[0].Rows[0]["EMPNAME"]);
                        model.DeptName = Convert.ToString(ds.Tables[0].Rows[0]["DEPTNAME"]);
                        model.AadharNo = Convert.ToString(ds.Tables[0].Rows[0]["AADHAAR"]);
                    }
                    else
                    {
                        model.EmpName = "";
                        model.DeptName = "";
                        model.AadharNo = "";
                    }
                }
                model.HistoryList = Common.ConvertDataTableToList<EHSViewModel>(_ehsService.usp_OHC(new EHSViewModel() { Activity = "FILL_PATIENT_HISTORY", EmpCode = strEmpCode.Trim().ToUpper(), AadharNo = model.AadharNo }));
            }
            else
            {
                model.EmpName = "";
                model.DeptName = "";
                model.AadharNo = "";
            }
            return Json(model);
        }


    }
}

