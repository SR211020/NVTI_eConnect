using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace eConnectV2.Controllers
{
    public class QualityController : Controller
    {
        private readonly IQualityService _qualityService;
        private readonly ICommonService _commonService;

        public QualityController(IQualityService qualityService, ICommonService commonService)
        {
            _qualityService = qualityService;
            _commonService = commonService;
        }

        ///////// DCN System /////////
        public IActionResult DCNAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            QualityViewModel model = new QualityViewModel();
            if (DocNo > 0)
            {
                model = FillDCNByReqNo(DocNo);
                model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD_ALL", UserId = User.Identity.GetADId() }));
                if (model.Status == 1)
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
                model.TDate = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = User.Identity.GetEmpName();
                model.DeptCode = User.Identity.GetDeptCode();
                model.DeptName = User.Identity.GetDeptName();
                model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD_ALL", UserId = User.Identity.GetADId() }));
                ViewBag.TagAttribute = "E";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DCNAdd(QualityViewModel model, IFormFile File_Name)
        {
            string strFilePath = Common.FilePath + @"\DCN\";
            string strFile = "";
            if (ModelState.IsValid)
            {
                if (File_Name != null)
                {
                    string fileName = Path.GetFileName(File_Name.FileName);
                    string actext = fileName.Split(".")[1];
                    strFile = User.Identity.GetADId() + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "." + actext;
                    if (model.Param1 != null)
                    {
                        string strPath = strFilePath + model.Param1;
                        if (System.IO.File.Exists(strPath))
                        {
                            System.IO.File.Delete(strPath);
                        }
                    }
                    bool save = Common.SaveFile1(File_Name, strFilePath, strFile);
                }
                model.Activity = "ADD_DCN";
                model.RequestNo = model.RequestNo > 0 ? Convert.ToInt32(model.RequestNo) : 0;
                model.EmpCode = model.EmpCode;
                model.DeptCode = model.DeptCode;
                model.Type = model.Type;
                model.Reason = model.Reason;
                model.Date1 = model.Date1;
                model.DocNo = model.DocNo;
                model.DocName = model.DocName;
                model.DocChanges = model.DocChanges;
                model.ChangeRequireIn = model.ChangeRequireIn;
                model.ChangeDocNo = model.ChangeDocNo;
                model.ChangeDocName = model.ChangeDocName;
                model.DeptHOD = model.DeptHOD;
                model.File_Name = strFile != "" ? strFile : model.Param1;
                model.Status = 2;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _qualityService.usp_DCN(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    Int32 iReqNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                    if (iFlag > 0)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        var mail = Task.Run(() => SendMail_DCN(model.RequestNo, User.Identity.GetEmpName(), model.Type, model.DeptHOD, "HOD", "Post"));
                        return RedirectToAction(nameof(DCNView));
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
            return RedirectToAction(nameof(DCNAdd));
        }

        public FileResult DownloadDCNFile(string fileName)
        {
            string path = Common.FilePath + @"\DCN\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        protected QualityViewModel FillDCNByReqNo(int DocNo)
        {
            QualityViewModel model = new QualityViewModel();
            DataTable dt = _qualityService.usp_DCN(new QualityViewModel { Activity = "GET_DCN_BY_DOCNO", RequestNo = DocNo });
            model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_DCN(new QualityViewModel() { Activity = "FILL_DCN_HISTORY", RequestNo = DocNo }));
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.TDate = Convert.ToString(dt.Rows[0]["TDATE"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["REQID"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.Type = Convert.ToString(dt.Rows[0]["REQ_TYPE"]);
                model.Reason = Convert.ToString(dt.Rows[0]["REASON"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["EFFECTIVE_FROM"]);
                model.DocNo = Convert.ToString(dt.Rows[0]["DOCNO"]);
                model.DocName = Convert.ToString(dt.Rows[0]["DOCNAME"]);
                model.DocChanges = Convert.ToString(dt.Rows[0]["DOC_CHANGES"]);
                model.ChangeRequireIn = Convert.ToString(dt.Rows[0]["CHANGE_REQUIRED_IN"]);
                model.ChangeDocNo = Convert.ToString(dt.Rows[0]["CHANGE_DOCNO"]);
                model.ChangeDocName = Convert.ToString(dt.Rows[0]["CHANGE_DOCNAME"]);
                model.File_Name = Convert.ToString(dt.Rows[0]["FILENAME"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.ApprID = Convert.ToString(dt.Rows[0]["PENDING_WITH"]);
                model.DeptHOD = Convert.ToString(dt.Rows[0]["DEPT_HOD"]);
                model.HODName = Convert.ToString(dt.Rows[0]["DEPT_HOD_NAME"]);
                model.DCC = Convert.ToString(dt.Rows[0]["DCC_INCHARGE"]);
                model.DCCDocNo = Convert.ToString(dt.Rows[0]["DCC_DOCNO"]);
                model.DCCDocName = Convert.ToString(dt.Rows[0]["DCC_DOC_NAME"]);
                model.DCCReqType = Convert.ToString(dt.Rows[0]["DCC_REQ_TYPE"]);
                model.Date2 = Convert.ToString(dt.Rows[0]["DCC_DONE_DATE"]);
            }
            return model;
        }
        public IActionResult DCNApprAdd(string Dno, string Utp)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            string ApprType = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            QualityViewModel model = new QualityViewModel();
            if (DocNo > 0)
            {
                model = FillDCNByReqNo(DocNo);
                model.ApprType = ApprType;
                if (model.Status == 2 && ApprType == "HOD")
                {
                    ViewBag.TagAttribute = "E";
                }
                else if (model.Status == 3 && ApprType == "DCC")
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
                ViewBag.TagAttribute = "V";
            }
            if (User.Identity.IsLoggedIn())
            {
                ViewBag.Login = "Y";
            }
            else
            {
                ViewBag.Login = "N";
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult DCNApprAdd(QualityViewModel model)
        {
            string strNextUserId = "";
            string strNextUserType = "";
            int iStatus = 0;
            string strStatusDescr = "";
            if (ModelState.IsValid)
            {
                if (model.ApprStatus == "Approve")
                {
                    if (model.ApprType == "HOD")    //Pending With DCC
                    {
                        iStatus = 3;
                        strNextUserId = "";
                        strNextUserType = "DCC";
                    }
                    if (model.ApprType == "DCC")    //Finish
                    {
                        iStatus = 4;
                        strNextUserId = model.UserID;
                        strNextUserType = "REQUESTOR";
                    }
                    strStatusDescr = "Approved";
                }
                else if (model.ApprStatus == "ReviewBack")    //Review Back
                {
                    iStatus = 1;
                    strStatusDescr = "Review Back";
                    strNextUserId = model.UserID;
                    strNextUserType = "REQUESTOR";
                }
                else    //Rejected
                {
                    iStatus = 5;
                    strStatusDescr = "Rejected";
                    strNextUserId = model.UserID;
                    strNextUserType = "REQUESTOR";
                }

                model.Activity = "APPROVE_DCN";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.DCCDocNo = model.DCCDocNo;
                model.DCCDocName = model.DCCDocName;
                model.DCCReqType = model.DCCReqType;
                model.Date2 = model.Date2;
                model.DCCStatus = model.DCCStatus;

                model.Status = iStatus;
                model.StatusDescr = strStatusDescr;
                model.Remarks = model.Remarks;
                model.ApprType = model.ApprType;
                if (User.Identity.IsLoggedIn())
                {
                    model.UserID = User.Identity.GetADId();
                }
                else
                {
                    model.UserID = model.ApprID;
                }
                DataTable dt = _qualityService.usp_DCN(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["FLAG"]) == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        var mail = Task.Run(() => SendMail_DCN(model.RequestNo, model.EmpName, model.Type, strNextUserId, strNextUserType, strStatusDescr));
                        return RedirectToAction(nameof(DCNView));
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
            return RedirectToAction(nameof(DCNApprAdd));
        }

        public IActionResult DCNView()
        {
            QualityViewModel model = new QualityViewModel();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "37" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }
        public JsonResult FillDCNViewList(int status, string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_DCN(new QualityViewModel
            {
                Activity = "FILL_DCN_LIST_APPRS",
                Status = status,
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    data.Add(new QualityViewModel
                    {
                        RequestNo = Convert.ToInt32(datarow["REQNO"]),
                        Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                        EmpCode = Convert.ToString(datarow["REQID"]),
                        EmpName = Convert.ToString(datarow["REQNAME"]),
                        DeptName = Convert.ToString(datarow["DEPTNAME"]),
                        Type = Convert.ToString(datarow["REQ_TYPE"]),
                        Reason = Convert.ToString(datarow["REASON"]),
                        Date1 = Convert.ToString(datarow["EFFECTIVE_FROM"]),
                        DocNo = Convert.ToString(datarow["DOCNO"]),
                        DocName = Convert.ToString(datarow["DOCNAME"]),
                        DocChanges = Convert.ToString(datarow["DOC_CHANGES"]),
                        ChangeRequireIn = Convert.ToString(datarow["CHANGE_REQUIRED_IN"]),
                        ChangeDocNo = Convert.ToString(datarow["CHANGE_DOCNO"]),
                        ChangeDocName = Convert.ToString(datarow["CHANGE_DOCNAME"]),

                        DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                        Date2 = Convert.ToString(datarow["DEPT_HOD_DATE"]),
                        DCC = Convert.ToString(datarow["DCC_INCHARGE"]),
                        Date3 = Convert.ToString(datarow["DCC_INCHARGE_DATE"]),
                        DCCDocNo = Convert.ToString(datarow["DCC_DOCNO"]),
                        DCCDocName = Convert.ToString(datarow["DCC_DOC_NAME"]),
                        Date4 = Convert.ToString(datarow["DCC_DONE_DATE"]),
                        DCCReqType = Convert.ToString(datarow["DCC_REQ_TYPE"]),
                        DCCStatus = Convert.ToString(datarow["DCC_DOC_DESTROY"]),

                        Status = Convert.ToInt32(datarow["STATUS"]),
                        StatusDescr = Convert.ToString(datarow["STATUS_DESCR"]),
                        UserID = Convert.ToString(datarow["USERID"]),
                        TDate = Convert.ToString(datarow["TDATE"]),
                        ApprType = Convert.ToString(datarow["APPRTYPE"]),
                        Param2 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"])),
                    });
                }
            }
            return Json(data);
        }

        public IActionResult DCNReport()
        {
            QualityViewModel model = new QualityViewModel();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "37" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }
        public JsonResult FillDCNReport(string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_DCN(new QualityViewModel
            {
                Activity = "FILL_REPORT_DCC",
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    data.Add(new QualityViewModel
                    {
                        RequestNo = Convert.ToInt32(datarow["REQNO"]),
                        Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                        EmpCode = Convert.ToString(datarow["REQID"]),
                        EmpName = Convert.ToString(datarow["REQNAME"]),
                        DeptName = Convert.ToString(datarow["DEPTNAME"]),
                        Type = Convert.ToString(datarow["REQ_TYPE"]),
                        Reason = Convert.ToString(datarow["REASON"]),
                        Date1 = Convert.ToString(datarow["EFFECTIVE_FROM"]),
                        DocNo = Convert.ToString(datarow["DOCNO"]),
                        DocName = Convert.ToString(datarow["DOCNAME"]),
                        DocChanges = Convert.ToString(datarow["DOC_CHANGES"]),
                        ChangeRequireIn = Convert.ToString(datarow["CHANGE_REQUIRED_IN"]),
                        ChangeDocNo = Convert.ToString(datarow["CHANGE_DOCNO"]),
                        ChangeDocName = Convert.ToString(datarow["CHANGE_DOCNAME"]),
                        DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                        Date2 = Convert.ToString(datarow["DEPT_HOD_DATE"]),
                        DCC = Convert.ToString(datarow["DCC_INCHARGE"]),
                        Date3 = Convert.ToString(datarow["DCC_INCHARGE_DATE"]),
                        DCCDocNo = Convert.ToString(datarow["DCC_DOCNO"]),
                        DCCDocName = Convert.ToString(datarow["DCC_DOC_NAME"]),
                        Date4 = Convert.ToString(datarow["DCC_DONE_DATE"]),
                        DCCReqType = Convert.ToString(datarow["DCC_REQ_TYPE"]),
                        DCCStatus = Convert.ToString(datarow["DCC_DOC_DESTROY"]),
                        StatusDescr = Convert.ToString(datarow["STATUS_DESCR"]),
                        UserID = Convert.ToString(datarow["USERID"]),
                        TDate = Convert.ToString(datarow["TDATE"]),
                        Param2 = Common.Encrypt("View")
                    });
                }
            }
            return Json(data);
        }

        protected void SendMail_DCN(int iReqNo, string strRequestorName, string strReqType, string strNextUserId, string strNextUserType, string strStatus)
        {
            string strEmailId = "";
            string strSubject = "";
            if (strNextUserType == "REQUESTOR")
            {
                DataTable dt = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strNextUserId });
                if (dt != null && dt.Rows.Count > 0)
                {
                    strEmailId = dt.Rows[0]["EMAILID"].ToString();
                }
                strSubject = "DCN Request Status.";
            }
            if (strNextUserType == "HOD")
            {
                DataTable dt = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strNextUserId });
                if (dt != null && dt.Rows.Count > 0)
                {
                    strEmailId = dt.Rows[0]["EMAILID"].ToString();
                }
                strSubject = "DCN Request for Approval.";
            }
            if (strNextUserType == "DCC")
            {
                DataTable dt = _qualityService.usp_DCN(new QualityViewModel { Activity = "GET_DCC_EMAIL" });
                if (dt != null && dt.Rows.Count > 0)
                {
                    strEmailId = dt.Rows[0]["EMAILID"].ToString();
                }
                strSubject = "DCN Request for Approval.";
            }
            //strEmailId = "pramodkumar.yadav@nvtpower.com";
            if (strEmailId == "")
            {
                return;
            }
            EmailSend objEmail = new();
            objEmail.To = strEmailId;

            objEmail.Subject = strSubject + " Request No : " + iReqNo;
            string strBody = "<b>Requestor Name : </b>" + strRequestorName + "<br/><br/>";
            strBody += "<b>Request Type : </b>" + strReqType + "<br/><br/>";
            if (strNextUserType == "REQUESTOR")
            {
                strBody += "<b>Status : </b>" + strStatus + "<br/><br/>";
            }

            if (strNextUserType == "HOD")
            {
                string Url;
                Url = Common.URL + "Quality/DCNApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iReqNo)) + "&Utp=" + Common.Encrypt(Convert.ToString(strNextUserType));
                //Url = "http://localhost:1768/" + "Quality/DCNApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iReqNo)) + "&Utp=" + Common.Encrypt(Convert.ToString(strNextUserType));
                strBody += "<b>For Approve/Reject/Review Back to employee click on below link </b><br/><br/>";
                strBody += "<a href='" + Url + "'>" + Url + "</a>";
            }

            if (strNextUserType == "DCC")
            {
                strBody += "<b>Kindly Login on </b>" + " <a href='" + Common.URL + "'>" + "eConnect" + "</a>" + " to Approve/Reject/Review Back the request";
            }
            objEmail.Body = strBody;
            objEmail.SendEmail();
        }
        ///////// End DCN System /////////

        ///////// Deviation System /////////

        public IActionResult DeviationAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            QualityViewModel model = new QualityViewModel();
            if (DocNo > 0)
            {
                model = FillDeviationByReqNo(DocNo);
                if (model.Status == 1)
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
                model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "2" }));
                model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "40" }));
                model.RequestNo = 0;
                model.TDate = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = User.Identity.GetEmpName();
                model.ContactNo = User.Identity.GetContactNo();
                model.DeptCode = User.Identity.GetDeptCode();
                model.DeptName = User.Identity.GetDeptName();
                ViewBag.TagAttribute = "E";
            }
            return View(model);
        }

        public JsonResult GetDeviationApprovers(string plant)
        {
            var data = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_Deviation(new QualityViewModel() { Activity = "FILL_DEVIATION_APPR_LIST", Plant = plant }));
            return Json(data);
        }

        public JsonResult ViewDeviationApprovers(string plant)
        {
            var data = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_Deviation(new QualityViewModel() { Activity = "VIEW_DEVIATION_APPR_LIST", Plant = plant }));
            return Json(data);
        }

        [HttpPost]
        public IActionResult DeviationAdd(QualityViewModel model, IFormFile DocName, string[] Departments)
        {
            string filepath = Common.FilePath + @"\Deviation\";
            string file = "";
            if (ModelState.IsValid)
            {
                if (Departments.Length == 0)
                {
                    model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "2" }));
                    model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "40" }));
                    ViewBag.TagAttribute = "E";
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = "Approver Hierarchy is not available";
                    return View(model);
                }
                if (DocName != null)
                {
                    string fileName = Path.GetFileName(DocName.FileName);
                    string actext = fileName.Split(".")[1];
                    file = User.Identity.GetADId() + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "." + actext;
                    if (model.Param2 != null)
                    {
                        string path = filepath + model.Param2;
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    bool save = Common.SaveFile1(DocName, filepath, file);
                }
                model.Activity = "ADD_DEVIATION";
                model.RequestNo = model.RequestNo > 0 ? Convert.ToInt32(model.RequestNo) : 0;
                model.EmpCode = model.EmpCode;
                model.DeptCode = model.DeptCode;
                model.Plant = model.Plant;
                model.ModelName = model.ModelName.ToUpper();
                model.ContactNo = model.ContactNo;
                model.PartNo = model.PartNo.ToUpper();
                model.Qty = model.Qty;
                model.Customer = model.Customer;
                model.Date1 = model.Date1;
                model.Type = model.Type;
                model.DocName = file != "" ? file : model.Param2;
                model.NatureOfIssue = model.NatureOfIssue;
                model.ActionProposed = model.ActionProposed;
                model.RootCause = model.RootCause;
                model.CorrectiveAction = model.CorrectiveAction;
                model.Status = 2;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _qualityService.usp_Deviation(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    Int32 iReqNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                    if (iFlag > 0)
                    {
                        InsertDeviationApprovalHistory(iReqNo, Departments);
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        var mail = Task.Run(() => SendMail_Deviation(iReqNo, model.EmpName, model.Type, model.NatureOfIssue, "NEXT_APPROVER", "Post"));
                        return RedirectToAction(nameof(DeviationView));
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
            return RedirectToAction(nameof(DeviationAdd));
        }

        protected void InsertDeviationApprovalHistory(int DocNo, string[] Departments)
        {
            QualityViewModel model = new QualityViewModel();
            model.Activity = "ADD_DEVIATION_APPROVERS";
            model.RequestNo = DocNo;
            foreach (string strDept in Departments)
            {
                model.ApprType = strDept;
                _qualityService.usp_Deviation(model);
            }
        }

        public FileResult DownloadDeviationkFile(string fileName)
        {
            string path = Common.FilePath + @"\Deviation\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        protected QualityViewModel FillDeviationByReqNo(int DocNo)
        {
            QualityViewModel model = new QualityViewModel();
            DataTable dt = _qualityService.usp_Deviation(new QualityViewModel { Activity = "GET_DEVIATION_BY_DOCNO", RequestNo = DocNo });
            model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "2" }));
            model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "40" }));
            model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_Deviation(new QualityViewModel() { Activity = "FILL_DEVIATION_HISTORY", RequestNo = DocNo }));
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["REQID"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.ContactNo = Convert.ToString(dt.Rows[0]["CONTACTNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.ModelName = Convert.ToString(dt.Rows[0]["MODEL"]);
                model.PartNo = Convert.ToString(dt.Rows[0]["PARTNO"]);
                model.Qty = Convert.ToString(dt.Rows[0]["QTY"]);
                model.Customer = Convert.ToString(dt.Rows[0]["CUSTOMER"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["VALIDITY"]);
                model.Type = Convert.ToString(dt.Rows[0]["DEVIATION_TYPE"]);
                model.DocName = Convert.ToString(dt.Rows[0]["FILENAME"]);
                model.NatureOfIssue = Convert.ToString(dt.Rows[0]["NATURE_OF_ISSUE"]);
                model.ActionProposed = Convert.ToString(dt.Rows[0]["ACTION_PROPOSED"]);
                model.RootCause = Convert.ToString(dt.Rows[0]["ROOT_CAUSE"]);
                model.CorrectiveAction = Convert.ToString(dt.Rows[0]["CORRECTIVE_ACTION"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);
                model.TDate = Convert.ToString(dt.Rows[0]["TDATE"]);
                model.ApprType = Convert.ToString(dt.Rows[0]["PENDING_WITH"]);
            }
            return model;
        }

        public IActionResult DeviationView()
        {
            QualityViewModel model = new QualityViewModel();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "39" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }

        public JsonResult FillDeviationViewList(int status, string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_Deviation(new QualityViewModel
            {
                Activity = "FILL_DEVIATION_LIST_APPRS",
                Status = status,
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    data.Add(new QualityViewModel
                    {
                        RequestNo = Convert.ToInt32(datarow["REQNO"]),
                        EmpCode = Convert.ToString(datarow["REQID"]),
                        EmpName = Convert.ToString(datarow["REQNAME"]),
                        DeptName = Convert.ToString(datarow["DEPTNAME"]),
                        ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                        Plant = Convert.ToString(datarow["PLANT"]),
                        ModelName = Convert.ToString(datarow["MODEL"]),
                        PartNo = Convert.ToString(datarow["PARTNO"]),
                        Qty = Convert.ToString(datarow["QTY"]),
                        Customer = Convert.ToString(datarow["CUSTOMER"]),
                        Date1 = Convert.ToString(datarow["VALIDITY"]),
                        Type = Convert.ToString(datarow["DEVIATION_TYPE"]),
                        NatureOfIssue = Convert.ToString(datarow["NATURE_OF_ISSUE"]),
                        ActionProposed = Convert.ToString(datarow["ACTION_PROPOSED"]),
                        RootCause = Convert.ToString(datarow["ROOT_CAUSE"]),
                        CorrectiveAction = Convert.ToString(datarow["CORRECTIVE_ACTION"]),
                        Status = Convert.ToInt32(datarow["STATUS"]),
                        StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                        UserID = Convert.ToString(datarow["USERID"]),
                        TDate = Convert.ToString(datarow["TDATE"]),
                        UserType = Convert.ToString(datarow["PENDING_WITH"]),
                        ApprType = Convert.ToString(datarow["APPRTYPE"]),
                        Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                        Param2 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"])),
                    });
                }
            }
            return Json(data);
        }

        public IActionResult DeviationApprAdd(string Dno, string Utp)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            string ApprType = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            QualityViewModel model = new QualityViewModel();
            if (DocNo > 0)
            {
                model = FillDeviationByReqNo(DocNo);
                ///// CHECK CURRENT APPROVER /////
                model.Activity = "CHECK_DEVIATION_APPR";
                model.RequestNo = DocNo;
                model.ApprType = ApprType;
                model.UserID = User.Identity.GetADId();
                DataTable dt = _qualityService.usp_Deviation(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        ViewBag.TagAttribute = "E";
                    }
                    else
                    {
                        ViewBag.TagAttribute = "V";
                    }
                }
                if (ApprType == "RptView")   //For Redirect On Report List Page
                {
                    ViewBag.TagAttribute = "RptView";
                }
            }
            else
            {
                ViewBag.TagAttribute = "V";
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult DeviationApprAdd(QualityViewModel model)
        {
            Int32 iStatus = 0;
            string strNextUserType = "";
            string strStatusDescr = "";
            if (ModelState.IsValid)
            {
                if (model.ApprStatus.ToUpper() == "APPROVE")
                {
                    if (model.ApprType == "QA-HEAD")
                    {
                        iStatus = 3;
                        strNextUserType = "REQUESTOR";
                    }
                    else
                    {
                        iStatus = 2;
                        strNextUserType = "NEXT_APPROVER";
                    }
                    strStatusDescr = "Approved";
                }
                else
                {
                    iStatus = 4;
                    strNextUserType = "REQUESTOR";
                    strStatusDescr = "Rejected";
                }
                model.Activity = "APPROVE_DEVIATION";
                model.RequestNo = model.RequestNo > 0 ? Convert.ToInt32(model.RequestNo) : 0;
                model.ApprType = model.ApprType;
                model.Status = iStatus;
                model.StatusDescr = strStatusDescr;
                model.Remarks = model.Remarks;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _qualityService.usp_Deviation(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        var mail = Task.Run(() => SendMail_Deviation(model.RequestNo, model.EmpName, model.Type, model.NatureOfIssue, strNextUserType, strStatusDescr));
                        return RedirectToAction(nameof(DeviationView));
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
            return RedirectToAction(nameof(DeviationApprAdd));
        }

        public IActionResult DeviationReport()
        {
            QualityViewModel model = new QualityViewModel();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "39" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }
        public JsonResult FillDeviationReport(string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_Deviation(new QualityViewModel
            {
                Activity = "FILL_REPORT_DEVIATION",
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    data.Add(new QualityViewModel
                    {
                        RequestNo = Convert.ToInt32(datarow["REQNO"]),
                        EmpCode = Convert.ToString(datarow["REQID"]),
                        EmpName = Convert.ToString(datarow["REQNAME"]),
                        DeptName = Convert.ToString(datarow["DEPTNAME"]),
                        ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                        Plant = Convert.ToString(datarow["PLANT"]),
                        ModelName = Convert.ToString(datarow["MODEL"]),
                        PartNo = Convert.ToString(datarow["PARTNO"]),
                        Qty = Convert.ToString(datarow["QTY"]),
                        Customer = Convert.ToString(datarow["CUSTOMER"]),
                        Date1 = Convert.ToString(datarow["VALIDITY"]),
                        Type = Convert.ToString(datarow["DEVIATION_TYPE"]),
                        NatureOfIssue = Convert.ToString(datarow["NATURE_OF_ISSUE"]),
                        ActionProposed = Convert.ToString(datarow["ACTION_PROPOSED"]),
                        RootCause = Convert.ToString(datarow["ROOT_CAUSE"]),
                        CorrectiveAction = Convert.ToString(datarow["CORRECTIVE_ACTION"]),
                        StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                        Param3 = Convert.ToString(datarow["DEVIATION_STATUS"]),
                        UserID = Convert.ToString(datarow["USERID"]),
                        TDate = Convert.ToString(datarow["TDATE"]),
                        Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                        Param2 = Common.Encrypt("RptView")
                    });
                }
            }
            return Json(data);
        }

        protected void SendMail_Deviation(int iReqNo, string strRequestorName, string strDeviationType, string strNatureOfIssue, string strNextUserType, string strStatus)
        {
            string strEmailId = "";
            string strSubject = "";
            DataTable dt = _qualityService.usp_Deviation(new QualityViewModel { Activity = "GET_DEVIATION_EMAIL", RequestNo = iReqNo, ApprType = strNextUserType });
            if (dt != null && dt.Rows.Count > 0)
            {
                strEmailId = dt.Rows[0]["EMAILID"].ToString();
            }
            if (strNextUserType == "REQUESTOR")
            {
                strSubject = "Deviation Request Status.";
            }
            else
            {
                strSubject = "Deviation Request for Approval.";
            }
            //strEmailId = "pramodkumar.yadav@nvtpower.com";
            if (strEmailId == "")
            {
                return;
            }
            EmailSend objEmail = new();
            objEmail.To = strEmailId;

            objEmail.Subject = strSubject + " Request No : " + iReqNo;
            string strBody = "<b>Requestor Name : </b>" + strRequestorName + "<br/><br/>";
            strBody += "<b>Deviation Type : </b>" + strDeviationType + "<br/><br/>";
            strBody += "<b>Nature of Issue : </b>" + strNatureOfIssue + "<br/><br/>";
            if (strNextUserType == "REQUESTOR")
            {
                strBody += "<b>Status : </b>" + strStatus + "<br/><br/>";
            }
            else
            {
                strBody += "<b>Kindly Login on </b>" + " <a href='" + Common.URL + "'>" + "eConnect" + "</a>" + " to Approve/Reject/Review Back the request";
            }
            objEmail.Body = strBody;
            objEmail.SendEmail();
        }

        ///////// End Deviation System /////////


        ///////// Calibration System /////////
        public IActionResult CalibrationView()
        {
            QualityViewModel model = new QualityViewModel();
            model.Status = 2;
            model.Date1 = DateTime.Now.AddDays(-7).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }
        public JsonResult FillCalibrationViewList(Int32 status, string category, string param1, string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_Calibration(new QualityViewModel { Activity = "FILL_CALIBRATION_LIST", Status = status, Category = category, Param1 = param1, Date1 = date1, Date2 = date2 });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new QualityViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["DOCNO"]),
                    Category = Convert.ToString(datarow["CATEGORY"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Division = Convert.ToString(datarow["DIVISION"]),
                    EquipId = Convert.ToString(datarow["EQUIPID"]),
                    Date1 = Convert.ToString(datarow["CALI_DATE"]),
                    Date2 = Convert.ToString(datarow["CALI_DUE_DATE"]),
                    DueDays = Convert.ToString(datarow["DUE_DAYS"]),
                    CalibrationStatus = Convert.ToString(datarow["CALI_STATUS"]),
                    CertificateStatus = Convert.ToString(datarow["CERTI_STATUS"]),
                    Description = Convert.ToString(datarow["DESCR"]),
                    Make = Convert.ToString(datarow["MAKE"]),
                    Agency = Convert.ToString(datarow["AGENCY"]),
                    EquipModel = Convert.ToString(datarow["EQUIP_MODEL"]),
                    Type = Convert.ToString(datarow["CALI_TYPE"]),
                    Location = Convert.ToString(datarow["LOCATION"]),
                    MacName = Convert.ToString(datarow["MAC_NAME"]),
                    LeastCnt = Convert.ToString(datarow["LEAST_CNT"]),
                    FullRange = Convert.ToString(datarow["FULL_RANGE"]),
                    OperatingRange = Convert.ToString(datarow["OP_RANGE"]),
                    Unit = Convert.ToString(datarow["UNIT"]),
                    StatusDescr = Convert.ToString(datarow["ISACTIVE"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"])),
                    Param2 = Convert.ToString(datarow["ISACTIVE"]).ToUpper() == "TRUE" ? Common.Encrypt(Convert.ToString("E")) : Common.Encrypt(Convert.ToString("V")),
                });
            }
            return Json(data);
        }

        public JsonResult UpdateCalibrationStatus(int ReqNo, int Status)
        {
            _qualityService.usp_Calibration(new QualityViewModel() { Activity = "UPDATE_STATUS", RequestNo = ReqNo, Status = Status, UserID = User.Identity.GetADId() });
            return Json("OK");
        }

        public IActionResult CalibrationAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            string strMode = "";
            if (Convert.ToString(HttpContext.Request.Query["Utp"]) != null)
            {
                strMode = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            }
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            QualityViewModel model = new QualityViewModel();
            {
                model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_Calibration(new QualityViewModel() { Activity = "FILL_HISTORY", RequestNo = DocNo }));
                model.CertificateList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_Calibration(new QualityViewModel() { Activity = "FILL_CERTIFICATE_HISTORY", RequestNo = DocNo }));
                model.MakeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "38" }));
                model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "1" }));
                model.DivisionList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "3" }));
            }

            DataTable dt = _qualityService.usp_Calibration(new QualityViewModel { Activity = "GET_CALIBRATION_BY_DOCNO", RequestNo = DocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.Category = Convert.ToString(dt.Rows[0]["CATEGORY"]);
                model.EquipId = Convert.ToString(dt.Rows[0]["EQUIPID"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["CALI_DATE"]);
                model.Date2 = Convert.ToString(dt.Rows[0]["CALI_DUE_DATE"]);
                model.DueDays = Convert.ToString(dt.Rows[0]["DUE_DAYS"]);
                model.CalibrationStatus = Convert.ToString(dt.Rows[0]["CALI_STATUS"]);
                model.CertificateStatus = Convert.ToString(dt.Rows[0]["CERTI_STATUS"]);
                model.Description = Convert.ToString(dt.Rows[0]["DESCR"]);
                model.Make = Convert.ToString(dt.Rows[0]["MAKE"]);
                model.Agency = Convert.ToString(dt.Rows[0]["AGENCY"]);
                model.EquipModel = Convert.ToString(dt.Rows[0]["EQUIP_MODEL"]);
                model.Type = Convert.ToString(dt.Rows[0]["CALI_TYPE"]);
                model.Location = Convert.ToString(dt.Rows[0]["LOCATION"]);
                model.MacName = Convert.ToString(dt.Rows[0]["MAC_NAME"]);
                model.LeastCnt = Convert.ToString(dt.Rows[0]["LEAST_CNT"]);
                model.FullRange = Convert.ToString(dt.Rows[0]["FULL_RANGE"]);
                model.OperatingRange = Convert.ToString(dt.Rows[0]["OP_RANGE"]);
                model.Unit = Convert.ToString(dt.Rows[0]["UNIT"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);

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
                ViewBag.TagAttribute = "A";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult CalibrationAdd(QualityViewModel model, IFormFile DocName)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_CALIBRATION";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.Category = model.Category;
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.EquipId = model.EquipId;
                model.Date1 = model.Date1;
                model.Date2 = model.Date2;
                model.CertificateStatus = model.CertificateStatus;
                model.Description = model.Description;
                model.Make = model.Make;
                model.Agency = model.Agency;
                model.EquipModel = model.EquipModel;
                model.Type = model.Type;
                model.Location = model.Location;
                model.MacName = model.MacName;
                model.LeastCnt = model.LeastCnt;
                model.FullRange = model.FullRange;
                model.OperatingRange = model.OperatingRange;
                model.Unit = model.Unit;
                model.Remarks = model.Remarks;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _qualityService.usp_Calibration(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag > 0)
                    {
                        if (model.Param1.ToUpper() == "YES")
                        {
                            UploadCertificate(model, DocName);
                        }

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
                        return RedirectToAction(nameof(CalibrationView));
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
            return RedirectToAction(nameof(CalibrationView));
        }

        protected void UploadCertificate(QualityViewModel model, IFormFile DocName)
        {
            string strFilePath = Common.FilePath + @"\Calibration\";
            string strFileName = "";
            if (DocName != null)
            {
                string strDocName = Path.GetFileName(DocName.FileName);
                string strExt = strDocName.Split(".")[1];
                strFileName = model.RequestNo + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + strExt;
                bool save = Common.SaveFile1(DocName, strFilePath, strFileName);
            }

            model.Activity = "ADD_CERTIFICATE";
            model.RequestNo = Convert.ToInt32(model.RequestNo);
            model.Date3 = model.Date3;
            model.DocName = strFileName;
            model.UserID = User.Identity.GetADId();
            _qualityService.usp_Calibration(model);
        }

        public FileResult DownloadCalibrationFile(string fileName)
        {
            string path = Common.FilePath + @"\Calibration\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public IActionResult CalibrationReport()
        {
            QualityViewModel model = new QualityViewModel();
            model.Status = 2;
            model.Date1 = DateTime.Now.AddMonths(-1).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }
        public JsonResult FillCalibrationReport(Int32 status, string category, string param1, string date1, string date2)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_Calibration(new QualityViewModel { Activity = "FILL_CALIBRATION_LIST", Status = status, Category = category, Param1 = param1, Date1 = date1, Date2 = date2 });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new QualityViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["DOCNO"]),
                    Category = Convert.ToString(datarow["CATEGORY"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Division = Convert.ToString(datarow["DIVISION"]),
                    EquipId = Convert.ToString(datarow["EQUIPID"]),
                    Date1 = Convert.ToString(datarow["CALI_DATE"]),
                    Date2 = Convert.ToString(datarow["CALI_DUE_DATE"]),
                    DueDays = Convert.ToString(datarow["DUE_DAYS"]),
                    CalibrationStatus = Convert.ToString(datarow["CALI_STATUS"]),
                    CertificateStatus = Convert.ToString(datarow["CERTI_STATUS"]),
                    Description = Convert.ToString(datarow["DESCR"]),
                    Make = Convert.ToString(datarow["MAKE"]),
                    Agency = Convert.ToString(datarow["AGENCY"]),
                    EquipModel = Convert.ToString(datarow["EQUIP_MODEL"]),
                    Type = Convert.ToString(datarow["CALI_TYPE"]),
                    Location = Convert.ToString(datarow["LOCATION"]),
                    MacName = Convert.ToString(datarow["MAC_NAME"]),
                    LeastCnt = Convert.ToString(datarow["LEAST_CNT"]),
                    FullRange = Convert.ToString(datarow["FULL_RANGE"]),
                    OperatingRange = Convert.ToString(datarow["OP_RANGE"]),
                    Unit = Convert.ToString(datarow["UNIT"]),
                    StatusDescr = Convert.ToString(datarow["ISACTIVE"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"])),
                    Param2 = Common.Encrypt(Convert.ToString("V")),
                });
            }
            return Json(data);
        }

        ///////// End Calibration System /////////

        ///////// Sample Scarp System /////////

        #region Sample Scrap System
        public IActionResult SampleScrapAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(Dno)) : 0;
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            QualityViewModel model = new();
            if (DocNo > 0)
            {
                model = FillSampleScrapDocNo(DocNo);
                model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "FILL_HISTORY", RequestNo = DocNo }));
                model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "1" }));
                model.DivisionList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "3" }));
                model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "66" }));
                model.ApproverList = Common.BindDropDown(_qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "FILL_APPR_L1" }));
                if (model.UserID == User.Identity.GetADId() && (model.Status == 1 || model.Status == 2))
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
                model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "FILL_HISTORY", RequestNo = DocNo }));
                model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "1" }));
                model.DivisionList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "3" }));
                model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "66" }));
                model.RequestNo = 0;
                model.Date1 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                ViewBag.TagAttribute = "E";
                model.Status = 0;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult SampleScrapAdd(QualityViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_SAMPLE_SCRAP";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.ModelName = model.ModelName;
                model.Customer = model.Customer;
                model.Date1 = model.Date1;
                model.Date2 = model.Date2;
                model.Period = model.Period;
                model.Quantity = model.Quantity;
                model.Location = model.Location;
                model.Product = model.Product;
                model.Description = model.Description;
                model.Remarks = model.Remarks;
                model.Status = 2;
                model.UserID = User.Identity.GetADId();
                DataTable dt = _qualityService.usp_SampleScrap(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag > 0)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(SampleScrapView));
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
            return RedirectToAction(nameof(SampleScrapView));
        }

        protected QualityViewModel FillSampleScrapDocNo(int DocNo)
        {
            QualityViewModel model = new();
            DataTable dt = _qualityService.usp_SampleScrap(new QualityViewModel { Activity = "GET_SAMPLE_SCRAP_BY_DOCNO", RequestNo = DocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.BoxId = Convert.ToString(dt.Rows[0]["BOXID"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.ModelName = Convert.ToString(dt.Rows[0]["MODEL_NAME"]);
                model.Customer = Convert.ToString(dt.Rows[0]["CUSTOMER_NAME"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["TEST_COMP_DATE"]);
                model.Date2 = Convert.ToString(dt.Rows[0]["SAMPLE_DISP_DATE"]);
                model.Period = Convert.ToInt32(dt.Rows[0]["RETENTION_PERIOD"]);
                model.Quantity = Convert.ToDecimal(dt.Rows[0]["SAMPLE_QTY"]);
                model.Location = Convert.ToString(dt.Rows[0]["STORAGE_LOC"]);
                model.Product = Convert.ToString(dt.Rows[0]["PRODUCT"]);
                model.Description = Convert.ToString(dt.Rows[0]["DESCRIPTION"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.DueDays = Convert.ToString(dt.Rows[0]["DUE_DAYS"]);
                model.Appr1 = Convert.ToString(dt.Rows[0]["APPR1"]);
                model.Appr2 = Convert.ToString(dt.Rows[0]["APPR2"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);
            }
            return model;
        }

        public JsonResult UpdateScrapStatus(int iDocNo, string strApprId, string strProduct, string strCustName, string strModel, string strQty, string strBoxId)
        {
            _qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "POST_TO_HOD", RequestNo = iDocNo, Status = 3, ApprID = strApprId, UserID = User.Identity.GetADId() });
            var mail = Task.Run(() => SendSampleScrapMail(iDocNo, strProduct, strCustName, strModel, strQty, strBoxId, User.Identity.GetADId(), "REQUESTOR", "", "Post", strApprId, "L1"));
            return Json(2);
        }

        public IActionResult SampleScrapView()
        {
            QualityViewModel model = new();
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "67" }));
            model.ScrapCount1 = Convert.ToInt32(_qualityService.usp_SampleScrap(new QualityViewModel { Activity = "GET_PENDING_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            model.ScrapCount2 = Convert.ToInt32(_qualityService.usp_SampleScrap(new QualityViewModel { Activity = "GET_ALL_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            return View(model);
        }

        public JsonResult FillSampleScrapViewList(string activity, string date1, string date2, int status, string scrapStatus, string dueDays)
        {
            var data = new List<QualityViewModel>();
            DataTable dt = _qualityService.usp_SampleScrap(new QualityViewModel
            {
                Activity = activity,
                Date1 = date1,
                Date2 = date2,
                Status = status,
                Param1 = scrapStatus,
                Param2 = dueDays,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new QualityViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["DOCNO"]),
                    BoxId = Convert.ToString(datarow["BOXID"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Division = Convert.ToString(datarow["DIVISION"]),
                    ModelName = Convert.ToString(datarow["MODEL_NAME"]),
                    Customer = Convert.ToString(datarow["CUSTOMER_NAME"]),
                    Date1 = Convert.ToString(datarow["TEST_COMP_DATE"]),
                    Date2 = Convert.ToString(datarow["SAMPLE_DISP_DATE"]),
                    DueDays = Convert.ToString(datarow["DUE_DAYS"]),
                    Period = Convert.ToInt32(datarow["RETENTION_PERIOD"]),
                    Quantity = Convert.ToDecimal(datarow["SAMPLE_QTY"]),
                    Location = Convert.ToString(datarow["STORAGE_LOC"]),
                    Product = Convert.ToString(datarow["PRODUCT"]),
                    Description = Convert.ToString(datarow["DESCRIPTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    ScrapStatus = Convert.ToString(datarow["ISSCRAP"]),
                    Appr1 = Convert.ToString(datarow["APPR1"]),
                    Appr2 = Convert.ToString(datarow["APPR2"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"])),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    Param3 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"]))
                });
            }
            return Json(data);
        }

        public IActionResult SampleScrapReport()
        {
            QualityViewModel model = new();
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "67" }));
            model.ScrapCount1 = Convert.ToInt32(_qualityService.usp_SampleScrap(new QualityViewModel { Activity = "GET_PENDING_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            model.ScrapCount2 = Convert.ToInt32(_qualityService.usp_SampleScrap(new QualityViewModel { Activity = "GET_ALL_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            return View(model);
        }

        public JsonResult FillSampleScrapReport(string activity, string date1, string date2, int status, string scrapStatus, string dueDays)
        {
            var data = new List<QualityViewModel>();

            DataTable dt = _qualityService.usp_SampleScrap(new QualityViewModel
            {
                Activity = activity,
                Date1 = date1,
                Date2 = date2,
                Status = status,
                Param1 = scrapStatus,
                Param2 = dueDays,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new QualityViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["DOCNO"]),
                    BoxId = Convert.ToString(datarow["BOXID"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Division = Convert.ToString(datarow["DIVISION"]),
                    ModelName = Convert.ToString(datarow["MODEL_NAME"]),
                    Customer = Convert.ToString(datarow["CUSTOMER_NAME"]),
                    Date1 = Convert.ToString(datarow["TEST_COMP_DATE"]),
                    Date2 = Convert.ToString(datarow["SAMPLE_DISP_DATE"]),
                    DueDays = Convert.ToString(datarow["DUE_DAYS"]),
                    Period = Convert.ToInt32(datarow["RETENTION_PERIOD"]),
                    Quantity = Convert.ToDecimal(datarow["SAMPLE_QTY"]),
                    Location = Convert.ToString(datarow["STORAGE_LOC"]),
                    Product = Convert.ToString(datarow["PRODUCT"]),
                    Description = Convert.ToString(datarow["DESCRIPTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    ScrapStatus = Convert.ToString(datarow["ISSCRAP"]),
                    Appr1 = Convert.ToString(datarow["APPR1"]),
                    Appr2 = Convert.ToString(datarow["APPR2"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["DOCNO"])),
                    Param2 = Common.Encrypt(Convert.ToString("V")),
                });
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetScrapData(int ReqNo, string boxId, string modelName, decimal quantity, string date1, string date2, int period)
        {
            var data = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_SampleScrap(new QualityViewModel()
            {
                Activity = "GET_LABEL_DATA",
                RequestNo = ReqNo,
                BoxId = boxId,
                ModelName = modelName,
                Quantity = quantity,
                Date1 = date1,
                Date2 = date2,
                Period = period,
                UserID = User.Identity.GetADId()

            }));
            var bId = data.FirstOrDefault().BoxId;
            var QRimg = Common.GenerateQrCodeByText(bId.ToString().Trim());
            return Json(new { result = data, img = QRimg });
        }

        public IActionResult SampleScrapApprAdd()
        {
            int DocNo = Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"]));
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            string strApprId = "";
            string strApprType = "";
            if (Convert.ToString(HttpContext.Request.Query["Uid"]) != null)
            {
                strApprId = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Uid"]));
            }
            if (Convert.ToString(HttpContext.Request.Query["Utp"]) != null)
            {
                strApprType = Convert.ToString(Common.Decrypt(HttpContext.Request.Query["Utp"]));
            }

            QualityViewModel model = FillSampleScrapDocNo(DocNo);
            model.HistoryList = Common.ConvertDataTableToList<QualityViewModel>(_qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "FILL_HISTORY", RequestNo = DocNo }));
            model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "1" }));
            model.DivisionList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "3" }));
            model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "66" }));

            model.ApprID = strApprId;          //From Querystring
            model.ApprType = strApprType;          //From Querystring

            if (model.Status == 3 && strApprId == model.Appr1 && strApprType == "L1")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 4 && strApprId == model.Appr2 && strApprType == "L2")
            {
                ViewBag.TagAttribute = "E";
            }
            else
            {
                ViewBag.TagAttribute = "V";     //Default Disabled Everything
            }

            if (User.Identity.IsLoggedIn())
            {
                ViewBag.Login = "Y";
            }
            else
            {
                ViewBag.Login = "N";
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult SampleScrapApprAdd(int iDocNo, string strProduct, string strCustName, string strModel, string strQty, string strBoxId, string strReqId, string strApprType, string strApprId, string strApprStatus, string strRemarks)
        {
            string strNextApprId = "";
            string strNextApprType = "";
            Int32 iStatus = 0;

            if (strApprStatus == "APPROVE")
            {
                if (strApprType == "L1")
                {
                    iStatus = 4;
                    strNextApprId = _qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "FILL_APPR_L2" }).Rows[0]["APPRID"].ToString();
                    strNextApprType = "L2";
                }
                if (strApprType == "L2")
                {
                    iStatus = 5;
                }
            }
            else if (strApprStatus == "REVIEWBACK")
            {
                iStatus = 1;                //Pending with Requestor
                strNextApprId = strReqId;
                strNextApprType = "REQUESTOR";
            }
            else if (strApprStatus == "REJECT")
            {
                iStatus = 6;
            }
            else
            {
                return Json(0);
            }
            if (iStatus == 0)
            {
                return Json(0);
            }

            QualityViewModel model = new();
            model.Activity = "APPROVE_REQUEST";
            model.RequestNo = iDocNo > 0 ? iDocNo : 0;
            model.ApprType = strApprType;              //Current Approver Type (L1 or L2)
            model.Status = iStatus;
            model.StatusDescr = strApprStatus;
            model.Remarks = strRemarks;
            model.NextApprID = strNextApprId;
            model.NextApprType = strNextApprType;     //Next Approver Type 

            if (User.Identity.IsLoggedIn())
            {
                model.ApprID = User.Identity.GetADId();
            }
            else
            {
                model.ApprID = strApprId;
            }

            DataTable dt = _qualityService.usp_SampleScrap(model);
            int iFlag = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag > 0)
                {
                    var mail = Task.Run(() => SendSampleScrapMail(iDocNo, strProduct, strCustName, strModel, strQty, strBoxId, strReqId, strApprType, strApprId, strApprStatus, strNextApprId, strNextApprType));
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return Json(iFlag);
        }

        protected void SendSampleScrapMail(int iDocNo, string strProduct, string strCustName, string strModel, string strQty, string strBoxId, string strReqId, string strApprType, string strApprId, string strApprStatus, string strNextApprId, string strNextApprType)
        {
            string strEmailId_TO = "";
            string strEmailId_CC = "";
            string strMailSubject = "";

            if (strApprType == "REQUESTOR")
            {
                strEmailId_TO = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strNextApprId }).Rows[0]["EMAILID"].ToString();
                strMailSubject = "Lab Tested Sample Scrap Request for Approval";
            }
            if (strApprType == "L1")
            {
                if (strApprStatus == "APPROVE")
                {
                    strEmailId_TO = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strNextApprId }).Rows[0]["EMAILID"].ToString();
                    strMailSubject = "Lab Tested Sample Scrap Request Approved";
                }
                else
                {
                    strEmailId_TO = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strReqId }).Rows[0]["EMAILID"].ToString();
                    strMailSubject = "Lab Tested Sample Scrap Request";
                }
            }
            if (strApprType == "L2")
            {
                strEmailId_TO = _commonService.usp_Master(new CommonViewModel { Activity = "GET_EMAILID", UserId = strReqId }).Rows[0]["EMAILID"].ToString();
                if (strApprStatus == "APPROVE")
                {
                    strEmailId_CC = _qualityService.usp_SampleScrap(new QualityViewModel() { Activity = "GET_EMAILID_CC" }).Rows[0]["EMAILID"].ToString();
                    strMailSubject = "Lab Tested Sample Scrap Request Approved";
                }
                else
                {
                    strMailSubject = "Lab Tested Sample Scrap Request";
                }
            }
            if (strEmailId_TO == "")
            {
                return;
            }
            //strEmailId_TO = "mohit.kumar@nvtpower.com";
            EmailSend objEmail = new();
            objEmail.To = strEmailId_TO;
            objEmail.CC = strEmailId_CC;
            objEmail.Subject = strMailSubject + " Doc No : " + iDocNo;
            string strBody = "<b>Product : </b>" + strProduct + "<br/><br/>";
            strBody += "<b>Customer Name : </b>" + strCustName + "<br/><br/>";
            strBody += "<b>Model Name : </b>" + strModel + "<br/><br/>";
            strBody += "<b>Quantity : </b>" + strQty + "<br/><br/>";
            strBody += "<b>BoxId : </b>" + strBoxId + "<br/><br/>";
            strBody += "<b>Status : </b>" + strApprStatus + "<br/><br/>";

            if (strApprType == "REQUESTOR" || (strApprType == "L1" && strApprStatus == "APPROVE"))
            {
                string Url;
                Url = Common.URL + "Quality/SampleScrapApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iDocNo)) + "&Uid=" + Common.Encrypt(Convert.ToString(strNextApprId)) + "&Utp=" + Common.Encrypt(Convert.ToString(strNextApprType));
                strBody += "<b>For Approve/Reject/Review back to employee click on below link </b><br/><br/>";
                strBody += "<a href='" + Url + "'>" + Url + "</a>";
            }
            objEmail.Body = strBody;
            objEmail.SendEmail();
        }
        #endregion

        ///////// End Sample Scrap System /////////


        #region Lab Test System
        public IActionResult Sample()
        {
            ViewBag.SearchOption = "Pending";
            var data = new LabTestViewModel();
            if (TempData["Temp_GetSample"] == null || TempData["Temp_GetSample"].ToString() == "1")
            {
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetPendingRequest", EmpCode = User.Identity.GetEmpCode() }));
            }
            else if (TempData["Temp_GetSample"].ToString() == "2")
            {
                ViewBag.SearchOption = "All";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetAllRequest", EmpCode = User.Identity.GetEmpCode() }));
            }
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Sample(LabTestViewModel model, List<IFormFile> AttachDocument)
        {
            model.Activity = "AddRequest";
            model.EmpCode = User.Identity.GetEmpCode();
            string deptName = model.Department;
            model.Department = User.Identity.GetDeptCode();
            DataTable dt = _qualityService.usp_LAB(model);
            if (dt != null && dt.Rows.Count > 0)
            {
                int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag > 0)
                {
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;

                        string filepath = Common.FilePath + @"\Lab\";

                        string docId = string.Empty;
                        foreach (IFormFile item in AttachDocument)
                        {
                            if (item.Length > 0)
                            {
                                string extn = Path.GetExtension(item.FileName);
                                if (Common.IsValidExtnForKpi(extn))
                                {
                                    model.DocUId = Guid.NewGuid().ToString() + extn;
                                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), filepath, model.DocUId);
                                    using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                                    await item.CopyToAsync(fileStream);
                                    docId = model.DocUId;
                                    if (!string.IsNullOrEmpty(docId))
                                    {
                                        _qualityService.usp_LAB(new LabTestViewModel()
                                        {
                                            Activity = "LabTestCondDoc",
                                            Param1 = docId,
                                            ID = Convert.ToInt32(dt.Rows[0]["RETVAL"])
                                        });
                                        TempData["Result"] = Common.ResultSuccess;
                                        TempData["Message"] = Common.ResultSuccessMessage;
                                    }
                                }
                                else
                                {
                                    TempData["Result"] = Common.ResultError;
                                    TempData["Message"] = "Invalid File Format";
                                    return RedirectToAction(nameof(Sample));
                                }
                            }
                        }
                        string proName = (model.Product == "Other" ? model.ProductOther : model.Product);
                        SendMail_Lab("functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", "Request for lab testing", model.EmpName + " (" + deptName + ")", proName, model.ModelName, model.PartNumber, model.SampleQty, model.TestName, "New Request", "");
                    }
                    if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                    }
                    return RedirectToAction(nameof(Sample));
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return View(model);
        }
        public JsonResult GetSample(int Id)
        {
            var data = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id }));
            return Json(data);
        }
        public JsonResult DeleteSample(int Id)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "DeleteSample", ID = Id }));
            return Json("0");
        }
        public JsonResult USample(int Id, string eSampleQty, string eTest, string eTestCond)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "UpdateSample", ID = Id, SampleQty = eSampleQty, TestName = eTest, TestConditionIfAny = eTestCond }));
            return Json("0");
        }
        [HttpPost]
        public JsonResult GetSampleOption(string type)
        {
            if (type == "1" || type == "2")
            {
                TempData["Temp_GetSample"] = type;
            }
            if (type == "21" || type == "22" || type == "23" || type == "24" || type == "25")
            {
                TempData["Temp_GetManageSample"] = type;
            }
            return Json("");
        }
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
            {
                return Content("filename not present");
            }
            string filepath = Common.FilePath + @"\Lab\";
            string path = Path.Combine(Directory.GetCurrentDirectory(), filepath, filename);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, Common.GetContentType(path), Path.GetFileName(path));
        }
        public IActionResult ManageSample()
        {
            ViewBag.SearchOption = "Pending";
            var data = new LabTestViewModel();
            data.SampleRunningList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetTestRunningRequest" }));
            if (TempData["Temp_GetManageSample"] == null || TempData["Temp_GetManageSample"].ToString() == "21")
            {
                ViewBag.SearchOption = "Pending";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetPendingRequest", EmpCode = "0" }));
            }
            else if (TempData["Temp_GetManageSample"].ToString() == "22")
            {
                ViewBag.SearchOption = "Accepted";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetAcceptedRequest" }));
            }
            else if (TempData["Temp_GetManageSample"].ToString() == "23")
            {
                ViewBag.SearchOption = "Pass";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetTestCompletedRequest" }));
            }
            else if (TempData["Temp_GetManageSample"].ToString() == "24")
            {
                ViewBag.SearchOption = "HoldFailed";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetTestHoldFailedRequest" }));
            }
            else if (TempData["Temp_GetManageSample"].ToString() == "25")
            {
                ViewBag.SearchOption = "Reject";
                data.SampleList = Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetRejectRequest" }));
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult ManageSample(LabTestViewModel model)
        {
            return View(model);
        }
        public JsonResult AcceptSample(int Id)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "AcceptSample", ID = Id }));
            var data = _qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id });
            string proName = (data.Rows[0]["Product"].ToString() == "Other" ? data.Rows[0]["ProductOther"].ToString() : data.Rows[0]["Product"].ToString());
            SendMail_Lab(data.Rows[0]["ReqEmail"].ToString() + ",functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", "Test request accepted", data.Rows[0]["EmpName"] + " (" + data.Rows[0]["Department"] + ")", proName, data.Rows[0]["ModelName"].ToString(), data.Rows[0]["PartNumber"].ToString(), data.Rows[0]["SampleQty"].ToString(), data.Rows[0]["TestName"].ToString(), "Accepted", "");
            return Json("0");
        }
        public JsonResult RejectSample(int Id, string reason)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel() { Activity = "RejectSample", ID = Id, Param1 = reason }));
            var data = _qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id });
            string proName = (data.Rows[0]["Product"].ToString() == "Other" ? data.Rows[0]["ProductOther"].ToString() : data.Rows[0]["Product"].ToString());
            SendMail_Lab(data.Rows[0]["ReqEmail"].ToString() + ",functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", "Test request rejected", data.Rows[0]["EmpName"] + " (" + data.Rows[0]["Department"] + ")", proName, data.Rows[0]["ModelName"].ToString(), data.Rows[0]["PartNumber"].ToString(), data.Rows[0]["SampleQty"].ToString(), data.Rows[0]["TestName"].ToString(), "Rejected", reason);
            return Json("0");
        }
        public JsonResult UStartSampleTestTime(int Id, string TestStartDate, string StartHR, string StartMN, string StartTimeFormat)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel()
            {
                Activity = "UpdateSampleTime",
                ID = Id,
                TestStartDate = TestStartDate,
                Param1 = StartHR,
                Param2 = StartMN,
                Param3 = StartTimeFormat
            }));
            var data = _qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id });
            string proName = (data.Rows[0]["Product"].ToString() == "Other" ? data.Rows[0]["ProductOther"].ToString() : data.Rows[0]["Product"].ToString());
            SendMail_Lab(data.Rows[0]["ReqEmail"].ToString() + ",functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", "Testing has been started", data.Rows[0]["EmpName"] + " (" + data.Rows[0]["Department"] + ")", proName, data.Rows[0]["ModelName"].ToString(), data.Rows[0]["PartNumber"].ToString(), data.Rows[0]["SampleQty"].ToString(), data.Rows[0]["TestName"].ToString(), "Started", "");
            return Json("0");
        }
        public JsonResult URunningTest(int Id, string runningStatus, string remark)
        {
            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel()
            {
                Activity = "URunningTest",
                ID = Id,
                Status = runningStatus,
                Param1 = remark
            }));
            var data = _qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id });
            string subject = "";
            string rejectReason = "";
            string status = "";
            if (runningStatus == "Pass")
            {
                subject = "Testing has been completed and Passed";
                status = "Pass";
            }
            else
            {
                subject = "Testing result on Hold or Failed ";
                rejectReason = remark;
                status = "Hold or Failed";
            }
            string proName = (data.Rows[0]["Product"].ToString() == "Other" ? data.Rows[0]["ProductOther"].ToString() : data.Rows[0]["Product"].ToString());
            SendMail_Lab(data.Rows[0]["ReqEmail"].ToString() + ",functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", subject, data.Rows[0]["EmpName"] + " (" + data.Rows[0]["Department"] + ")", proName, data.Rows[0]["ModelName"].ToString(), data.Rows[0]["PartNumber"].ToString(), data.Rows[0]["SampleQty"].ToString(), data.Rows[0]["TestName"].ToString(), status, rejectReason);
            return Json("0");
        }
        [HttpPost]
        //public JsonResult UReviewStatus(IFormFileCollection formFiles)
        public JsonResult UReviewStatus(int Id, string rStatus, string remark)
        {
            var data1 = Request.Form.Files;

            Common.ConvertDataTableToList<LabTestViewModel>(_qualityService.usp_LAB(new LabTestViewModel()
            {
                Activity = "UReviewStatus",
                ID = Id,
                Status = rStatus,
                Param1 = remark
            }));
            var data = _qualityService.usp_LAB(new LabTestViewModel() { Activity = "GetSample", ID = Id });
            string subject = "";
            string rejectReason = "";
            string status = "";
            if (rStatus == "Pass")
            {
                subject = "Testing result Pass after review";
                status = "Pass";
            }
            else
            {
                subject = "Testing result Failed after review";
                rejectReason = remark;
                status = "Fail";
            }
            string proName = (data.Rows[0]["Product"].ToString() == "Other" ? data.Rows[0]["ProductOther"].ToString() : data.Rows[0]["Product"].ToString());
            SendMail_Lab(data.Rows[0]["ReqEmail"].ToString() + ",functional.quality.coordinator@nvtpower.com,Suman.kumari@NVTpower.com", subject, data.Rows[0]["EmpName"] + " (" + data.Rows[0]["Department"] + ")", proName, data.Rows[0]["ModelName"].ToString(), data.Rows[0]["PartNumber"].ToString(), data.Rows[0]["SampleQty"].ToString(), data.Rows[0]["TestName"].ToString(), status, rejectReason);
            return Json("0");
        }
        protected void SendMail_Lab(string toEmail, string subject, string reqNameWithDept, string product, string model, string partNumber, string sampleQuantity, string testName, string status, string reason)
        {
            if (toEmail == "")
            {
                return;
            }
            EmailSend objEmail = new();
            objEmail.To = toEmail;
            objEmail.Subject = subject;
            string strBody = "<b>Requestor Name : </b>" + reqNameWithDept + "<br/><br/>";
            strBody += "<b>Product : </b>" + product + "<br/><br/>";
            strBody += "<b>Model : </b>" + model + "<br/><br/>";
            strBody += "<b>Part Number : </b>" + partNumber + "<br/><br/>";
            strBody += "<b>Sample Quantity : </b>" + sampleQuantity + "<br/><br/>";
            strBody += "<b>Test Name : </b>" + testName + "<br/><br/>";
            strBody += "<b>Status : </b>" + status + "<br/><br/>";
            if (reason != "")
            {
                strBody += "<b>Reason : </b>" + reason + "<br/><br/>";
            }
            objEmail.Body = strBody;
            Thread email = new Thread(delegate ()
            {
                objEmail.SendEmail();
            });
            email.IsBackground = true;
            email.Start();
        }
        public IActionResult LabMachine()
        {
            var data = new LabTestViewModel();
            return View(data);
        }
        #endregion
    }
}
