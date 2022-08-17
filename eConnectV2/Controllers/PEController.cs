using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class PEController : Controller
    {

        private readonly IPEService _peService;
        private readonly ICommonService _commonService;
        public PEController(IPEService peService, ICommonService commonService)
        {
            _peService = peService;
            _commonService = commonService;
        }

        #region
        public IActionResult RedBin()
        {
            var model = new PEViewModel();
            model.LineNoList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "44" }));
            model.MCatgList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "47" }));
            model.BinList = Common.ConvertDataTableToList<PEViewModel>(_peService.usp_PE(new PEViewModel { Activity = "FILL_SCRAP_LIST" }));
            BindModelNameList(ref model);
            BindScrapSubCatgList(ref model);
            DataTable dt = _peService.usp_PE(new PEViewModel { Activity = "CHECK_REDBIN_ADMIN", UserId = User.Identity.GetADId() });
            if (dt != null && dt.Rows.Count > 0)
            {
                int flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (flag == 1)
                {
                    ViewBag.Admin = "Y";
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult RedBin(PEViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _peService.usp_PE(new PEViewModel()
                {
                    Activity = "ADD_SCRAP",
                    Customer = model.Customer,
                    ModelName = model.ModelName,
                    Line = model.Line,
                    ScrapCatg = model.ScrapCatg,
                    ScrapSubCatg = model.ScrapSubCatg,
                    ScrapDate = model.ScrapDate,
                    MCatg = model.MCatg,
                    Shift = model.Shift,
                    BarCode = model.BarCode.Trim().ToUpper(),
                    Remarks = model.Remarks.Trim(),
                    Param1 = model.OperatorId.Trim().ToUpper(),
                    UserId = User.Identity.GetADId()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.BarcodeAlreadyExist;
                        model.LineNoList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "44" }));
                        model.MCatgList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "47" }));
                        model.BinList = Common.ConvertDataTableToList<PEViewModel>(_peService.usp_PE(new PEViewModel { Activity = "FILL_SCRAP_LIST" }));
                        BindModelNameList(ref model, model.Customer);
                        BindScrapSubCatgList(ref model, model.ScrapCatg);
                        return View(model);
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
            return RedirectToAction(nameof(RedBin));
        }

        public IActionResult RedBinReports()
        {
            var model = new PEViewModel();
            DateTime dtToday = DateTime.Now;
            if (dtToday.Month == 1 || dtToday.Month == 2 || dtToday.Month == 3)
            {
                model.Date1 = new DateTime(dtToday.Year - 1, 4, 1).ToString("dd'-'MMM'-'yyyy");
            }
            else
            {
                model.Date1 = new DateTime(dtToday.Year, 4, 1).ToString("dd'-'MMM'-'yyyy");
            }
            model.Date2 = dtToday.ToString("dd'-'MMM'-'yyyy");
            model.CustomerList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = 42 }));
            return View(model);
        }
        [HttpPost]
        public JsonResult ReportsData(string date1, string date2)
        {
            DataTable dt = _peService.usp_PE(new PEViewModel() { Activity = "GET_REDBIN_CHART", Date1 = date1, Date2 = date2 });
            return Json(Common.ConvertDataTableToList<PEViewModel>(dt));
        }

        public IActionResult ProdQtyAdd()
        {
            var model = new PEViewModel();
            model.CustomerList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = 42 }));
            model.ProdList = Common.ConvertDataTableToList<PEViewModel>(_peService.usp_PE(new PEViewModel { Activity = "FILL_PROD_LIST" }));
            return View(model);
        }

        [HttpPost]
        public IActionResult ProdQtyAdd(PEViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _peService.usp_PE(new PEViewModel()
                {
                    Activity = "ADD_PROD_QTY",
                    Customer = model.Customer,
                    ProdDate = model.ProdDate,
                    Quantity = model.Quantity,
                    UserId = User.Identity.GetADId()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.CustomerList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = 42 }));
                        model.ProdList = Common.ConvertDataTableToList<PEViewModel>(_peService.usp_PE(new PEViewModel { Activity = "FILL_PROD_LIST" }));
                        return View(model);
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
            return RedirectToAction(nameof(ProdQtyAdd));
        }

        private PEViewModel BindModelNameList(ref PEViewModel model, string customer = null)
        {
            model.CustomerList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = 42 }));
            if (!string.IsNullOrEmpty(customer))
            {
                model.ModelNameList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = 43, Param1 = model.Customer }));
            }
            return model;
        }
        private PEViewModel BindScrapSubCatgList(ref PEViewModel model, string scrapCatg = null)
        {
            model.ScrapCatgList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = 45 }));
            if (!string.IsNullOrEmpty(scrapCatg))
            {
                model.ScrapSubCatgList = Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = 46, Param1 = model.ScrapCatg }));
            }
            return model;
        }

        [HttpPost]
        public JsonResult GetModelName(string customer)
        {
            return Json(Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = 43, Param1 = customer })));
        }

        [HttpPost]
        public JsonResult GetScrapSubCatg(string scrapCatg)
        {
            return Json(Common.BindDropDown(_peService.usp_PE(new PEViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = 46, Param1 = scrapCatg })));
        }
        [HttpPost]
        public JsonResult DeleteRedBinData(string barcode)
        {
            _peService.usp_PE(new PEViewModel { Activity = "DELETE_SCRAP", BarCode = barcode });
            return Json("OK");
        }
        #endregion
    }
}
