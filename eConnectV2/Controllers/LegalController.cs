using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class LegalController : Controller
    {
        private readonly ILegalService _legalService;
        private readonly ICommonService _commonService;
        private SqlCommand Cmd { get; set; }
        public LegalController(ILegalService legalService, ICommonService commonService)
        {
            _legalService = legalService;
            _commonService = commonService;
        }

        public IActionResult ContractAdd(string Dno, string Mode)
        {
            //int DocNo = Dno != null ? Convert.ToInt32(Common.Decrypt(HttpContext.Request.Query["Dno"])) : 0;
            int DocNo = Dno != null ? Convert.ToInt32(HttpContext.Request.Query["Dno"]) : 0;
            var model = new LegalViewModel
            {
                DDLDeptList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_DEPT" })),
                DDLCategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "36" })),
                ContractHistory = Common.ConvertDataTableToList<LegalViewModel>(_legalService.usp_Legal(new LegalViewModel() { Activity = "GET_HISTORY", DocNo = DocNo }))
            };
            if (DocNo != 0)
            {
                DataTable dt = _legalService.usp_Legal(new LegalViewModel { Activity = "GET_CONTRACT_BY_DOC", DocNo = DocNo });
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                    model.Vendor = Convert.ToString(dt.Rows[0]["VENDOR"]);
                    model.DeptCode = Convert.ToString(dt.Rows[0]["DEPTCODE"]);
                    model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                    model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                    model.ContractCategory = Convert.ToString(dt.Rows[0]["CONTRACT_CATG"]);
                    model.ContractName = Convert.ToString(dt.Rows[0]["CONTRACT_NAME"]);
                    model.ContractNature = Convert.ToString(dt.Rows[0]["CONTRACT_NATURE"]);
                    model.ContractValue = Convert.ToString(dt.Rows[0]["CONTRACT_VALUE"]);
                    model.Currency = Convert.ToString(dt.Rows[0]["CURRENCY"]);
                    model.MasterContract = Convert.ToString(dt.Rows[0]["MASTER_CONTRACT"]);
                    model.ContractNumber = Convert.ToString(dt.Rows[0]["CONTRACT_NUMBER"]);
                    model.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                    model.ExeDate = Convert.ToString(dt.Rows[0]["EXE_DATE"]);
                    model.EffDate = Convert.ToString(dt.Rows[0]["EFF_DATE"]);
                    model.ExpDate = Convert.ToString(dt.Rows[0]["EXP_DATE"]);
                    model.DocDraft = Convert.ToString(dt.Rows[0]["DOC_DRAFT"]);
                    model.DocSigned = Convert.ToString(dt.Rows[0]["DOC_SIGNED"]);
                    model.Remark = Convert.ToString(dt.Rows[0]["REMARK"]);
                    model.ServiceDescription = Convert.ToString(dt.Rows[0]["SERVICE_DESC"]);
                    model.UserId = Convert.ToString(dt.Rows[0]["CREATEDBY"]);
                    //ViewBag.TagAttribute = "V";
                    if (Mode == "E")
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
                    ViewBag.TagAttribute = "E";
                }
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult ContractAdd(LegalViewModel model, IFormFile DocDraft, IFormFile DocSigned, string submit)
        {
            string filepath1 = Common.FilePath + @"\Legal\Draft\";
            string filepath2 = Common.FilePath + @"\Legal\Signed\";
            string file1 = "";
            string file2 = "";
            if (ModelState.IsValid)
            {
                if (DocDraft != null)
                {
                    string fileName = Path.GetFileName(DocDraft.FileName);
                    string actext = fileName.Split(".")[1];
                    file1 = User.Identity.GetADId() + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + actext;
                    bool save = Common.SaveFile1(DocDraft, filepath1, file1);
                }
                else
                {
                    file1 = model.Param1;
                }
                if (DocSigned != null)
                {
                    string fileName = Path.GetFileName(DocSigned.FileName);
                    string actext = fileName.Split(".")[1];
                    file2 = User.Identity.GetADId() + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + actext;
                    bool save = Common.SaveFile1(DocSigned, filepath2, file2);
                }
                else
                {
                    file2 = model.Param2;
                }
                DataTable dt = _legalService.usp_Legal(new LegalViewModel
                {
                    Activity = "ADD_UPD_CONTRACT",
                    DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0,
                    ContractNumber = model.ContractNumber,
                    ContractValue = model.ContractValue,
                    Currency = model.Currency,
                    ContractCategory = model.ContractCategory,
                    ContractName = model.ContractName,
                    ContractNature = model.ContractNature,
                    Vendor = model.Vendor,
                    Plant = model.Plant,
                    DeptCode = model.DeptCode,
                    Email = model.Email,
                    ExeDate = model.ExeDate != null ? model.ExeDate : "",
                    EffDate = model.EffDate != null ? model.EffDate : "",
                    ExpDate = model.ExpDate != null ? model.ExpDate : "",
                    MasterContract = model.MasterContract.Trim(),
                    DocDraft = file1,
                    DocSigned = file2,
                    Remark = model.Remark,
                    ServiceDescription = model.ServiceDescription,
                    UserId = User.Identity.GetADId()
                });
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
                        return RedirectToAction(nameof(ContractView));
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
            return RedirectToAction(nameof(ContractAdd));
        }

        public IActionResult ContractView()
        {
            var data = new LegalViewModel()
            {
                ContractList = Common.ConvertDataTableToList<LegalViewModel>(_legalService.usp_Legal(new LegalViewModel() { Activity = "GET_CONTRACT_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult ContractView(LegalViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ContractList = Common.ConvertDataTableToList<LegalViewModel>(_legalService.usp_Legal(new LegalViewModel() { Activity = "GET_FILTERED_LIST", Date1 = model.Date1, Date2 = model.Date2, Date3 = model.Date3, Date4 = model.Date4, Date5 = model.Date5, Date6 = model.Date6 }));
                return View(model);
            }
            return View(model);
        }
        public JsonResult RemoveContract(int docNo)
        {
            var data = _legalService.usp_Legal(new LegalViewModel() { Activity = "REMOVE_CONTRACT", DocNo = docNo });
            return Json("OK");
        }
        public FileResult DownloadLegalDoc(string fileName)
        {
            string path = Common.FilePath + @"\Legal\Draft\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadLegalSigned(string fileName)
        {
            string path = Common.FilePath + @"\Legal\Signed\" + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
