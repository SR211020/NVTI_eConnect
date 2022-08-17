using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class DigitalTrainingController : Controller
    {
        private readonly IDTService _dtService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _iweb;
        public DigitalTrainingController(IDTService dtService, ICommonService commonService, IWebHostEnvironment iweb)
        {
            _dtService = dtService;
            _commonService = commonService;
            _iweb = iweb;
        }





        public IActionResult ManPower()
        {
            var data = new DTViewModel
            {
                EmpTypeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "51" })),
                EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "52" })),
            };
            data.EmpGradeList.RemoveRange(1, 3);
            data.EmpStatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "53" }));
            data.EmpStatusList.RemoveRange(1, 2);
            data.ShowActiveManPowerList = 1;
            if (TempData["Temp_DTGetActiveManpOwer"] != null && TempData["Temp_DTGetActiveManpOwer"].ToString() == "0")
            {
                data.ShowActiveManPowerList = 0;
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetInActiveManPower" }));
            }
            else
            {
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPower" }));
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult ManPower(DTViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _dtService.Usp_DT(new DTViewModel()
                {
                    Activity = "AddManPower",
                    AadharNo = model.AadharNo.Trim(),
                    EmpName = model.EmpName.Trim(),
                    EmpCode = model.EmpCode.Trim(),
                    Qualification = model.Qualification.Trim(),
                    DOJ = model.DOJ.Trim(),
                    EmpType = model.EmpType.Trim(),
                    EmpStatus = model.EmpStatus.Trim(),
                    Grade = model.Grade.Trim(),
                    IsEmpReJoin = model.IsEmpReJoin,
                    OldEmpCode = model.OldEmpCode.Trim(),
                    AdId = User.Identity.GetADId()
                });
                if (dt.Rows[0]["FLAG"].ToString() == "1")
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                    if (model.IsEmpReJoin)
                    {
                        DateTime DOL = Convert.ToDateTime(model.DOL);
                        DateTime tDate = DateTime.Now.AddDays(-30);
                        if (DOL.Date < tDate.Date)
                        {
                            //more than 30 days
                            _dtService.Usp_DT(new DTViewModel() { Activity = "UpdateSkillsNotConsider", AadharNo = model.AadharNo.Trim() });
                        }
                    }
                    return RedirectToAction(nameof(ManPower));
                }
                if (dt.Rows[0]["FLAG"].ToString() == "0")
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                    model.IsEmpStatusLeft = dt.Rows[0]["RetVal"].ToString() == "2";
                    if (model.IsEmpStatusLeft)
                    {
                        var empLeftDtls = _dtService.Usp_DT(new DTViewModel() { Activity = "GetLeftManPowerByANo", AadharNo = model.AadharNo.Trim() });
                        ModelState.Remove("EmpName");
                        ModelState.Remove("EmpCode");
                        ModelState.Remove("Qualification");
                        ModelState.Remove("EmpType");
                        ModelState.Remove("Grade");
                        model.EmpName = empLeftDtls.Rows[0]["EMPNAME"].ToString();
                        model.EmpCode = "";
                        model.Qualification = empLeftDtls.Rows[0]["QUALIFICATION"].ToString();
                        model.EmpType = empLeftDtls.Rows[0]["EMPTYPE"].ToString();
                        model.Grade = empLeftDtls.Rows[0]["GRADE"].ToString();
                        model.OldEmpCode = empLeftDtls.Rows[0]["EMPCODE"].ToString();
                        model.DOL = empLeftDtls.Rows[0]["DOL"].ToString();
                    }
                }
                model.EmpTypeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "51" }));
                model.EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "52" }));
                if (model.Grade == "D")
                {
                    model.EmpGradeList.RemoveRange(1, 3);
                }
                else
                {
                    model.EmpGradeList.Clear();
                    model.EmpGradeList.Add(new SelectListItem() { Text = model.Grade, Value = model.Grade });
                    model.EmpGradeList.Add(new SelectListItem() { Text = "D", Value = "D" });
                }
                model.EmpStatusList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "53" }));
                model.EmpStatusList.RemoveRange(1, 2);
                model.ShowActiveManPowerList = 1;
                model.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPower" }));
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult GetEmpStatus()
        {
            return Json(Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "53" })));
        }
        [HttpPost]
        public JsonResult GetManPowerDetails(string aNo)
        {
            return Json(Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetManPowerByANo", AadharNo = aNo.Trim() })));
        }
        [HttpPost]
        public JsonResult GetManPowerSet(string type)
        {
            TempData["Temp_DTGetActiveManpOwer"] = type;
            return Json("");
        }
        [HttpPost]
        public JsonResult GetEmpType()
        {
            return Json(Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "51" })));
        }
        [HttpPost]
        public JsonResult SaveManPower(string eAadharNo, string eEmpName, string eEmpCode, string eQualification, string eDOJ, string eDOL, string eEmpStatus, string eEmpType, string eGrade, string eReason)
        {
            var dt = _dtService.Usp_DT(new DTViewModel()
            {
                Activity = "UpdateManPowerByANo",
                AadharNo = eAadharNo,
                EmpName = eEmpName,
                EmpCode = eEmpCode,
                Qualification = eQualification,
                DOJ = eDOJ,
                DOL = eDOL,
                EmpStatus = eEmpStatus,
                EmpType = eEmpType,
                Grade = eGrade,
                Reason = eReason,
                AdId = User.Identity.GetADId()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = dt.Rows[0]["MSG"].ToString() });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        [HttpPost]
        public JsonResult GetManPowerSkills(string aNo)
        {
            var data = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetManPowerSkills", AadharNo = aNo.Trim() }));
            return Json(data);
        }





        public IActionResult SkillQuestion()
        {
            var data = new DTViewModel
            {
                EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" })),
                StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" })),
                QuestionList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetQuestion", Station = "1", Grade = "C" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult SkillQuestion(DTViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" }));
                model.StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" }));
                model.QuestionList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetQuestion", Station = model.Station, Grade = model.Grade }));
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult GetStation()
        {
            return Json(Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" })));
        }
        [HttpPost]
        public JsonResult GetGrade()
        {
            return Json(Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" })));
        }
        [HttpPost]
        public JsonResult ManageQuestion(string eStation, string eGrade, string eQue, string eOption1, string eOption2, string eOption3, string eOption4, string eAns, string eAction, int eId)
        {
            var dt = _dtService.Usp_DT(new DTViewModel()
            {
                Activity = "SaveQuestion",
                Station = eStation,
                Grade = eGrade,
                Que = eQue.Trim(),
                Option1 = eOption1.Trim(),
                Option2 = eOption2.Trim(),
                Option3 = eOption3.Trim(),
                Option4 = eOption4.Trim(),
                Ans = eAns.Trim(),
                Action = eAction.Trim(),
                AdId = User.Identity.GetADId(),
                Id = eId
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                return Json(new { result = Common.ResultSuccess, msg = dt.Rows[0]["MSG"].ToString() });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        [HttpPost]
        public JsonResult DeleteQuestion(int id)
        {
            var dt = _dtService.Usp_DT(new DTViewModel() { Activity = "DeleteQuestion", Id = id });
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
        public JsonResult GetQueById(int id)
        {
            return Json(Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetQueById", Id = id })));
        }





        public IActionResult OJTMaterial()
        {
            var data = new DTViewModel
            {
                EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" })),
                StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" })),
                OJTDocumentList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetOJTDoc" }))
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> OJTMaterial(DTViewModel model, List<IFormFile> AttachDocument)
        {
            if (ModelState.IsValid)
            {
                string filepath = Common.FilePath + @"\DT\";

                string docId = string.Empty;
                foreach (IFormFile item in AttachDocument)
                {
                    if (item.Length > 0)
                    {
                        string fileName = Path.GetFileName(item.FileName);
                        string extn = Path.GetExtension(item.FileName);
                        if (Common.IsValidExtnForDT(extn))
                        {
                            model.DocUId = Guid.NewGuid().ToString() + extn;
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), filepath, model.DocUId);
                            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                            await item.CopyToAsync(fileStream);
                            docId = model.DocUId;
                            if (!string.IsNullOrEmpty(docId))
                            {
                                _dtService.Usp_DT(new DTViewModel()
                                {
                                    Activity = "AddOJTDoc",
                                    Station = model.Station,
                                    Grade = model.Grade,
                                    DocUId = docId,
                                    AdId = User.Identity.GetADId()
                                });
                                TempData["Result"] = Common.ResultSuccess;
                                TempData["Message"] = Common.ResultSuccessMessage;
                            }
                        }
                        else
                        {
                            TempData["Result"] = Common.ResultError;
                            TempData["Message"] = "Invalid File Format";
                            return RedirectToAction(nameof(OJTMaterial));
                        }
                    }
                }
            }
            model.EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" }));
            model.StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" }));
            model.OJTDocumentList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetOJTDoc" }));
            return View(model);
        }
        [HttpPost]
        public JsonResult DeleteOJTDoc(int id)
        {
            var dt = _dtService.Usp_DT(new DTViewModel() { Activity = "DeleteOJTDoc", DocUId = id.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                string docId = dt.Rows[0]["RETVAL"].ToString();
                if (!string.IsNullOrEmpty(docId))
                {
                    string filepath = Common.FilePath + @"\DT\";
                    string img = Path.Combine(_iweb.WebRootPath, filepath, docId);
                    FileInfo fi = new FileInfo(img);
                    if (fi != null)
                    {
                        System.IO.File.Delete(img);
                        fi.Delete();
                    }
                }
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
            {
                return Content("filename not present");
            }
            string filepath = Common.FilePath + @"\DT\";
            string path = Path.Combine(Directory.GetCurrentDirectory(), filepath, filename);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, Common.GetContentType(path), Path.GetFileName(path));
        }





        public IActionResult OJTAttendance()
        {
            var data = new DTViewModel
            {
                TrainerList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTrainerDDL" })),
                EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" })),
                StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" })),
                ManPowerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPowerDDL" })),
                OJTAttendanceList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetOJTAttendance" })),
            };
            data.EmpList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetAllManpower" }));
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> OJTAttendance(DTViewModel model, List<IFormFile> AttachDocument)
        {
            if (ModelState.IsValid)
            {
                string filepath = Common.FilePath + @"\DT\";

                string docId = string.Empty;
                foreach (IFormFile item in AttachDocument)
                {
                    if (item.Length > 0)
                    {
                        string fileName = Path.GetFileName(item.FileName);
                        string extn = Path.GetExtension(item.FileName);
                        if (Common.IsValidExtnForDT(extn))
                        {
                            model.DocUId = Guid.NewGuid().ToString() + extn;
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), filepath, model.DocUId);
                            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                            await item.CopyToAsync(fileStream);
                            docId = model.DocUId;
                            if (!string.IsNullOrEmpty(docId))
                            {
                                var dt = _dtService.Usp_DT(new DTViewModel()
                                {
                                    Activity = "AddOJTAttendance",
                                    Station = model.Station,
                                    Grade = model.Grade,
                                    TrainingDate = model.TrainingDate,
                                    TrainingFor = model.TrainingFor,
                                    Trainer = model.Trainer,
                                    EmpCodes = model.SelectedImpact,
                                    DocUId = docId,
                                    AdId = User.Identity.GetADId()
                                });
                                if (dt.Rows[0]["FLAG"].ToString() == "1")
                                {
                                    TempData["Result"] = Common.ResultSuccess;
                                    TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                                    return RedirectToAction(nameof(OJTAttendance));
                                }
                            }
                        }
                        else
                        {
                            TempData["Result"] = Common.ResultError;
                            TempData["Message"] = "Invalid File Format";
                            return RedirectToAction(nameof(OJTMaterial));
                        }
                    }
                }
            }
            model.TrainerList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTrainerDDL" }));
            model.EmpGradeList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "55" }));
            model.StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" }));
            model.ManPowerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPowerDDL" }));
            model.OJTAttendanceList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetOJTAttendance" }));
            return View(model);
        }
        [HttpPost]
        public JsonResult DeleteOJTAttendance(int id)
        {
            var dt = _dtService.Usp_DT(new DTViewModel() { Activity = "DeleteOJTAttendance", Id = id });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                string docId = dt.Rows[0]["RETVAL"].ToString();
                if (!string.IsNullOrEmpty(docId))
                {
                    string filepath = Common.FilePath + @"\DT\";
                    string img = Path.Combine(_iweb.WebRootPath, filepath, docId);
                    FileInfo fi = new FileInfo(img);
                    if (fi != null)
                    {
                        System.IO.File.Delete(img);
                        fi.Delete();
                    }
                }
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        [HttpPost]
        public JsonResult GetOJTAttendEmp(int id)
        {
            var dt = _dtService.Usp_DT(new DTViewModel() { Activity = "GetOJTAttendance", Id = id });
            var EmpCodes = dt.Rows[0]["EmpCodes"].ToString();
            var empList = _dtService.Usp_DT(new DTViewModel() { Activity = "GetManPower", EmpCodes = EmpCodes });

            var result = new DTViewModel()
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                Station = dt.Rows[0]["Station"].ToString(),
                Grade = dt.Rows[0]["Grade"].ToString(),
                TrainingDate = dt.Rows[0]["TrainingDate"].ToString(),
                TrainingFor = dt.Rows[0]["TrainingFor"].ToString(),
                Trainer = dt.Rows[0]["Trainer"].ToString(),
                ManPowerList = Common.ConvertDataTableToList<DTViewModel>(empList)
            };
            return Json(new { success = true, responseMsg = "", result = result });
        }
        [HttpPost]
        public JsonResult GetEmpOJTAttendance(string empcode)
        {
            return Json(Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetEmpOJTAttendance", EmpCode = empcode })));
        }





        public IActionResult Approver()
        {
            var data = new DTViewModel()
            {
                EmpList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_BY_CODE" })),
                ApproverList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetApprover" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult Approver(DTViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _dtService.Usp_DT(new DTViewModel()
                {
                    Activity = "AddApprover",
                    CustomerName = model.CustomerName,
                    PrimaryApprover = model.PrimaryApprover,
                    Approver1 = model.Approver1,
                    Approver2 = model.Approver2,
                    Approver3 = model.Approver3,
                    Approver4 = model.Approver4,
                    Approver5 = model.Approver5,
                    Approver6 = model.Approver6,
                    AdId = User.Identity.GetADId()
                });
                if (dt.Rows[0]["FLAG"].ToString() == "1")
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                    return RedirectToAction(nameof(Approver));
                }
                if (dt.Rows[0]["FLAG"].ToString() == "0")
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                }
                model.EmpList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_BY_CODE" }));
                model.ApproverList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetApprover" }));
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult UpdateApprover(int eApprId, string CustomerName, string PrimaryApprover, string Approver1, string Approver2, string Approver3, string Approver4, string Approver5, string Approver6)
        {
            var dt = _dtService.Usp_DT(new DTViewModel()
            {
                Activity = "UpdateApprover",
                AdId = User.Identity.GetADId(),
                Id = eApprId,
                CustomerName = CustomerName,
                PrimaryApprover = PrimaryApprover,
                Approver1 = Approver1,
                Approver2 = Approver2,
                Approver3 = Approver3,
                Approver4 = Approver4,
                Approver5 = Approver5,
                Approver6 = Approver6
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = dt.Rows[0]["MSG"].ToString();
                return Json(new { result = Common.ResultSuccess, msg = dt.Rows[0]["MSG"].ToString() });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        [HttpPost]
        public JsonResult DeleteApprover(int id)
        {
            var dt = _dtService.Usp_DT(new DTViewModel() { Activity = "DeleteApprover", Id = id });
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
        public JsonResult GetApproverById(int id)
        {
            return Json(Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetApproverById", Id = id })));
        }





        public IActionResult SETest()
        {
            ViewBag.TestStart = "NO";
            var data = new DTViewModel
            {
                ManPowerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPowerDDL" })),
                StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" })),
                CustomerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetApproverDDL" })),
                TestConductedByEmpName = User.Identity.GetEmpName(),
                TestConductedByEmpCode = User.Identity.GetEmpCode()
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult SETest(DTViewModel model)
        {
            ViewBag.TestStart = "NO";
            if (ModelState.IsValid)
            {
                ViewBag.TestStart = "YES";
                model.QuestionList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetQuestion", Station = model.TestStation, Grade = model.TestForGrade }));
                var conductTest = new DTViewModel
                {
                    TestEmpCode = model.TestEmpCode,
                    TestEmpName = model.TestEmpName,
                    TestStation = model.TestStation,
                    TestStationName = model.TestStationName,
                    TestCurrentGrade = model.TestCurrentGrade,
                    TestConductedByEmpCode = model.TestConductedByEmpCode,
                    TestConductedByEmpName = model.TestConductedByEmpName,
                    TestCustomer = model.TestCustomer,
                    TestCustomerName = model.TestCustomerName,
                    TestForGrade = model.TestForGrade,
                    QuestionList = model.QuestionList
                };
                TempData["Temp_SETestDraft"] = JsonConvert.SerializeObject(conductTest, Formatting.Indented);
                TempData.Keep(); //To maintain the state of TempData  
            }
            model.ManPowerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetActiveManPowerDDL" }));
            model.StationList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "54" }));
            model.CustomerDDLList = Common.BindDropDown(_dtService.Usp_DT(new DTViewModel() { Activity = "GetApproverDDL" }));
            model.TestConductedByEmpName = User.Identity.GetEmpName();
            model.TestConductedByEmpCode = User.Identity.GetEmpCode();
            return View(model);
        }
        [HttpPost]
        public JsonResult SetSelectedTestAns(int qid, string ans, string selectedAnsText)
        {
            int attemptedAns = 0;
            var model = new DTViewModel();
            if (TempData["Temp_SETestDraft"] != null)
            {
                model = JsonConvert.DeserializeObject<DTViewModel>(TempData["Temp_SETestDraft"].ToString());
                int totalNoOfQue = model.QuestionList.Count;
                foreach (var item in model.QuestionList)
                {
                    if (item.Id == qid)
                    {
                        item.SelectedAnsCode = ans;
                        item.SelectedAnsName = selectedAnsText;
                    }
                    if (!string.IsNullOrEmpty(item.SelectedAnsCode))
                    {
                        int PerQueCompletedPercentageValue = (int)Convert.ToInt64(Math.Ceiling(100 / Convert.ToDecimal(totalNoOfQue)));
                        attemptedAns = PerQueCompletedPercentageValue + attemptedAns;
                    }
                }
            }
            model.AttemptedAns = attemptedAns;
            TempData["Temp_SETestDraft"] = JsonConvert.SerializeObject(model, Formatting.Indented);
            TempData.Keep(); //To maintain the state of TempData
            return Json(model.AttemptedAns);
        }
        [HttpPost]
        public JsonResult SubmitTest()
        {
            var model = new DTViewModel();
            if (TempData["Temp_SETestDraft"] != null)
            {
                int testScoreOutof = 0;
                int testScore = 0;
                model = JsonConvert.DeserializeObject<DTViewModel>(TempData["Temp_SETestDraft"].ToString());
                foreach (var item in model.QuestionList)
                {
                    ++testScoreOutof;
                    if (item.Ans.Trim().ToUpper() == item.SelectedAnsCode.Trim().ToUpper())
                    {
                        ++testScore;
                        item.IsAnsCorrect = 1;
                    }
                }
                var testSummary = _dtService.Usp_DT(new DTViewModel()
                {
                    Activity = "AddSkillTest",
                    EmpCode = model.TestEmpCode,
                    EmpName = model.TestEmpName,
                    Param1 = model.TestStation,
                    Param2 = model.TestStationName,
                    Grade = model.TestForGrade,
                    CustomerId = model.TestCustomer,
                    CustomerName = model.TestCustomerName,
                    TestScore = testScore,
                    TestScoreOutof = testScoreOutof,
                    AdId = User.Identity.GetEmpCode()
                });

                foreach (var item1 in model.QuestionList)
                {
                    _dtService.Usp_DT(new DTViewModel()
                    {
                        Activity = "AddSkillTestDtl",
                        Id = Convert.ToInt32(testSummary.Rows[0]["RETVAL"]),
                        EmpCode = model.TestEmpCode,
                        EmpName = model.TestEmpName,
                        Param1 = model.TestStationName,
                        Grade = model.TestForGrade,
                        CustomerName = model.TestCustomerName,
                        Que = item1.Que,
                        SelectedAnsName = item1.SelectedAnsName,
                        Ans = item1.AnsText,
                        IsAnsCorrect = item1.IsAnsCorrect,
                        AdId = User.Identity.GetEmpCode()
                    });
                }
            }
            return Json("1");
        }
        [HttpPost]
        public JsonResult GetMPGrade(string empCode, string station)
        {
            return Json(Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetMPGrade", EmpCode = empCode, Station = station })));
        }





        public IActionResult ActionByQA()
        {
            ViewBag.SearchOption = "PendingViva";
            var data = new DTViewModel();
            if (TempData["Temp_GetPendingViva"] == null || TempData["Temp_GetPendingViva"].ToString() == "-1")
            {
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetPendingViva", EmpCode = User.Identity.GetEmpCode() }));                
            }
            else if (TempData["Temp_GetPendingViva"].ToString() == "1")
            {
                ViewBag.SearchOption = "Pass";
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetAllPassTest" }));
            }
            else if (TempData["Temp_GetPendingViva"].ToString() == "0")
            {
                ViewBag.SearchOption = "Fail";
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetAllFailTest" }));
            }
            else if (TempData["Temp_GetPendingViva"].ToString() == "2")
            {
                ViewBag.SearchOption = "AllTestResults";
                data.ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetAllTestResults" }));
            }
            return View(data);
        }
        [HttpPost]
        public JsonResult GetTestResult(string type)
        {
            TempData["Temp_GetPendingViva"] = type;
            return Json("");
        }
        [HttpPost]
        public JsonResult GetTestDetails(int id)
        {
            var data = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTestResultById", Id = id }));
            var data1 = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTestResultDtlById", Id = id }));
            return Json(new { data, data1 });
        }
        [HttpPost]
        public JsonResult QAAction(int id, string action)
        {
            var dt = _dtService.Usp_DT(new DTViewModel()
            {
                Activity = "UpdateTestStatus",
                Id = id,
                Param1 = action,
                Param2 = User.Identity.GetEmpCode() + " - " + User.Identity.GetEmpName(),
                AdId = User.Identity.GetEmpCode()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.ResultSuccessMessage });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }





        public IActionResult DueTest()
        {
            var data = new DTViewModel
            {
                ManPowerList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTestDueDateGradeC" })),
                QuestionList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTestDueDateGradeB" })),
                OJTDocumentList = Common.ConvertDataTableToList<DTViewModel>(_dtService.Usp_DT(new DTViewModel() { Activity = "GetTestDueDateGradeA" }))
            };
            return View(data);
        }




        public IActionResult SkillMatrix()
        {
            return View();
        }
        public IActionResult Reports()
        {
            return View();
        }      
    }
}
