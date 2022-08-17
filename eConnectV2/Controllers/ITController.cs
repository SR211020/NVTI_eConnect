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
    public class ITController : Controller
    {
        private readonly IITService _itService;
        private readonly ICommonService _commonService;

        public ITController(IITService itService, ICommonService commonService)
        {
            _itService = itService;
            _commonService = commonService;
        }

        #region HelpDesk System
        public IActionResult HDIndex()
        {
            ITViewModel model = new();
            model.DeptCode = User.Identity.GetDeptCode();
            model.HDCount1 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_PENDING_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            if (model.DeptCode == "DP0010")
            {
                model.HDCount2 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_PENDING_REQ_ITADMIN_COUNT" }).Rows[0]["TOTAL"]);
            }
            model.HDTotal = model.HDCount1 + model.HDCount2;
            return View(model);
        }

        public IActionResult HDAdd(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(Dno)) : 0;
            ITViewModel model = new();
            model.ITHOD = Convert.ToString(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_IT_HOD", UserID = User.Identity.GetADId() }).Rows[0]["HODCODE"]);
            if (DocNo > 0)
            {
                model = FillHelpdeskByReqNo(DocNo);
                model.DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" }));
                model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HELPDESK_HISTORY", RequestNo = DocNo }));
                model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "23" }));
                model.SubCategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "24", Param1 = model.ProbCatg }));
                model.EmployeeList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HOD", UserID = User.Identity.GetADId() }));
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
                model.DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" }));
                model.EmployeeList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HOD", UserID = User.Identity.GetADId() }));
                //model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD_ALL", UserId = User.Identity.GetADId() }));
                model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "23" }));
                model.RequestNo = 0;
                model.TDate = DateTime.Now.ToShortDateString();
                model.Plant = User.Identity.GetPlant();
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = model.EmpName2 = User.Identity.GetEmpName();
                model.DeptCode = model.DeptCode2 = User.Identity.GetDeptCode();
                model.DeptName = User.Identity.GetDeptName();
                model.Email = User.Identity.GetEmailId();
                model.ContactNo = User.Identity.GetContactNo();
                model.ExtNo = User.Identity.GetExtNo();
                model.Date1 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                model.Param1 = DateTime.Now.AddHours(-1).ToString("HH:mm");
                model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                model.Param2 = DateTime.Now.ToString("HH:mm");
                model.StatusDescr = "Open";
                ViewBag.TagAttribute = "E";
            }
            return View(model);
        }

        protected ITViewModel FillHelpdeskByReqNo(int DocNo)
        {
            ITViewModel model = new();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_HELPDESK_BY_DOCNO", RequestNo = DocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.TDate = Convert.ToString(dt.Rows[0]["TDATE"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Type = Convert.ToString(dt.Rows[0]["REQ_TYPE"]);
                if (model.Type.ToUpper() == "TASK")
                {
                    model.DeptCode = User.Identity.GetDeptCode();
                }
                else
                {
                    model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                }
                model.DeptCode2 = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["REQID"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.EmpName2 = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                model.ContactNo = Convert.ToString(dt.Rows[0]["CONTACTNO"]);
                model.ExtNo = Convert.ToString(dt.Rows[0]["EXTNO"]);
                model.Priority = Convert.ToString(dt.Rows[0]["PRIORITY"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["START_DATE"]);
                model.Param1 = Convert.ToString(dt.Rows[0]["START_TIME"]);
                model.Date2 = Convert.ToString(dt.Rows[0]["END_DATE"]);
                model.Param2 = Convert.ToString(dt.Rows[0]["END_TIME"]);
                if(model.Date1 == "" || model.Date2 == "")
                {
                    model.Date1 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                    model.Param1 = DateTime.Now.AddHours(-1).ToString("HH:mm");
                    model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
                    model.Param2 = DateTime.Now.ToString("HH:mm");
                }
                model.DocName = Convert.ToString(dt.Rows[0]["FILENAME"]);
                model.ProbCatg = Convert.ToString(dt.Rows[0]["PROBLEM_CATG"]);
                model.SubCatg = Convert.ToString(dt.Rows[0]["SUB_CATG"]);
                model.Subject = Convert.ToString(dt.Rows[0]["SUBJECT"]);
                model.Descr1 = Convert.ToString(dt.Rows[0]["PROBLEM_DESCR"]);
                model.Solution = Convert.ToString(dt.Rows[0]["SOLUTION"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.StatusDescr = Convert.ToString(dt.Rows[0]["STATUSDESCR"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);
                model.DeptHOD = Convert.ToString(dt.Rows[0]["DEPT_HOD"]);
                model.ITHOD = Convert.ToString(dt.Rows[0]["IT_HOD"]);
                model.ITEngineer = Convert.ToString(dt.Rows[0]["IT_ENGG"]);
                model.Area = Convert.ToString(dt.Rows[0]["IT_ENGG_AREA"]);

            }
            return model;
        }

        public JsonResult BindHelpDeskSubCatg(string catg)
        {
            var data = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "24", Param1 = catg }));
            return Json(data);
        }

        [HttpPost]
        public IActionResult HDAdd(ITViewModel model, IFormFile DocName)
        {
            string strFilePath = Common.FilePath + @"\Helpdesk\";
            string strFileName = "";
            if (ModelState.IsValid)
            {
                if (DocName != null)
                {
                    string strFile = Path.GetFileName(DocName.FileName);
                    string strExt = strFile.Split(".")[1];
                    strFileName = User.Identity.GetADId() + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "." + strExt;
                    if (model.DocNameEdit != null)
                    {
                        string strPath = strFilePath + model.DocNameEdit;
                        if (System.IO.File.Exists(strPath))
                        {
                            System.IO.File.Delete(strPath);
                        }
                    }
                    bool save = Common.SaveFile1(DocName, strFilePath, strFileName);
                }
                if (strFileName == "")
                {
                    strFileName = model.DocNameEdit;
                }
                model.Activity = "ADD_REQUEST";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.Plant = model.Plant;
                model.Priority = model.Priority;
                model.ContactNo = model.ContactNo;
                model.ExtNo = model.ExtNo;
                model.Type = model.Type;
                model.ProbCatg = model.ProbCatg;
                model.SubCatg = model.SubCatg;
                model.Subject = model.Subject;
                model.Descr1 = model.Descr1;
                model.DocName = strFileName;

                if (model.Type.ToUpper() == "NEW REQUEST")
                {
                    model.EmpCode = model.EmpCode;
                    model.EmpName = model.EmpName;
                    model.Email = model.Email;
                    model.DeptCode = model.DeptCode;
                    model.Solution = "";
                    if (model.DeptHOD != model.ITHOD)
                    {
                        model.Status = 2;
                        model.ApprID = model.DeptHOD;
                        model.ApprType = "HOD";
                    }
                    else
                    {
                        model.Status = 3;
                        model.ApprID = model.ITHOD;
                        model.ApprType = "IT_HOD";
                    }
                }
                else if (model.Type.ToUpper() == "INCIDENT")
                {
                    model.EmpCode = model.EmpCode;
                    model.EmpName = model.EmpName;
                    model.Email = model.Email;
                    model.DeptCode = model.DeptCode;
                    model.Solution = "";
                    model.Status = 4;
                    model.ApprType = "IT_ADMIN";
                }
                else  //For Task Add
                {
                    model.EmpCode = "";
                    model.EmpName = model.EmpName2;
                    model.Email = "";
                    model.DeptCode = model.DeptCode2;
                    model.Date1 = model.Date1 + " " + model.Param1;
                    model.Date2 = model.Date2 + " " + model.Param2;
                    model.Solution = model.Solution;
                    model.Status = 7;
                }
                model.UserID = User.Identity.GetADId();
                model.Remarks = model.Remarks;

                DataTable dt = _itService.usp_ITHelpDesk(model);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    int iReqNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
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
                        if (model.Type.ToUpper() == "NEW REQUEST")
                        {
                            if (model.DeptHOD != model.ITHOD)
                            {
                                var mail = Task.Run(() => SendHelpdeskMail(iReqNo, "New/Change Request", model.EmpName, model.Subject, "HOD", model.DeptHOD, 2, "Post"));
                            }
                            else
                            {
                                var mail = Task.Run(() => SendHelpdeskMail(iReqNo, "New/Change Request", model.EmpName, model.Subject, "IT_HOD", model.ITHOD, 3, "Post"));
                            }
                        }
                        return RedirectToAction(nameof(HDView));
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
            return RedirectToAction(nameof(HDAdd));
        }

        public FileResult DownloadHelpDeskFile(string fileName)
        {
            string path = Common.FilePath + @"\Helpdesk\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        
        public JsonResult CancelHelpDeskRequest(int ReqNo)
        {
            _itService.usp_ITHelpDesk(new ITViewModel() { Activity = "CANCEL_HELPDESK_REQUEST", RequestNo = ReqNo, UserID = User.Identity.GetADId() });
            return Json("OK");
        }

        public IActionResult HDView()
        {
            ITViewModel model = new();
            model.DeptCode = User.Identity.GetDeptCode();

            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "21" }));
            model.TypeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "22" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            
            model.HDCount1 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_PENDING_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            if (model.DeptCode == "DP0010")
            {
                model.HDCount2 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_PENDING_REQ_ITADMIN_COUNT" }).Rows[0]["TOTAL"]);
            }
            model.HDCount3 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_ALL_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            return View(model);
        }

        public JsonResult FillHelpdeskViewList(string activity, int status, string date1, string date2, string requestType)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel
            {
                Activity = activity,
                Status = status,
                Date1 = date1,
                Date2 = date2,
                Type = requestType,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    Type = Convert.ToString(datarow["REQ_TYPE"]),
                    Priority = Convert.ToString(datarow["PRIORITY"]),
                    Date1 = Convert.ToString(datarow["START_DATE"]),
                    Date2 = Convert.ToString(datarow["END_DATE"]),
                    TotalTime = Convert.ToString(datarow["TOTAL_TIME"]),
                    ProbCatg = Convert.ToString(datarow["PROBLEM_CATG"]),
                    SubCatg = Convert.ToString(datarow["SUB_CATG"]),
                    Subject = Convert.ToString(datarow["SUBJECT"]),
                    Descr1 = Convert.ToString(datarow["PROBLEM_DESCR"]),
                    Solution = Convert.ToString(datarow["SOLUTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    Area = Convert.ToString(datarow["IT_ENGG_AREA"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    Param3 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"]))
                });
            }
            return Json(data);
        }

        public IActionResult HDApprAdd()
        {
            int DocNo = Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"]));
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
            //int ReqNo = Convert.ToInt32(HttpContext.Request.Query["ReqNo"]);
            //string ApprId = HttpContext.Request.Query["ApprId"];
            //string ApprType = HttpContext.Request.Query["ApprType"];

            ITViewModel model = FillHelpdeskByReqNo(DocNo);
            model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "23" }));
            model.SubCategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "24", Param1 = model.ProbCatg }));
            model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HELPDESK_HISTORY", RequestNo = DocNo }));
            model.EmployeeList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "FILL_IT_ENGINEER" }));
            model.AreaList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "25" }));

            model.ApprID = strApprId;          //From Querystring
            model.ApprType = strApprType;          //From Querystring

            if (model.Status == 2 && strApprId == model.DeptHOD && strApprType == "HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 3 && strApprId == model.ITHOD && strApprType == "IT_HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 4 && User.Identity.GetDeptCode() == "DP0010" && strApprType == "IT_ADMIN")     //For IT Department User Only
            {
                model.ApprID = User.Identity.GetADId();
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 5 && strApprId == model.ITEngineer && strApprType == "IT_ENGG")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 6 && strApprId == model.UserID && strApprType == "UAT")
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
        public JsonResult HDApprAddAjax(int iReqNo, string strPlant, string strReqType, string strApprType, string strReqId, string strReqName, string strPriority, string strProbCatg, string strSubCatg,
            string strSubject, string strSolution, string strITEngineer, string strITEnggArea, string strRemarks, string strCurrentApprId, string strCurrentApprStatus, string strDate1, string strDate2)
        {
            string strNextApprId = "";
            string strNextApprType = "";
            Int32 iNextStatus = 0;
            string strNextStatusDescr = "";
            string strIsEmailSend = "Y";
            string strEmailApprId = "";
            string strEmailApprType = "";

            if (strCurrentApprStatus == "APPROVE")
            {
                if (strApprType == "HOD")
                {
                    iNextStatus = 3;                //Pending With IT Hod
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_commonService.usp_Master(new CommonViewModel { Activity = "FILL_DEPT_HOD", Dept = "DP0010", Plant = strPlant }).Rows[0]["CODE"]);
                    strEmailApprType = strNextApprType = "IT_HOD";
                    strIsEmailSend = "Y";
                }
                if (strApprType == "IT_HOD")          //For IT HOD
                {
                    iNextStatus = 5;                //Pending With IT Engineer
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_IT_ENGINEER", RequestNo = iReqNo }).Rows[0]["IT_ENGG"]);
                    if (strNextApprId == "")
                    {
                        strEmailApprId = strNextApprId = "NI00005";
                    }
                    strEmailApprType = strNextApprType = "IT_ENGG";
                    strIsEmailSend = "Y";
                }
            }
            else if (strCurrentApprStatus == "COMPLETE")
            {
                if (strApprType == "IT_ENGG")         //For IT Engineer
                {
                    iNextStatus = 6;                //Pending With UAT
                    strNextStatusDescr = "Completed";
                    strEmailApprId = strNextApprId = strReqId;
                    strEmailApprType = strNextApprType = "UAT";
                    strIsEmailSend = "Y";
                }
                if (strApprType == "UAT")             //For UAT
                {
                    iNextStatus = 7;                //Finish
                    strNextStatusDescr = "Finish";
                    strNextApprId = "";
                    strNextApprType = "";
                    strEmailApprId = strITEngineer;
                    strEmailApprType = "IT_ENGG";
                    strIsEmailSend = "Y";
                }
            }
            else if (strCurrentApprStatus == "ASSIGN")
            {
                if (strApprType == "IT_ADMIN")        //For IT Admin
                {
                    iNextStatus = 5;                //Pending With IT Engineer (Assign To Self)
                    strNextStatusDescr = "Assigned";
                    strNextApprId = strCurrentApprId;
                    strNextApprType = "IT_ENGG";
                    strIsEmailSend = "N";
                }
            }
            else if (strCurrentApprStatus == "FORWARD")
            {
                iNextStatus = 5;                //Pending With IT Engineer
                strNextStatusDescr = "Forwarded to IT Engineer";
                strEmailApprId = strNextApprId = strITEngineer;
                strEmailApprType = strNextApprType = "IT_ENGG";
                strIsEmailSend = "Y";
            }
            else if (strCurrentApprStatus == "WIP")
            {
                iNextStatus = 5;                     //Pending With IT Engineer
                strNextStatusDescr = "WIP";
                strNextApprId = strCurrentApprId;
                strNextApprType = "IT_ENGG";
                strIsEmailSend = "N";
            }
            else if (strCurrentApprStatus == "REJECT")
            {
                iNextStatus = 8;                //Rejected
                strNextStatusDescr = "Rejected";
                strNextApprId = "";
                strNextApprType = "";
                strEmailApprId = strReqId;
                strEmailApprType = "REQUESTOR";
                strIsEmailSend = "Y";
            }
            else if (strCurrentApprStatus == "REVIEWBACK")
            {
                if (strApprType == "UAT")
                {
                    iNextStatus = 5;                //Pending with IT Engineer
                    strNextStatusDescr = "Review Back";
                    strEmailApprId = strNextApprId = strITEngineer;
                    strEmailApprType = strNextApprType = "IT_ENGG";
                    strIsEmailSend = "Y";
                }
                else
                {
                    iNextStatus = 1;                //Pending with Requestor
                    strNextStatusDescr = "Review Back";
                    strEmailApprId = strNextApprId = strReqId;
                    strEmailApprType = strNextApprType = "REQUESTOR";
                    strIsEmailSend = "Y";
                }
            }
            else
            {
                return Json(0);
            }
            if (iNextStatus == 0)
            {
                return Json(0);
            }

            ITViewModel model = new();
            model.Activity = "APPROVE_REQUEST";
            model.RequestNo = iReqNo > 0 ? iReqNo : 0;
            if (strCurrentApprStatus == "COMPLETE")
            {
                if (strApprType == "IT_ENGG")
                {
                    model.Date1 = strDate1;
                    model.Date2 = strDate2;
                }
            }
            else
            {
                model.Date1 = " ";
                model.Date2 = " ";
            }
            model.Priority = strPriority;
            model.ProbCatg = strProbCatg;
            model.SubCatg = strSubCatg;
            model.Solution = strSolution;
            model.Param2 = strITEnggArea;
            model.Remarks = strRemarks;
            model.ApprType = strApprType;       //Current Approver Type
            model.ApprID = strNextApprId;
            model.Param1 = strNextApprType;       //Next Approver Type
            model.Status = iNextStatus;
            model.StatusDescr = strNextStatusDescr;
            if (User.Identity.IsLoggedIn())
            {
                model.UserID = User.Identity.GetADId();
            }
            else
            {
                model.UserID = strCurrentApprId;
            }

            DataTable dt = _itService.usp_ITHelpDesk(model);
            Int32 iFlag = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag > 0)
                {
                    if (strIsEmailSend == "Y")
                    {
                        var mail = Task.Run(() => SendHelpdeskMail(iReqNo, strReqType, strReqName, strSubject, strEmailApprType, strEmailApprId, iNextStatus, strNextStatusDescr));
                    }
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return Json(iFlag);
        }

        protected void SendHelpdeskMail(int iReqNo, string strReqType, string strReqName, string strReqSubject, string strApprType, string strApprId, int iStatus, string strStatusDescr)
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

            string strMailSubject;
            if (iStatus == 1)
            {
                strMailSubject = "Helpdesk Request Review Back. ";
            }
            else if (iStatus == 7)
            {
                strMailSubject = "Helpdesk Request Finished. ";
            }
            else if (iStatus == 8)
            {
                strMailSubject = "Helpdesk Request Rejected. ";
            }
            else
            {
                strMailSubject = "Helpdesk Request for Approval. ";
            }
            //strEmailId = "mohit.kumar@nvtpower.com";
            EmailSend objEmail = new();
            objEmail.To = strEmailId;
            objEmail.Subject = strMailSubject + "Request No : " + iReqNo;
            string strBody = "<b>Requestor Name : </b>" + strReqName + "<br/><br/>";
            strBody += "<b>Request Type : </b>" + strReqType + "<br/><br/>";
            strBody += "<b>Subject : </b>" + strReqSubject + "<br/><br/>";
            strBody += "<b>Status : </b>" + strStatusDescr + "<br/><br/>";

            if (strApprType != "REQUESTOR")
            {
                string Url;
                Url = Common.URL + "IT/HDApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iReqNo)) + "&Utp=" + Common.Encrypt(Convert.ToString(strApprType)) + "&Uid=" + Common.Encrypt(Convert.ToString(strApprId));
                strBody += "<b>For Approve/Reject/Review back to employee click on below link </b><br/><br/>";
                strBody += "<a href='" + Url + "'>" + Url + "</a>";
            }
            objEmail.Body = strBody;
            objEmail.SendEmail();
        }

        public IActionResult HDReport()
        {
            ITViewModel model = new();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "21" }));
            model.TypeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "22" }));
            model.Date1 = model.Date3 = model.Date5 = ("01-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year);
            model.Date2 = model.Date4 = model.Date6 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }

        public JsonResult HelpDeskExcelReport(string activity, string fromDate, string toDate, int status, string reqType)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_HELPDESK_DETAILED_REPORT", Date1 = fromDate, Date2 = toDate, Status = status, Type = reqType });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    Type = Convert.ToString(datarow["REQ_TYPE"]),
                    Priority = Convert.ToString(datarow["PRIORITY"]),
                    Date1 = Convert.ToString(datarow["START_DATE"]),
                    Date2 = Convert.ToString(datarow["END_DATE"]),
                    TotalTime = Convert.ToString(datarow["TOTAL_TIME"]),
                    ProbCatg = Convert.ToString(datarow["PROBLEM_CATG"]),
                    SubCatg = Convert.ToString(datarow["SUB_CATG"]),
                    Subject = Convert.ToString(datarow["SUBJECT"]),
                    Descr1 = Convert.ToString(datarow["PROBLEM_DESCR"]),
                    Solution = Convert.ToString(datarow["SOLUTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    Area = Convert.ToString(datarow["IT_ENGG_AREA"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"]))
                });
            }
            return Json(data);
        }

        public JsonResult HelpdeskGraphClosedRequests(string fromDate, string toDate)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_HELPDESK_GRAPH_REPORT1", Date1 = fromDate, Date2 = toDate });
            foreach (DataRow dataRow in dt.Rows)
            {
                data.Add(
                    new ITViewModel
                    {
                        GroupName = dataRow["PROBLEM_CATG"].ToString().Trim(),
                        Closed = Convert.ToInt32(dataRow["CLOSED_ISSUES"])
                    });
            }
            return Json(data);
        }
        public JsonResult HelpdeskGraphPendingRequests(string fromDate, string toDate)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_HELPDESK_GRAPH_REPORT1", Date1 = fromDate, Date2 = toDate });
            foreach (DataRow dataRow in dt.Rows)
            {
                data.Add(
                    new ITViewModel
                    {
                        GroupName = dataRow["PROBLEM_CATG"].ToString().Trim(),
                        Pending = Convert.ToInt32(dataRow["PENDING_ISSUES"])
                    });
            }
            return Json(data);
        }
        public JsonResult HelpdeskGraphITEngineers(string fromDate, string toDate)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_HELPDESK_ITENGG_REPORT1", Date1 = fromDate, Date2 = toDate });
            foreach (DataRow dataRow in dt.Rows)
            {
                data.Add(
                    new ITViewModel
                    {
                        ITEngineer = dataRow["IT_ENGG"].ToString().Trim(),
                        Pending = Convert.ToInt32(dataRow["PENDING_ISSUES"]),
                        Closed = Convert.ToInt32(dataRow["CLOSED_ISSUES"])
                    });
            }
            return Json(data);
        }

        #endregion

        #region Helpdesk UserId Create
        public IActionResult HDUserIdCreate(string Dno)
        {
            int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(Dno)) : 0;
            ViewBag.RUrl = Convert.ToString(HttpContext.Request.Query["RUrl"]);
            ITViewModel model = new();

            model.ITHOD = Convert.ToString(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_IT_HOD", UserID = User.Identity.GetADId() }).Rows[0]["HODCODE"]);
            if (DocNo > 0)
            {
                model = FillUsersCreateByReqNo(DocNo);
                model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_CREATE_USERS_HISTORY", RequestNo = DocNo }));
                model.ApproverList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HOD_USERCREATE", UserID = User.Identity.GetADId() }));
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
                model.ApproverList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_HOD_USERCREATE", UserID = User.Identity.GetADId() }));
                model.RequestNo = 0;
                model.TDate = DateTime.Now.ToShortDateString();
                model.Plant = User.Identity.GetPlant();
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = User.Identity.GetEmpName();
                model.DeptCode = User.Identity.GetDeptCode();
                model.DeptName = User.Identity.GetDeptName();
                model.Email = User.Identity.GetEmailId();
                model.ContactNo = User.Identity.GetContactNo();
                model.ExtNo = User.Identity.GetExtNo();
                model.StatusDescr = "Open";
                ViewBag.TagAttribute = "E";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult HDUserIdCreate(ITViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.ReqFor.ToUpper() == "OTHERS")
                {
                    model.EmpCode2 = model.EmpCode2;
                    model.EmpName2 = model.EmpName2;
                }
                else
                {
                    model.EmpCode2 = User.Identity.GetEmpCode();
                    model.EmpName2 = User.Identity.GetEmpName();
                }

                model.ProbCatg = model.IdType;
                if (model.ProbCatg.ToUpper() == "EMAILID")
                {
                    model.EmailAccess = model.EmailAccess;
                }
                else
                {
                    model.EmailAccess = "N";
                }
                if (model.DeptHOD != model.ITHOD)
                {
                    model.Status = 2;
                    model.ApprID = model.DeptHOD;
                    model.ApprType = "HOD";
                }
                else
                {
                    model.Status = 3;
                    model.ApprID = model.ITHOD;
                    model.ApprType = "IT_HOD";

                }
                model.UserID = User.Identity.GetADId();
                model.Activity = "USERID_CREATE";
                DataTable dt = _itService.usp_ITHelpDesk(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    int iReqNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                    if (iFlag == 1 || iFlag == 2)
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
                        if (model.DeptHOD != model.ITHOD)
                        {
                            var mail = Task.Run(() => SendUserCreateMail(iReqNo, model.EmpName, model.ProbCatg, model.ReqFor, model.EmpCode2, model.EmpName2, "HOD", model.DeptHOD, 2, "Post"));

                        }
                        else
                        {
                            var mail = Task.Run(() => SendUserCreateMail(iReqNo, model.EmpName, model.ProbCatg, model.ReqFor, model.EmpCode2, model.EmpName2, "IT_HOD", model.ITHOD, 3, "Post"));
                        }
                        return RedirectToAction(nameof(HDUserCreateView));
                    }
                    if (iFlag == 3)
                    {
                        if (iFlag == 3)
                        {
                            TempData["Result"] = Common.ResultError;
                            TempData["Message"] = "Record Already Exists";

                        }
                        return RedirectToAction(nameof(HDUserIdCreate));
                    }
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return RedirectToAction(nameof(HDUserIdCreate));

        }

        protected ITViewModel FillUsersCreateByReqNo(int DocNo)
        {
            ITViewModel model = new();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_CREATE_USERS_BY_DOCNO", RequestNo = DocNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["REQID"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.ContactNo = Convert.ToString(dt.Rows[0]["CONTACTNO"]);
                model.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                model.ReqFor = Convert.ToString(dt.Rows[0]["REQ_FOR"]);
                model.EmpCode2 = Convert.ToString(dt.Rows[0]["REQFOR_EMPCODE"]);
                model.EmpName2 = Convert.ToString(dt.Rows[0]["REQFOR_EMPNAME"]);
                model.IdType = Convert.ToString(dt.Rows[0]["ID_TYPE"]);
                model.EmailAccess = Convert.ToString(dt.Rows[0]["EXT_EMAIL_ACCESS"]);
                model.Roles = Convert.ToString(dt.Rows[0]["ROLES"]);
                model.Descr1 = Convert.ToString(dt.Rows[0]["DESCRIPTION"]);
                model.DeptHOD = Convert.ToString(dt.Rows[0]["DEPT_HOD"]);
                model.ITHOD = Convert.ToString(dt.Rows[0]["IT_HOD"]);
                model.ITEngineer = Convert.ToString(dt.Rows[0]["IT_ENGG"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.StatusDescr = Convert.ToString(dt.Rows[0]["STATUSDESCR"]);
                model.UserID = Convert.ToString(dt.Rows[0]["USERID"]);
                model.TDate = Convert.ToString(dt.Rows[0]["TDATE"]);
            }
            return model;
        }

        public IActionResult HDUserCreateView()
        {
            ITViewModel model = new();
            model.DeptCode = User.Identity.GetDeptCode();

            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "71" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");

            model.HDCount1 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_PENDING_USERS_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            model.HDCount2 = Convert.ToInt32(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_ALL_USERS_REQ_COUNT", UserID = User.Identity.GetADId() }).Rows[0]["TOTAL"]);
            return View(model);

        }

        public IActionResult HDUserCreateReport()
        {
            ITViewModel model = new();
            model.DeptCode = User.Identity.GetDeptCode();

            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "71" }));
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.Date3 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date4 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.Date5 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date6 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }

        public JsonResult FillUserCreateViewList(string activity, int status, string date1, string date2)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel
            {
                Activity = activity,
                Status = status,
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    ReqFor = Convert.ToString(datarow["REQ_FOR"]),
                    EmpCode2 = Convert.ToString(datarow["EMPCODE"]),
                    EmpName2 = Convert.ToString(datarow["EMPNAME"]),
                    IdType = Convert.ToString(datarow["CATEGORY"]),
                    EmailAccess = Convert.ToString(datarow["ACCESS"]),
                    Roles = Convert.ToString(datarow["ROLES"]),
                    Descr1 = Convert.ToString(datarow["DESCRIPTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    Param3 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"]))
                });
            }
            return Json(data);
        }

        public JsonResult GetIdWiseData(string date3, string date4)
        {
            var data = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "GET_GRAPH_BY_IDTYPE", Date3 = date3, Date4 = date4 }));
            return Json(data);
        }

        public JsonResult GetDeptWiseData(string date5, string date6)
        {
            var data = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "GET_GRAPH_BY_DEPTWISE", Date5 = date5, Date6 = date6 }));
            return Json(data);
        }

        public JsonResult FillUserCreateReport(int status, string date1, string date2)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITHelpDesk(new ITViewModel
            {
                Activity = "FILL_CREATE_USERS_REPORT_LIST",
                Status = status,
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    ReqFor = Convert.ToString(datarow["REQ_FOR"]),
                    EmpCode2 = Convert.ToString(datarow["EMPCODE"]),
                    EmpName2 = Convert.ToString(datarow["EMPNAME"]),
                    IdType = Convert.ToString(datarow["CATEGORY"]),
                    EmailAccess = Convert.ToString(datarow["ACCESS"]),
                    Roles = Convert.ToString(datarow["ROLES"]),
                    Descr1 = Convert.ToString(datarow["DESCRIPTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),
                    ApprName = Convert.ToString(datarow["APPRNAME"]),
                    Param1 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    Param3 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"]))
                });
            }
            return Json(data);
        }

        public JsonResult CancelUserManageRequest(int ReqNo)
        {
            _itService.usp_ITHelpDesk(new ITViewModel() { Activity = "CANCEL_USER_REQUEST", RequestNo = ReqNo, UserID = User.Identity.GetADId() });
            return Json("OK");
        }

        public IActionResult HDUserCreateApprAdd()
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

            ITViewModel model = FillUsersCreateByReqNo(DocNo);
            model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITHelpDesk(new ITViewModel() { Activity = "FILL_CREATE_USERS_HISTORY", RequestNo = DocNo }));
            model.EmployeeList = Common.BindDropDown(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "FILL_IT_ENGINEER" }));

            model.ApprID = strApprId;          //From Querystring
            model.ApprType = strApprType;          //From Querystring

            if (model.Status == 2 && strApprId == model.DeptHOD && strApprType == "HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 3 && strApprId == model.ITHOD && strApprType == "IT_HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 4 && strApprId == model.ITEngineer && strApprType == "IT_ENGG")
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
        public JsonResult HDUserCreateApprAdd(int iReqNo, string strPlant, string strReqId, string strReqName, string strEmpCode, string strEmpName, string strIdType,
            string strReqFor, string strApprType, string strCurrentApprId, string strCurrentApprStatus, string strITEngineer, string strRemarks)
        {
            string strNextApprId = "";
            string strNextApprType = "";
            Int32 iNextStatus = 0;
            string strNextStatusDescr = "";
            string strEmailApprId = "";
            string strEmailApprType = "";

            if (strCurrentApprStatus == "APPROVE")
            {
                if (strApprType == "HOD")
                {
                    iNextStatus = 3;                //Pending With IT Hod
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_commonService.usp_Master(new CommonViewModel { Activity = "FILL_DEPT_HOD", Dept = "DP0010", Plant = strPlant }).Rows[0]["CODE"]);
                    strEmailApprType = strNextApprType = "IT_HOD";
                }
                if (strApprType == "IT_HOD")          //For IT HOD
                {
                    iNextStatus = 4;                //Pending With IT Engineer
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_itService.usp_ITHelpDesk(new ITViewModel { Activity = "GET_ITENGG_UID", RequestNo = iReqNo }).Rows[0]["IT_ENGG"]);
                    if (strNextApprId == "")
                    {
                        strEmailApprId = strNextApprId = "NI00005";
                    }
                    strEmailApprType = strNextApprType = "IT_ENGG";
                }
            }
            else if (strCurrentApprStatus == "COMPLETE")
            {
                if (strApprType == "IT_ENGG")         //For IT Engineer
                {
                    iNextStatus = 5;
                    strNextStatusDescr = "Completed";
                    strEmailApprId = strNextApprId = strReqId;
                    strEmailApprType = strNextApprType = "REQUESTOR";
                }
            }
            else if (strCurrentApprStatus == "FORWARD")
            {
                iNextStatus = 4;                //Pending With IT Engineer
                strNextStatusDescr = "Forwarded to IT Engineer";
                strEmailApprId = strNextApprId = strITEngineer;
                strEmailApprType = strNextApprType = "IT_ENGG";
            }
            else if (strCurrentApprStatus == "REJECT")
            {
                iNextStatus = 6;                //Rejected
                strNextStatusDescr = "Rejected";
                strNextApprId = "";
                strNextApprType = "";
                strEmailApprId = strReqId;
                strEmailApprType = "REQUESTOR";
            }
            else if (strCurrentApprStatus == "REVIEWBACK")
            {
                iNextStatus = 1;                //Pending with Requestor
                strNextStatusDescr = "Review Back";
                strEmailApprId = strNextApprId = strReqId;
                strEmailApprType = strNextApprType = "REQUESTOR";
            }
            else
            {
                return Json(0);
            }
            if (iNextStatus == 0)
            {
                return Json(0);
            }

            ITViewModel model = new();
            model.Activity = "APPROVE_USER_REQUEST";
            model.RequestNo = iReqNo > 0 ? iReqNo : 0;
            model.Remarks = strRemarks;
            model.ApprType = strApprType;       //Current Approver Type
            model.ApprID = strNextApprId;
            model.Param1 = strNextApprType;       //Next Approver Type
            model.Status = iNextStatus;
            model.StatusDescr = strNextStatusDescr;

            if (User.Identity.IsLoggedIn())
            {
                model.UserID = User.Identity.GetADId();
            }
            else
            {
                model.UserID = strCurrentApprId;
            }

            DataTable dt = _itService.usp_ITHelpDesk(model);
            Int32 iFlag = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag > 0)
                {
                    var mail = Task.Run(() => SendUserCreateMail(iReqNo, strReqName, strIdType, strReqFor, strEmpCode, strEmpName, strEmailApprType, strEmailApprId, iNextStatus, strNextStatusDescr));
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return Json(iFlag);
        }

        protected void SendUserCreateMail(int iReqNo, string strReqName, string strIdType, string strReqFor, string strEmpCode, string strEmpName, string strApprType, string strApprId, int iStatus, string strStatusDescr)
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

            string strMailSubject;
            if (iStatus == 1)
            {
                strMailSubject = "UserId Creation Request Review Back. ";
            }
            else if (iStatus == 5)
            {
                strMailSubject = "UserId Creation Request Finished. ";
            }
            else if (iStatus == 6)
            {
                strMailSubject = " UserId Creation Request Rejected. ";
            }
            else
            {
                strMailSubject = "UserId Creation Request for Approval. ";
            }
            //strEmailId = "mohit.kumar@nvtpower.com";
            EmailSend objEmail = new();
            objEmail.To = strEmailId;
            objEmail.Subject = strMailSubject + "Request No : " + iReqNo;
            string strBody = "<b>Requestor Name : </b>" + strReqName + "<br/><br/>";
            strBody += "<b>Request For Id : </b>" + strIdType + "<br/><br/>";
            strBody += "<b>Req For : </b>" + strReqFor + "<br/><br/>";
            strBody += "<b>EmpCode : </b>" + strEmpCode + "<br/><br/>";
            strBody += "<b>EmpName : </b>" + strEmpName + "<br/><br/>";
            strBody += "<b>Status : </b>" + strStatusDescr + "<br/><br/>";

            if (strApprType != "REQUESTOR")
            {
                string Url;
                Url = Common.URL + "IT/HDUserCreateApprAdd?Dno=" + Common.Encrypt(Convert.ToString(iReqNo)) + "&Utp=" + Common.Encrypt(Convert.ToString(strApprType)) + "&Uid=" + Common.Encrypt(Convert.ToString(strApprId));
                strBody += "<b>For Approve/Reject/Review back to the employee click on the below link </b><br/><br/>";
                strBody += "<a href='" + Url + "'>" + Url + "</a>";
            }
            objEmail.Body = strBody;
            objEmail.SendEmail();
        }


        #endregion

        #region IT Asset Management System
        public IActionResult AssetMasterAdd(string Dno)
        {
            int SrNo = Dno != null ? Convert.ToInt32(Common.Decrypt(Dno)) : 0;
            string strMode = "";
            ITViewModel model = new ITViewModel();
            if (SrNo > 0)
            {

                model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "2" }));
                model.MakeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "63" }));
                model.VendorList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "64" }));
                model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "62" }));
                model.TDate = DateTime.Now.ToShortDateString();
                model.EmpName = User.Identity.GetEmpName();
                model.UserID = User.Identity.GetADId();

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
                model.MakeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "63" }));
                model.VendorList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "64" }));
                model.TDate = DateTime.Now.ToShortDateString();
                model.UserID = User.Identity.GetADId();
                model.EmpName = User.Identity.GetEmpName();
                ViewBag.TagAttribute = "E";
            }
            DataTable dt = _itService.usp_ITAsset(new ITViewModel { Activity = "GET_ASSET_BY_DOCNO", SrNo = SrNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.SrNo = Convert.ToInt32(dt.Rows[0]["SRNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Type = Convert.ToString(dt.Rows[0]["ASSET_TYPE"]);
                model.EquipCatg = Convert.ToString(dt.Rows[0]["EQUIPMENT_TYPE"]);
                model.Make = Convert.ToString(dt.Rows[0]["MAKE"]);
                model.ModelName = Convert.ToString(dt.Rows[0]["MODEL"]);
                model.DeviceSerialNo = Convert.ToString(dt.Rows[0]["DEVICE_SRNO"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["WRTY_START_DATE"]);
                model.Date2 = Convert.ToString(dt.Rows[0]["WRTY_END_DATE"]);
                model.Quantity = Convert.ToInt32(dt.Rows[0]["QUANTITY"]);
                model.VendorName = Convert.ToString(dt.Rows[0]["VENDOR"]);
                model.InvoiceNo = Convert.ToString(dt.Rows[0]["INVNO"]);
                model.PONo = Convert.ToString(dt.Rows[0]["PONO"]);
                model.Amount = Convert.ToString(dt.Rows[0]["PURCHASE_COST"]);
                model.PermanentAssetNo = Convert.ToString(dt.Rows[0]["ASSETNO"]);
                model.Param1 = Convert.ToString(dt.Rows[0]["TEMP_ASSETNO"]);
                model.Processor = Convert.ToString(dt.Rows[0]["PROCESSOR"]);
                model.CPUSpeed = Convert.ToString(dt.Rows[0]["CPU_SPEED"]);
                model.RAM = Convert.ToString(dt.Rows[0]["RAM"]);
                model.HDDType = Convert.ToString(dt.Rows[0]["HDD_TYPE"]);
                model.HDDCapacity = Convert.ToString(dt.Rows[0]["HDD_CAPACITY"]);
                model.DVD = Convert.ToString(dt.Rows[0]["DVD_RW"]);
                model.OSVersion = Convert.ToString(dt.Rows[0]["OS_VERSION"]);
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
        public IActionResult AssetMasterAdd(ITViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_ASSET";
                model.SrNo = Convert.ToInt32(model.SrNo);
                model.Plant = model.Plant;
                model.Type = model.Type;
                model.EquipCatg = model.EquipCatg;
                model.Make = model.Make;
                model.ModelName = model.ModelName;
                model.DeviceSerialNo = model.DeviceSerialNo;
                model.Date1 = model.Date1;
                model.Date2 = model.Date2;
                model.Quantity = model.Quantity;
                model.VendorName = model.VendorName;
                model.InvoiceNo = model.InvoiceNo;
                model.PONo = model.PONo;
                model.Amount = model.Amount;
                model.PermanentAssetNo = model.PermanentAssetNo;
                model.Param1 = model.Param1;
                if (model.EquipCatg.ToUpper() == "LAPTOP" || model.EquipCatg.ToUpper() == "DESKTOP")
                {
                    model.Processor = model.Processor;
                    model.CPUSpeed = model.CPUSpeed;
                    model.RAM = model.RAM;
                    model.HDDType = model.HDDType;
                    model.HDDCapacity = model.HDDCapacity;
                    model.DVD = model.DVD;
                    model.OSVersion = model.OSVersion;
                }
                else
                {
                    model.Processor = "";
                    model.CPUSpeed = "";
                    model.RAM = "";
                    model.HDDType = "";
                    model.HDDCapacity = "";
                    model.DVD = "";
                    model.OSVersion = "";

                }
                if (model.Type.ToUpper() == "CONSUMABLE")
                {
                    model.PermanentAssetNo = model.PermanentAssetNo;
                    model.Param1 = model.Param1;
                }
                else
                {
                    model.PermanentAssetNo = "";
                    model.Param1 = "";
                }
                model.Remarks = model.Remarks;
                model.UserID = User.Identity.GetADId();

                DataTable dt = _itService.usp_ITAsset(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    //int iReqNo = Convert.ToInt32(dt.Rows[0]["SRNO"]);

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
                        return RedirectToAction(nameof(AssetMasterView));
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
            return RedirectToAction(nameof(AssetMasterView));
        }
        
        public JsonResult BindEquipType(string type)
        {
            var data = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "62", Param1 = type }));
            return Json(data);
        }


        //public JsonResult AssetAddByAjax(Int32 iSrNo, string strPlant, string strAssetType, string strEquipmentCatg, string strMake, string strModelName, string strDeviceSerialNo, string strWarrentyStartDate,
        //           string strWarrentyEndDate, Int32 Quantity, string strVendorName, string strInvoiceNo, string strPONo, decimal dPurhaseCost, string strPermanentAssetNo, string strTempAssetNo, string strProcessor, string strCPUSpeed, string strRAM, string strHDDType, string strHDDCapacity, string strDVD, string strOSVersion, string strRemarks)
        //{
        //    ITViewModel model = new ITViewModel();
        //    model.Activity = "ADD_ASSET";
        //    model.SrNo = iSrNo > 0 ? iSrNo : 0;
        //    model.Plant = strPlant;
        //    model.Type = strAssetType;
        //    model.EquipCatg = strEquipmentCatg;
        //    model.Make = strMake;
        //    model.ModelName = strModelName;
        //    model.DeviceSerialNo = strDeviceSerialNo;
        //    model.Date1 = strWarrentyStartDate;
        //    model.Date2 = strWarrentyEndDate;
        //    model.QTY = Quantity;
        //    model.VendorName = strVendorName;
        //    model.InvoiceNo = strInvoiceNo;
        //    model.PONo = strPONo;
        //    model.Amount = dPurhaseCost;
        //    model.PermanentAssetNo = strPermanentAssetNo;
        //    model.Param1 = strTempAssetNo;
        //    model.Processor = strProcessor;
        //    model.CPUSpeed = strCPUSpeed;
        //    model.RAM = strRAM;
        //    model.HDDType = strHDDType;
        //    model.HDDCapacity = strHDDCapacity;
        //    model.DVD = strDVD;
        //    model.OSVersion = strOSVersion;          
        //    model.Remarks = strRemarks;
        //    model.UserID = User.Identity.GetADId();
        //    model.Status = 2;
        //    DataTable dt = _itService.usp_ITAsset(model);
        //    Int32 iFlag = 0;
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
        //        Int32 iReqNo = Convert.ToInt32(dt.Rows[0]["SrNo"]);
        //    }

        //    else
        //    {
        //        TempData["Result"] = Common.ResultError;
        //        TempData["Message"] = Common.SomethingWentWrong;
        //    }

        //    return Json(iFlag);
        //}

        public IActionResult AssetMasterView()
        {
            ITViewModel model = new();

            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            //model.DeptCode = User.Identity.GetDeptCode();
            return View(model);
        }

        public JsonResult FillAssetMasterViewList(string date1, string date2)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITAsset(new ITViewModel
            {
                Activity = "FILL_ASSET_VIEW_LIST",
                Date1 = date1,
                Date2 = date2,
                //UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    SrNo = Convert.ToInt32(datarow["SRNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    Type = Convert.ToString(datarow["ASSET_TYPE"]),
                    EquipCatg = Convert.ToString(datarow["EQUIPMENT_TYPE"]),
                    Make = Convert.ToString(datarow["MAKE"]),
                    ModelName = Convert.ToString(datarow["MODEL"]),
                    DeviceSerialNo = Convert.ToString(datarow["DEVICE_SRNO"]),
                    Date1 = Convert.ToString(datarow["WRTY_START_DATE"]),
                    Date2 = Convert.ToString(datarow["WRTY_END_DATE"]),
                    Quantity = Convert.ToInt32(datarow["QUANTITY"]),
                    VendorName = Convert.ToString(datarow["VENDOR"]),
                    InvoiceNo = Convert.ToString(datarow["INVNO"]),
                    PONo = Convert.ToString(datarow["PONO"]),
                    Amount = Convert.ToString(datarow["PURCHASE_COST"]),
                    PermanentAssetNo = Convert.ToString(datarow["ASSETNO"]),
                    Param1 = Convert.ToString(datarow["TEMP_ASSETNO"]),
                    Processor = Convert.ToString(datarow["PROCESSOR"]),
                    CPUSpeed = Convert.ToString(datarow["CPU_SPEED"]),
                    RAM = Convert.ToString(datarow["RAM"]),
                    HDDType = Convert.ToString(datarow["HDD_TYPE"]),
                    HDDCapacity = Convert.ToString(datarow["HDD_CAPACITY"]),
                    DVD = Convert.ToString(datarow["DVD_RW"]),
                    OSVersion = Convert.ToString(datarow["OS_VERSION"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["SrNo"])),
                    Param3 = Common.Encrypt(Convert.ToString("V")),

                });
            }
            return Json(data);
        }


        public IActionResult AssetReqAdd(string Dno)
        {
            int RequestNo = Dno != null ? Convert.ToInt32(Common.Decrypt(Dno)) : 0;
            ITViewModel model = new();
            string strMode = "";
            if (RequestNo > 0)
            {
                //model = FillHelpdeskByReqNo(SrNo);
                model.TDate = DateTime.Now.ToShortDateString();
                model.DeptName = User.Identity.GetDeptName();
                model.StatusDescr = "Open";
                model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "62" }));
                model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD", UserId = User.Identity.GetADId() }));
                model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITAsset(new ITViewModel() { Activity = "FILL_ASSET_HISTORY", RequestNo = RequestNo }));

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
                model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD", UserId = User.Identity.GetADId() }));
                model.RequestNo = 0;
                model.TDate = DateTime.Now.ToShortDateString();
                model.Plant = User.Identity.GetPlant();
                model.EmpCode = User.Identity.GetEmpCode();
                model.EmpName = User.Identity.GetEmpName();
                model.DeptCode = User.Identity.GetDeptCode();
                model.DeptName = User.Identity.GetDeptName();
                model.Email = User.Identity.GetEmailId();
                model.ContactNo = User.Identity.GetContactNo();
                model.StatusDescr = "Open";
                ViewBag.TagAttribute = "E";
            }
            DataTable dt = _itService.usp_ITAsset(new ITViewModel { Activity = "GET_ASSET_REQUEST_BY_REQNO", RequestNo = RequestNo });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.RequestNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["REQID"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["REQNAME"]);
                model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                model.ContactNo = Convert.ToString(dt.Rows[0]["CONTACTNO"]);
                model.Status = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                model.StatusDescr = Convert.ToString(dt.Rows[0]["STATUSDESCR"]);
                model.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                model.Type = Convert.ToString(dt.Rows[0]["ASSET_TYPE"]);
                model.EquipCatg = Convert.ToString(dt.Rows[0]["EQUIPMENT_TYPE"]);
                model.Subject = Convert.ToString(dt.Rows[0]["SUBJECT"]);
                model.Descr1 = Convert.ToString(dt.Rows[0]["DESCRIPTION"]);
                model.Remarks = Convert.ToString(dt.Rows[0]["REMARKS"]);
                model.DeptHOD = Convert.ToString(dt.Rows[0]["DEPT_HOD"]);
               // model.ITHOD = Convert.ToString(dt.Rows[0]["IT_HOD"]);
               // model.ITEngineer = Convert.ToString(dt.Rows[0]["IT_ENGG"]);
                //model.ApprID = Convert.ToString(dt.Rows[0]["DEPT_HOD"]);
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
        public IActionResult AssetReqAdd(ITViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ASSET_NEW_REQUEST";
                model.RequestNo = Convert.ToInt32(model.RequestNo);
                model.Plant = model.Plant;
                model.EmpCode = model.EmpCode;
                model.EmpName = model.EmpName;
                model.DeptCode = model.DeptCode;
                model.ContactNo = model.ContactNo;
                model.Email = model.Email;
                model.Type = model.Type;
                model.EquipCatg = model.EquipCatg;
                model.Subject = model.Subject;
                model.Descr1 = model.Descr1;
                model.DeptName = User.Identity.GetDeptName();
                model.UserID = User.Identity.GetADId();
                model.ApprID = model.DeptHOD;
                model.Status = 2;
                model.Remarks = model.Remarks;
                DataTable dt = _itService.usp_ITAsset(model);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    //int iReqNo = Convert.ToInt32(dt.Rows[0]["REQNO"]);

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
                        //var mail = Task.Run(() => SendHelpdeskMail(iReqNo, "New/Change Request", model.EmpName, model.Subject, "HOD", model.DeptHOD, 2, "Post"));
                        //return RedirectToAction(nameof(AssetRequestView));
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
            return RedirectToAction(nameof(AssetReqView));
        }
        public IActionResult AssetReqView()
        {
            ITViewModel model = new();

            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            //model.DeptCode = User.Identity.GetDeptCode();
            model.DeptName = User.Identity.GetDeptCode();
            model.StatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "65" }));

            return View(model);
        }
        public JsonResult FillAssetReqViewList(string date1, string date2, int status)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITAsset(new ITViewModel
            {
                Activity = "FILL_ASSET_REQUEST_LIST",
                Date1 = date1,
                Date2 = date2,
                Status = status,
                //UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    //DeptCode = Convert.ToString(datarow["DEPTCODE"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    Type = Convert.ToString(datarow["ASSET_TYPE"]),
                    EquipCatg = Convert.ToString(datarow["EQUIPMENT_TYPE"]),
                    Subject = Convert.ToString(datarow["SUBJECT"]),
                    Descr1 = Convert.ToString(datarow["DESCRIPTION"]),
                    Remarks = Convert.ToString(datarow["REMARKS"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    ApprName = Convert.ToString(datarow["PENDING_WITH"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    Param2 = Common.Encrypt(Convert.ToString(datarow["REQNO"])),
                    Param3 = Common.Encrypt(Convert.ToString("V")),
                });
            }
            return Json(data);
        }


        public JsonResult CancelAssetRequest(int requestNo)
        {
            _itService.usp_ITAsset(new ITViewModel() { Activity = "CANCEL_ASSET_REQUEST", RequestNo = requestNo, UserID = User.Identity.GetADId() });
            return Json("OK");
        }


        public IActionResult AssetReqApprAdd()
        {
            int iReqNo = Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"]));
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
            //int ReqNo = Convert.ToInt32(HttpContext.Request.Query["ReqNo"]);
            //string ApprId = HttpContext.Request.Query["ApprId"];
            //string ApprType = HttpContext.Request.Query["ApprType"];

            ITViewModel model = new ITViewModel();
            model.CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "62" }));
            model.EmployeeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_HOD", UserId = User.Identity.GetADId() }));
            model.HistoryList = Common.ConvertDataTableToList<ITViewModel>(_itService.usp_ITAsset(new ITViewModel() { Activity = "FILL_ASSET_HISTORY", RequestNo = iReqNo }));

            model.ApprID = strApprId;          //From Querystring
            model.ApprType = strApprType;          //From Querystring

            if (model.Status == 2 && strApprId == model.DeptHOD && strApprType == "HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            else if (model.Status == 3 && strApprId == model.ITHOD && strApprType == "IT_HOD")
            {
                ViewBag.TagAttribute = "E";
            }
            
            else if (model.Status == 4 && strApprId == model.ITEngineer && strApprType == "IT_ENGG")
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
        public JsonResult AssetReqApprAddAjax(int iReqNo, string strPlant, string strReqType, string strApprType, string strReqId, string strReqName, string strEquipCatg,
            string strSubject, string strITEngineer,  string strRemarks, string strCurrentApprId, string strCurrentApprStatus)
        {
            string strNextApprId = "";
            string strNextApprType = "";
            Int32 iNextStatus = 0;
            string strNextStatusDescr = "";
            string strIsEmailSend = "Y";
            string strEmailApprId = "";
            string strEmailApprType = "";

            if (strCurrentApprStatus == "APPROVE")
            {
                if (strApprType == "HOD")
                {
                    iNextStatus = 3;                //Pending With IT Hod
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_commonService.usp_Master(new CommonViewModel { Activity = "FILL_DEPT_HOD", Dept = "DP0010", Plant = strPlant }).Rows[0]["CODE"]);
                    strEmailApprType = strNextApprType = "IT_HOD";
                    strIsEmailSend = "Y";
                }
                if (strApprType == "IT_HOD")          //For IT HOD
                {
                    iNextStatus = 5;                //Pending With IT Engineer
                    strNextStatusDescr = "Approved";
                    strEmailApprId = strNextApprId = Convert.ToString(_itService.usp_ITAsset(new ITViewModel { Activity = "GET_IT_ENGINEER", RequestNo = iReqNo }).Rows[0]["IT_ENGG"]);
                    if (strNextApprId == "")
                    {
                        strEmailApprId = strNextApprId = "NI00005";
                    }
                    strEmailApprType = strNextApprType = "IT_ENGG";
                    strIsEmailSend = "Y";
                }
            }
            else if (strCurrentApprStatus == "COMPLETE")
            {
                if (strApprType == "IT_ENGG")         //For IT Engineer
                {
                    iNextStatus = 6;                //Pending With UAT
                    strNextStatusDescr = "Completed";
                    strEmailApprId = strNextApprId = strReqId;
                    //strEmailApprType = strNextApprType = "UAT";
                    strIsEmailSend = "Y";
                }
                //if (strApprType == "UAT")             //For UAT
                //{
                //    iNextStatus = 7;                //Finish
                //    strNextStatusDescr = "Finish";
                //    strNextApprId = "";
                //    strNextApprType = "";
                //    strEmailApprId = strITEngineer;
                //    strEmailApprType = "IT_ENGG";
                //    strIsEmailSend = "Y";
                //}
            }
            else if (strCurrentApprStatus == "ASSIGN")
            {
                if (strApprType == "IT_ADMIN")        //For IT Admin
                {
                    iNextStatus = 5;                //Pending With IT Engineer (Assign To Self)
                    strNextStatusDescr = "Assigned";
                    strNextApprId = strCurrentApprId;
                    strNextApprType = "IT_ENGG";
                    strIsEmailSend = "N";
                }
            }
            else if (strCurrentApprStatus == "FORWARD")
            {
                iNextStatus = 5;                //Pending With IT Engineer
                strNextStatusDescr = "Forwarded to IT Engineer";
                strEmailApprId = strNextApprId = strITEngineer;
                strEmailApprType = strNextApprType = "IT_ENGG";
                strIsEmailSend = "Y";
            }
            else if (strCurrentApprStatus == "WIP")
            {
                iNextStatus = 5;                     //Pending With IT Engineer
                strNextStatusDescr = "WIP";
                strNextApprId = strCurrentApprId;
                strNextApprType = "IT_ENGG";
                strIsEmailSend = "N";
            }
            else if (strCurrentApprStatus == "REJECT")
            {
                iNextStatus = 8;                //Rejected
                strNextStatusDescr = "Rejected";
                strNextApprId = "";
                strNextApprType = "";
                strEmailApprId = strReqId;
                strEmailApprType = "REQUESTOR";
                strIsEmailSend = "Y";
            }
            else if (strCurrentApprStatus == "REVIEWBACK")
            {
                if (strApprType == "UAT")
                {
                    iNextStatus = 5;                //Pending with IT Engineer
                    strNextStatusDescr = "Review Back";
                    strEmailApprId = strNextApprId = strITEngineer;
                    strEmailApprType = strNextApprType = "IT_ENGG";
                    strIsEmailSend = "Y";
                }
                else
                {
                    iNextStatus = 1;                //Pending with Requestor
                    strNextStatusDescr = "Review Back";
                    strEmailApprId = strNextApprId = strReqId;
                    strEmailApprType = strNextApprType = "REQUESTOR";
                    strIsEmailSend = "Y";
                }
            }
            else
            {
                return Json(0);
            }
            if (iNextStatus == 0)
            {
                return Json(0);
            }

            ITViewModel model = new();
            model.Activity = "APPROVE_REQUEST";
            model.RequestNo = iReqNo > 0 ? iReqNo : 0;
            
            model.EquipCatg = strEquipCatg;
            //model.Solution = strSolution;
            //model.Param2 = strITEnggArea;
            model.Remarks = strRemarks;

            model.ApprType = strApprType;       //Current Approver Type
            model.ApprID = strNextApprId;
            model.Param1 = strNextApprType;       //Next Approver Type
            model.Status = iNextStatus;
            model.StatusDescr = strNextStatusDescr;
            if (User.Identity.IsLoggedIn())
            {
                model.UserID = User.Identity.GetADId();
            }
            else
            {
                model.UserID = strCurrentApprId;
            }

            DataTable dt = _itService.usp_ITAsset(model);
            Int32 iFlag = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag > 0)
                {
                    if (strIsEmailSend == "Y")
                    {
                        //var mail = Task.Run(() => SendHelpdeskMail(iReqNo, strReqType, strReqName, strSubject, strEmailApprType, strEmailApprId, iNextStatus, strNextStatusDescr));
                    }
                }
                else
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.SomethingWentWrong;
                }
            }
            return Json(iFlag);
        }


        public IActionResult AssetPendingReqView()
        {
            ITViewModel model = new();
            model.Date1 = DateTime.Now.AddMonths(-12).ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            return View(model);
        }

        public JsonResult FillAssetPendingReqList(string date1, string date2)
        {
            var data = new List<ITViewModel>();
            DataTable dt = _itService.usp_ITAsset(new ITViewModel
            {
                Activity = "FILL_ASSET_PENDING_LIST",
                Date1 = date1,
                Date2 = date2,
                UserID = User.Identity.GetADId()
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new ITViewModel
                {
                    RequestNo = Convert.ToInt32(datarow["REQNO"]),
                    Plant = Convert.ToString(datarow["PLANT"]),
                    EmpCode = Convert.ToString(datarow["REQID"]),
                    EmpName = Convert.ToString(datarow["REQNAME"]),
                    DeptName = Convert.ToString(datarow["DEPTNAME"]),
                    ContactNo = Convert.ToString(datarow["CONTACTNO"]),
                    Email = Convert.ToString(datarow["EMAIL"]),
                    Type = Convert.ToString(datarow["REQ_TYPE"]),
                    Subject = Convert.ToString(datarow["SUBJECT"]),
                    Descr1 = Convert.ToString(datarow["DESCRIPTION"]),
                    Status = Convert.ToInt32(datarow["STATUS"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    UserID = Convert.ToString(datarow["USERID"]),
                    TDate = Convert.ToString(datarow["TDATE"]),
                    DeptHOD = Convert.ToString(datarow["DEPT_HOD"]),
                    ITHOD = Convert.ToString(datarow["IT_HOD"]),
                    ITEngineer = Convert.ToString(datarow["IT_ENGG"]),
                    ApprID = Convert.ToString(datarow["PENDING_WITH"]),
                    ApprType = Convert.ToString(datarow["APPRTYPE"]),

                    Param2 = Common.Encrypt(Convert.ToString(datarow["REQNO"]))
                    //Param2 = Common.Encrypt(Convert.ToString(datarow["PENDING_WITH"])),
                    //Param3 = Common.Encrypt(Convert.ToString(datarow["APPRTYPE"]))
  

                });
            }
            return Json(data);
        }

        #endregion

    }
}







