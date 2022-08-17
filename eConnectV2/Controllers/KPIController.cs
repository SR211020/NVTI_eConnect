using eConnectV2.Extensions;
using eConnectV2.Filters;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Authorize]
    [ServiceFilter(typeof(ActionFilterConfig))]
    public class KPIController : Controller
    {
        private readonly IKPIService _kpiService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _iweb;

        public const string KpiUser = "USER", KpiDept = "DEPT", KpiOrg = "ORG";
        public const string KpiStatusOpen = "1", KpiStatusPWResDept = "2", KpiStatusPWHod = "3", KpiStatusApproved = "4", KpiStatusReject = "5";

        public KPIController(IKPIService kpiService, ICommonService commonService, IWebHostEnvironment iweb)
        {
            _kpiService = kpiService;
            _commonService = commonService;
            _iweb = iweb;
        }


        public IActionResult Dashboard()
        {
            var data = new KPIViewModel
            {
                FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }))
            };
            if (string.IsNullOrEmpty(data.FinYearCode))
            {
                var config = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetConfigValue", Param1 = "KPI", ConfigCategory1 = "DefaultFinYear" });
                data.DefaultFinYearCode = data.FinYearCode = config.Rows[0]["CONFIGVALUE"].ToString();
                data.Month = (DateTime.Now.AddMonths(-1).Month).ToString();
            }
            return View(data);
        }



        public IActionResult Add()
        {
            var data = new KPIViewModel
            {
                FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" })),
                DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" })),
                CategoryList = GetKPICategoryList()
            };

            var config = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetConfigValue", Param1 = "KPI", ConfigCategory1 = "DefaultFinYear" });
            data.FinYearCode = config.Rows[0]["CONFIGVALUE"].ToString();
            data.FinYearName = data.FinYearList.Where(x => x.Value == data.FinYearCode).FirstOrDefault().Text;

            data.DeptCode = data.RespDeptCode = User.Identity.GetDeptCode();
            data.HodName = _commonService.usp_Master(new CommonViewModel { Activity = "GET_HOD", Param1 = data.DeptCode, Plant = User.Identity.GetPlant() }).Rows[0]["HOD_NAME"].ToString();

            if (IsHod(User.Identity.GetADId()))
            {
                data.KpiType = KpiDept;
                data.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
            }
            else
            {
                data.KpiType = KpiUser;
            }
            Tuple<string, string> val = GetKPINo(data.KpiType, data.FinYearCode, data.DeptCode, User.Identity.GetEmpCode(), User.Identity.GetADId());
            data.KpiNo = val.Item1;
            data.KpiStatusCode = val.Item2;
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Add(KPIViewModel model, List<IFormFile> AttachDocument)
        {
            if (ModelState.IsValid)
            {
                if (IsHod(User.Identity.GetADId()))
                {
                    model.KpiType = KpiDept;
                    model.HodCode = User.Identity.GetEmpCode();
                    model.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
                }
                else
                {
                    model.KpiType = KpiUser;
                    model.HodCode = _commonService.usp_Master(new CommonViewModel { Activity = "GET_HOD", Param1 = model.DeptCode, Plant = User.Identity.GetPlant() }).Rows[0]["HOD_EMPCODE"].ToString();
                }
                //get kpi no, if already created
                Tuple<string, string> val = GetKPINo(model.KpiType, model.FinYearCode, model.DeptCode, User.Identity.GetEmpCode(), User.Identity.GetADId());
                model.KpiNo = val.Item1;
                model.KpiStatusCode = val.Item2;
                if (model.KpiNo == "0")
                {
                    var dt1 = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "InsertMstKPI", KpiType = model.KpiType, FinYearCode = model.FinYearCode, EmpCode = User.Identity.GetEmpCode(), DeptCode = model.DeptCode, HodCode = model.HodCode, KpiStatusCode = KpiStatusOpen, AdId = User.Identity.GetADId() });
                    if (dt1.Rows.Count > 0) { model.KpiNo = dt1.Rows[0]["KPINO"].ToString(); }
                }
                //insert into details table
                if (Convert.ToInt32(model.KpiNo) > 0)
                {
                    if (CheckWeightage(model.KpiNo, model.Weightage) > 100)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = "Weightage cannot be more than 100%";
                    }
                    else if (SaveKPIDetails(ref model))
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = "KPI added successfully";

                        string filepath = Common.FilePath + @"\Kpi\";

                        string docId = string.Empty;
                        foreach (IFormFile item in AttachDocument)
                        {
                            if (item.Length > 0)
                            {
                                string fileName = Path.GetFileName(item.FileName);
                                string extn = Path.GetExtension(item.FileName);
                                if (Common.IsValidExtnForKpi(extn))
                                {
                                    model.DocUId = Guid.NewGuid().ToString() + extn;
                                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), filepath, model.DocUId);
                                    using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                                    await item.CopyToAsync(fileStream);
                                    docId = model.DocUId;
                                }
                                else
                                {
                                    TempData["Result"] = Common.ResultError;
                                    TempData["Message"] = "Invalid File Format";
                                    return RedirectToAction(nameof(Add));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(docId)) { KpiDocUpdate(model); }
                    }
                }
            }
            model.FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }));
            model.DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" }));
            model.CategoryList = GetKPICategoryList();
            return View(model);
        }



        private static List<SelectListItem> GetKPICategoryList()
        {
            var data = new List<SelectListItem>
            {
                new SelectListItem() { Value = "1", Text = "Lower is better" },
                new SelectListItem() { Value = "2", Text = "Higher is better" },
                new SelectListItem() { Value = "3", Text = "0 for all Target" },
                new SelectListItem() { Value = "4", Text = "1 for all Target" },
                new SelectListItem() { Value = "5", Text = "100% for all Target" },
                new SelectListItem() { Value = "6", Text = "In terms of cost" }
            };
            return data;
        }
        private bool IsHod(string adId) // checked into mst department table
        {
            return Convert.ToBoolean(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "IsHod", AdId = adId }).Rows[0]["FLAG"]);
        }
        private bool IsKPIHod(string empCode) // checked into mst Config table
        {
            return Convert.ToBoolean(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "IsKPIHod", EmpCode = empCode }).Rows[0]["FLAG"]);
        }
        private decimal CheckWeightage(string kpiNo, string weightage)
        {
            decimal TotalWeightage = 0;
            var dt = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "CheckWeightage", KpiNo = kpiNo });
            foreach (DataRow dataRow in dt.Rows)
            {
                TotalWeightage += Convert.ToDecimal(dataRow["WEIGHTAGE"]);
            }
            TotalWeightage += Convert.ToDecimal(weightage);
            return TotalWeightage;
        }
        private bool SaveKPIDetails(ref KPIViewModel model)
        {
            var dt = _kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "AddKpiDetail",
                KpiNo = model.KpiNo.Trim(),
                KpiName = model.KpiName.Trim(),
                FinYearCode = model.FinYearCode.Trim(),
                KpiType = model.KpiType.Trim(),
                CategoryCode = model.CategoryCode.Trim(),
                Weightage = model.Weightage.Trim(),
                RespDeptCode = model.RespDeptCode.Trim(),
                IsKPIApprove = "Y",
                ApproverCode = model.HodCode,
                BottomTarget = model.BottomTarget,
                BasicTarget = model.BasicTarget,
                ChallengeTarget = model.ChallengeTarget
            });
            foreach (DataRow dataRow in dt.Rows)
            {
                model.SrNo = dataRow["SRNO"].ToString().Trim();
                return true;
            }
            return false;
        }
        private void KpiDocUpdate(KPIViewModel model)
        {
            _ = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "KpiDetailDocUpdate", KpiNo = model.KpiNo, SrNo = model.SrNo, DocUId = model.DocUId });
        }
        private Tuple<string, string> GetKPINo(string kpiType, string finYearCode, string deptCode, string empCode, string adId)
        {
            var dt = _kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "GetDraftKpiByFinYear",
                EmpCode = empCode,
                AdId = adId,
                KpiType = kpiType,
                FinYearCode = finYearCode,
                DeptCode = deptCode
            });
            if (dt.Rows.Count > 0) { return Tuple.Create(dt.Rows[0]["KPINO"].ToString(), dt.Rows[0]["STATUS"].ToString()); };
            return Tuple.Create("0", "0");
        }



        public IActionResult Computation()
        {
            var data = new KPIViewModel
            {
                FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }))
            };
            var config = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetConfigValue", Param1 = "KPI", ConfigCategory1 = "DefaultFinYear" });
            data.FinYearCode = config.Rows[0]["CONFIGVALUE"].ToString();
            data.DeptName = User.Identity.GetDeptName();
            data.DeptCode = User.Identity.GetDeptCode();
            data.EmpCode = User.Identity.GetEmpCode();
            //Case 1: Shirish Prasad || Nitika Arora
            if (data.EmpCode == "9016864" || data.EmpCode == "9022761")
            {
                data.KpiType = KpiDept;
                data.DeptCode = "DP0017";
                data.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetAllKPIDepts" }));
                data.EmployeeList.Clear();
                data.EmployeeList.Add(new SelectListItem() { Value = data.EmpCode, Text = (data.EmpCode + " - " + User.Identity.GetEmpName()) });
                //default data Bind
                data.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetViewKPI",
                    FinYearCode = data.FinYearCode,
                    DeptCode = data.DeptCode,
                    EmpCode = "0"
                }));
                return View(data);
            }
            //Case 2: Puran Singh
            if (data.EmpCode == "9024962")
            {
                data.KpiType = KpiDept;
                data.HodDepartmentList.Clear();
                data.HodDepartmentList.Add(new SelectListItem() { Text = "Production", Value = "DP0001" });
                data.HodDepartmentList.Add(new SelectListItem() { Text = "Engineering", Value = "DP0003" });
                data.HodDepartmentList.Add(new SelectListItem() { Text = "Maintenance", Value = "DP0004" });
                data.HodDepartmentList.Add(new SelectListItem() { Text = "Technical Service & Infrastructure", Value = "DP0007" });
                data.DeptCode = data.HodDepartmentList.FirstOrDefault().Value;
                data.DeptName = data.HodDepartmentList.FirstOrDefault().Text;
                //default data Bind
                data.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetViewKPI",
                    FinYearCode = data.FinYearCode,
                    DeptCode = data.DeptCode,
                    EmpCode = "0"
                }));
                data.EmployeeList.Clear();
                data.EmployeeList.Add(new SelectListItem() { Value = data.EmpCode, Text = (data.EmpCode + " - " + User.Identity.GetEmpName()) });
                return View(data);
            }
            //Case 3: KPI User and HOD
            if (IsKPIHod(User.Identity.GetEmpCode()))
            {
                data.KpiType = KpiDept;
                data.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
                string deptCode = string.Empty; // because hod can have multiple department
                deptCode = String.Join(",", data.HodDepartmentList.Select(x => x.Value).ToArray());
                data.DeptCode = data.HodDepartmentList.FirstOrDefault().Value;
                data.DeptName = data.HodDepartmentList.FirstOrDefault().Text;
                data.EmployeeList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetDeptEmp",
                    DeptCode = deptCode
                }));
                var IsLoggedEmpInList = data.EmployeeList.Where(x => x.Value == User.Identity.GetEmpCode());
                if (IsLoggedEmpInList != null && IsLoggedEmpInList.Any())
                {
                    //Logged HOD is exists in DDl EmpList
                }
                else
                {
                    data.EmployeeList.Add(new SelectListItem() { Value = data.EmpCode, Text = (data.EmpCode + " - " + User.Identity.GetEmpName()) });
                }
            }
            else
            {
                data.KpiType = KpiUser;
            }

            //default data Bind
            data.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "GetViewKPI",
                FinYearCode = data.FinYearCode,
                DeptCode = data.DeptCode,
                EmpCode = data.EmpCode
            }));
            return View(data);
        }
        [HttpPost]
        public IActionResult Computation(KPIViewModel model)
        {
            model.FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }));
            if (ModelState.IsValid)
            {
                string empCode = User.Identity.GetEmpCode();
                //Case 1: Shirish Prasad || Nitika Arora
                if (empCode == "9016864" || empCode == "9022761")
                {
                    model.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                    {
                        Activity = "GetViewKPI",
                        FinYearCode = model.FinYearCode,
                        DeptCode = model.DeptCode,
                        EmpCode = "0"
                    }));
                    model.KpiType = KpiDept;
                    model.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetAllKPIDepts" }));
                    model.EmployeeList.Clear();
                    model.EmployeeList.Add(new SelectListItem() { Value = empCode, Text = (empCode + " - " + User.Identity.GetEmpName()) });
                    return View(model);
                }

                //Case 2: Puran Singh
                if (empCode == "9024962")
                {
                    model.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                    {
                        Activity = "GetViewKPI",
                        FinYearCode = model.FinYearCode,
                        DeptCode = model.DeptCode,
                        EmpCode = "0"
                    }));
                    model.KpiType = KpiDept;
                    model.HodDepartmentList.Clear();
                    model.HodDepartmentList.Add(new SelectListItem() { Text = "Production", Value = "DP0001" });
                    model.HodDepartmentList.Add(new SelectListItem() { Text = "Engineering", Value = "DP0003" });
                    model.HodDepartmentList.Add(new SelectListItem() { Text = "Maintenance", Value = "DP0004" });
                    model.HodDepartmentList.Add(new SelectListItem() { Text = "Technical Service & Infrastructure", Value = "DP0007" });
                    model.DeptCode = model.HodDepartmentList.FirstOrDefault().Value;
                    model.DeptName = model.HodDepartmentList.FirstOrDefault().Text;
                    model.EmployeeList.Clear();
                    model.EmployeeList.Add(new SelectListItem() { Value = empCode, Text = (empCode + " - " + User.Identity.GetEmpName())});
                    return View(model);
                }

                //Case 3: KPI User and HODs
                model.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetViewKPI",
                    FinYearCode = model.FinYearCode,
                    DeptCode = model.DeptCode,
                    EmpCode = model.EmpCode
                }));                
            }
            if (IsKPIHod(User.Identity.GetEmpCode()))
            {
                model.KpiType = KpiDept;
                model.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
                string deptCode = string.Empty; // because hod can have multiple department
                deptCode = String.Join(",", model.HodDepartmentList.Select(x => x.Value).ToArray());
                model.DeptCode = model.HodDepartmentList.FirstOrDefault().Value;
                model.DeptName = model.HodDepartmentList.FirstOrDefault().Text;
                model.EmployeeList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetDeptEmp",
                    DeptCode = deptCode
                }));
                var IsLoggedEmpInList = model.EmployeeList.Where(x => x.Value == User.Identity.GetEmpCode());
                if (IsLoggedEmpInList != null && IsLoggedEmpInList.Any())
                {
                    //Logged HOD is exists in DDl EmpList
                }
                else
                {
                    model.EmployeeList.Add(new SelectListItem() { Value = model.EmpCode, Text = (model.EmpCode + " - " + User.Identity.GetEmpName()) });
                }
            }
            else
            {
                model.KpiType = KpiUser;
            }
            return View(model);
        }



        public IActionResult Manage()
        {
            var data = new KPIViewModel
            {
                FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }))
            };
            var config = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetConfigValue", Param1 = "KPI", ConfigCategory1 = "DefaultFinYear" });
            data.FinYearCode = config.Rows[0]["CONFIGVALUE"].ToString();
            data.DeptName = User.Identity.GetDeptName();
            if (IsHod(User.Identity.GetADId()))
            {
                string deptCode = string.Empty;
                data.KpiType = KpiDept;
                data.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
                if (data.HodDepartmentList.Count > 1)
                {
                    deptCode = String.Join(",", data.HodDepartmentList.Select(x => x.Value).ToArray());
                }
                else
                {
                    deptCode = data.HodDepartmentList.FirstOrDefault().Value;
                    data.DeptName = data.HodDepartmentList.FirstOrDefault().Text;
                }
                data.EmployeeList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetDeptEmp",
                    DeptCode = deptCode
                }));
                var IsLoggedEmpInList = data.EmployeeList.Where(x => x.Value == User.Identity.GetEmpCode());
                if (IsLoggedEmpInList != null && IsLoggedEmpInList.Count() > 0)
                {

                }
                else
                {
                    string empCode = User.Identity.GetEmpCode();
                    string empName = User.Identity.GetEmpName();
                    data.EmployeeList.Add(new SelectListItem() { Value = empCode, Selected = true, Text = (empCode + " - " + empName) });
                }
                if (deptCode.Contains(","))
                {
                    data.DeptCode = deptCode.Split(",")[0];
                    data.DeptName = data.HodDepartmentList.Where(x => x.Value == data.DeptCode).FirstOrDefault().Text;
                }
                else
                {
                    data.DeptCode = deptCode;
                }
            }
            else
            {
                data.KpiType = KpiUser;
                data.DeptCode = User.Identity.GetDeptCode();
            }
            data.EmpCode = User.Identity.GetEmpCode();
            //default data Bind
            if (data.DeptCode == "DP0017")
            {
                data.EmpCode = "0";
            }
            data.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "GetViewKPI",
                FinYearCode = data.FinYearCode,
                DeptCode = data.DeptCode,
                EmpCode = data.EmpCode
            }));
            return View(data);
        }
        [HttpPost]
        public IActionResult Manage(KPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.DeptCode == "DP0017")
                {
                    model.EmpCode = "0";
                }
                model.KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetViewKPI",
                    FinYearCode = model.FinYearCode,
                    DeptCode = model.DeptCode,
                    EmpCode = model.EmpCode
                }));
            }
            model.FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }));
            if (IsHod(User.Identity.GetADId()))
            {
                string deptCode = string.Empty;
                model.KpiType = KpiDept;
                model.HodDepartmentList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetHodDepts", Param1 = User.Identity.GetEmpCode() }));
                if (model.HodDepartmentList.Count > 1)
                {
                    deptCode = String.Join(",", model.HodDepartmentList.Select(x => x.Value).ToArray());
                }
                else
                {
                    deptCode = model.HodDepartmentList.FirstOrDefault().Value;
                    model.DeptName = model.HodDepartmentList.FirstOrDefault().Text;
                }
                model.EmployeeList = Common.BindDropDown(_kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "GetDeptEmp",
                    DeptCode = deptCode
                }));
                model.DeptCode = deptCode;
            }
            else
            {
                model.KpiType = KpiUser;
                model.DeptCode = User.Identity.GetDeptCode();
            }
            model.EmpCode = User.Identity.GetEmpCode();
            var IsLoggedEmpInList = model.EmployeeList.Where(x => x.Value == User.Identity.GetEmpCode());
            if (IsLoggedEmpInList != null && IsLoggedEmpInList.Count() > 0)
            {

            }
            else
            {
                string empCode = User.Identity.GetEmpCode();
                string empName = User.Identity.GetEmpName();
                model.EmployeeList.Add(new SelectListItem() { Value = empCode, Selected = true, Text = (empCode + " - " + empName) });
            }
            return View(model);
        }



        public IActionResult AddOrg()
        {
            var data = new KPIViewModel
            {
                FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" })),
                DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" })),
                CategoryList = GetKPICategoryList()
            };

            var config = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetConfigValue", Param1 = "KPI", ConfigCategory1 = "DefaultFinYear" });
            data.FinYearCode = config.Rows[0]["CONFIGVALUE"].ToString();
            data.FinYearName = data.FinYearList.Where(x => x.Value == data.FinYearCode).FirstOrDefault().Text;

            data.DeptCode = data.RespDeptCode = "DP0017"; //Management
            data.EmpName = data.HodName = _commonService.usp_Master(new CommonViewModel { Activity = "GET_HOD", Param1 = data.DeptCode, Plant = User.Identity.GetPlant() }).Rows[0]["HOD_NAME"].ToString();
            data.DeptName = "Management";
            data.KpiType = KpiOrg;

            Tuple<string, string> val = GetKPINo(data.KpiType, data.FinYearCode, data.DeptCode, "9016864", "NI00010");
            data.KpiNo = val.Item1;
            data.KpiStatusCode = val.Item2;
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrg(KPIViewModel model, List<IFormFile> AttachDocument)
        {
            if (ModelState.IsValid)
            {
                model.KpiType = KpiOrg;
                model.HodCode = model.EmpCode = "9016864";
                model.DeptCode = "DP0017"; //Management
                model.Status = KpiStatusOpen;
                model.HodCode = "9016864"; //Shirish Prasad
                model.AdId = "NI00010";
                //get kpi no, if already created
                Tuple<string, string> val = GetKPINo(model.KpiType, model.FinYearCode, model.DeptCode, model.EmpCode, model.AdId);
                model.KpiNo = val.Item1;
                model.KpiStatusCode = val.Item2;
                if (model.KpiNo == "0")
                {
                    var dt1 = _kpiService.Usp_KPI(new KPIViewModel()
                    {
                        Activity = "InsertMstKPI",
                        KpiType = model.KpiType,
                        FinYearCode = model.FinYearCode,
                        EmpCode = model.EmpCode,
                        DeptCode = model.DeptCode,
                        HodCode = model.HodCode,
                        KpiStatusCode = model.Status,
                        AdId = model.AdId
                    });
                    if (dt1.Rows.Count > 0) { model.KpiNo = dt1.Rows[0]["KPINO"].ToString(); }
                }
                //insert into details table
                if (Convert.ToInt32(model.KpiNo) > 0)
                {
                    if (CheckWeightage(model.KpiNo, model.Weightage) > 100)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = "Weightage cannot be more than 100%";
                    }
                    else if (SaveKPIDetails(ref model))
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = "KPI added successfully";

                        string filepath = Common.FilePath + @"\Kpi\";

                        string docId = string.Empty;
                        foreach (IFormFile item in AttachDocument)
                        {
                            if (item.Length > 0)
                            {
                                string fileName = Path.GetFileName(item.FileName);
                                string extn = Path.GetExtension(item.FileName);
                                if (Common.IsValidExtnForKpi(extn))
                                {
                                    model.DocUId = Guid.NewGuid().ToString() + extn;
                                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), filepath, model.DocUId);
                                    using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                                    await item.CopyToAsync(fileStream);
                                    docId = model.DocUId;
                                }
                                else
                                {
                                    TempData["Result"] = Common.ResultError;
                                    TempData["Message"] = "Invalid File Format";
                                    return RedirectToAction(nameof(Add));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(docId)) { KpiDocUpdate(model); }
                    }
                }
            }
            model.FinYearList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "4" }));
            model.DepartmentList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" }));
            model.CategoryList = GetKPICategoryList();
            return View(model);
        }



        [HttpPost]
        public JsonResult GetMyDraftKpis(string finYear, string deptCode)
        {
            string kpiType;
            string empCode = User.Identity.GetEmpCode();
            string adId = User.Identity.GetADId();
            if (deptCode == "DP0017")
            {
                kpiType = KpiOrg;
                empCode = "9016864";
                adId = "NI00010";
            }
            else
            {
                kpiType = IsHod(User.Identity.GetADId()) ? KpiDept : KpiUser;
            }
            Tuple<string, string> val = GetKPINo(kpiType, finYear, deptCode, empCode, adId);
            string kpiNo = val.Item1;
            string status = val.Item2;
            var data = new KPIViewModel
            {
                KpiList = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetKpiDetails", KpiNo = kpiNo, FinYearCode = finYear })),
                KpiStatusCode = status,
                KpiNo = kpiNo
            };
            return Json(data);
        }
        [HttpPost]
        public JsonResult RemoveKpi(string kpiNo, string srNo, string finYear)
        {
            var docId = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "GetKpiDetails", KpiNo = kpiNo, SrNo = srNo, FinYearCode = finYear })).FirstOrDefault().DocUId;
            var data = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "RemoveKpiDetail", KpiNo = kpiNo, SrNo = srNo }));
            if (!string.IsNullOrEmpty(docId))
            {
                string filepath = Common.FilePath + @"\Kpi\";
                string img = Path.Combine(_iweb.WebRootPath, filepath, docId);
                FileInfo fi = new FileInfo(img);
                if (fi != null)
                {
                    System.IO.File.Delete(img);
                    fi.Delete();
                }
            }
            return Json(data);
        }
        [HttpPost]
        public JsonResult SubmitKpi(string kpiNo, string kpiType)
        {
            string status = KpiStatusApproved;
            var result = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel() { Activity = "SubmitKpi", KpiNo = kpiNo, KpiStatusCode = status }));
            return Json(result);
        }
        [HttpPost]
        public JsonResult KpiUpdateTraget(string btm, string bs, string chl, string kpino, string srno, string month)
        {
            _ = _kpiService.Usp_KPI(new KPIViewModel() { Activity = "ChangeKpiTarget", KpiNo = kpino, SrNo = srno, BottomTarget = btm, BasicTarget = bs, ChallengeTarget = chl, Month = month });
            return Json(true);
        }
        [HttpPost]
        public JsonResult KpiDetailsInput(List<string>[] row)
        {
            bool isSuccess = false; ;
            double calc;
            foreach (var item in row)
            {
                calc = GetKpiCalc(
                    Convert.ToInt32(item[7]),
                    Convert.ToDouble(item[2]),
                    Convert.ToDouble(item[4]),
                    Convert.ToDouble(item[5]),
                    Convert.ToDouble(item[6]),
                    Convert.ToDouble(item[8])
                    );
                var result = _kpiService.Usp_KPI(new KPIViewModel()
                {
                    Activity = "KpiDetailsInput",
                    KpiNo = item[0],
                    SrNo = item[1],
                    InputVal = item[2],
                    Month = item[3],
                    Calc = calc
                });
                isSuccess = true;
            }
            if (isSuccess)
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultSuccessMessage;
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return Json(isSuccess);
        }
        private double GetKpiCalc(int kpiType, double actualValue, double btmTarget, double bscTarget, double chlTarget, double weightage)
        {

            double score = 0;
            if (kpiType == 1 || kpiType == 6) // lower is better --  In terms of cost
            {
                if (actualValue > btmTarget)
                {
                    score = 0;
                }
                else
                {
                    if (actualValue > bscTarget)
                    {
                        double uv = actualValue - btmTarget;
                        uv *= 20;
                        double lv = bscTarget - btmTarget;

                        double clpercent = (uv / lv);
                        score = 60 + (clpercent);
                    }
                    else
                    {
                        if (actualValue > chlTarget)
                        {
                            double uv = actualValue - bscTarget;
                            uv *= 20;
                            double lv = chlTarget - bscTarget;
                            double clpercent = (uv / lv);
                            score = 80 + (clpercent);
                        }
                        else
                        {
                            score = 100;
                        }
                    }
                }
            }
            else if (kpiType == 2) // higher is better
            {
                if (actualValue < btmTarget)
                {
                    score = 0;
                }
                else
                {
                    if (actualValue < bscTarget)
                    {
                        double uv = actualValue - btmTarget;
                        uv *= 20;
                        double lv = bscTarget - btmTarget;

                        double clpercent = (uv / lv);
                        score = 60 + (clpercent);
                    }
                    else
                    {
                        if (actualValue < chlTarget)
                        {
                            double uv = actualValue - bscTarget;
                            uv *= 20;
                            double lv = chlTarget - bscTarget;
                            double clpercent = (uv / lv);
                            score = 80 + (clpercent);
                        }
                        else
                        {
                            score = 100;
                        }
                    }
                }
            }
            else if (kpiType == 3) // 0 for all Target
            {
                if (actualValue == 0)
                {
                    score = 100;
                }
                else
                {
                    score = 0;
                }
            }
            else if (kpiType == 4) // 1 for all Target
            {
                if (actualValue == 1)
                {
                    score = 100;
                }
                else
                {
                    score = 0;
                }
            }
            else if (kpiType == 5) // 100% for all Target
            {
                if (actualValue == 100)
                {
                    score = 100;
                }
                else
                {
                    score = 0;
                }
            }
            score = (score * weightage) / 100;
            return score;
        }
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
            {
                return Content("filename not present");
            }
            string filepath = Common.FilePath + @"\Kpi\";
            string path = Path.Combine(Directory.GetCurrentDirectory(), filepath, filename);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, Common.GetContentType(path), Path.GetFileName(path));
        }
        [HttpPost]
        public JsonResult GetOrgData(string finYear)
        {
            var data = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "GET_DASHBOARD_ORG",
                FinYearCode = finYear
            }));
            return Json(data);
        }
        [HttpPost]
        public JsonResult GetDeptData(string finYear, string month)
        {
            var data = Common.ConvertDataTableToList<KPIViewModel>(_kpiService.Usp_KPI(new KPIViewModel()
            {
                Activity = "GET_DASHBOARD_DEPT",
                FinYearCode = finYear,
                Month = month
            }));
            return Json(data);
        }
    }
}
