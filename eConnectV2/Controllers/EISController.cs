using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Data;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using eConnectV2.Helpers;
using eConnectV2.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eConnectV2.Controllers
{
    public class EISController : Controller
    {
        private readonly IEISService _eisService;
        private readonly ICommonService _commonService;
        public EISController(IEISService eisService, ICommonService commonService)
        {
            _eisService = eisService;
            _commonService = commonService;
        }

        public IActionResult EISDashboard()
        {
            #region Fill Controls
            var model = new EISViewModel();
            model.MonthList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "7" }));
            DateTime dtToday = DateTime.Now;
            model.SqDate1 = dtToday.AddDays(-7).ToString("yyyy'-'MM'-'dd");
            model.SqDate2 = dtToday.AddDays(-1).ToString("yyyy'-'MM'-'dd");

            if (dtToday.Month == 1 || dtToday.Month == 2 || dtToday.Month == 3 || dtToday.Month == 4)  //Because From Month is 4 month less to current Month
            {
                model.NPFromYr3 = model.OPFromYr3 = model.GPFromYr3 = model.NPFromYr2 = model.OPFromYr2 = model.GPFromYr2 = model.NPFromYr
                = model.OPFromYr = model.GPFromYr = model.TaxFromYr = model.OicFromYr = model.OtcFromYr = model.FrxFromYr = model.OFRFromYr = model.RlyFromYr = model.IDLFromYr = model.SGAFromYr
                = model.IFRFromYr = model.MOHFromYr = model.DLFromYr = model.DepFromYr = model.MatFromYr = model.ScrFromYr = model.PackRevFromYr = model.CellRevFromYr = model.SalesFromYr
                = dtToday.AddYears(-1).Year;
            }
            else
            {
                model.NPFromYr3 = model.OPFromYr3 = model.GPFromYr3 = model.NPFromYr2 = model.OPFromYr2 = model.GPFromYr2 = model.NPFromYr
                = model.OPFromYr = model.GPFromYr = model.TaxFromYr = model.OicFromYr = model.OtcFromYr = model.FrxFromYr = model.OFRFromYr = model.RlyFromYr = model.IDLFromYr = model.SGAFromYr 
                = model.IFRFromYr = model.MOHFromYr = model.DLFromYr = model.DepFromYr = model.MatFromYr = model.ScrFromYr = model.PackRevFromYr = model.CellRevFromYr = model.SalesFromYr 
                = dtToday.Year;
            }
            model.NPFromMon3 = model.OPFromMon3 = model.GPFromMon3 = model.NPFromMon2 = model.OPFromMon2 = model.GPFromMon2 = model.NPFromMon
            = model.OPFromMon = model.GPFromMon = model.TaxFromMon = model.OicFromMon = model.OtcFromMon = model.FrxFromMon = model.OFRFromMon = model.RlyFromMon = model.IDLFromMon = model.SGAFromMon 
            = model.IFRFromMon = model.MOHFromMon = model.DLFromMon = model.DepFromMon = model.MatFromMon = model.ScrFromMon = model.PackRevFromMon = model.CellRevFromMon = model.SalesFromMon 
            = dtToday.AddMonths(-4).Month;

            model.NPToYr3 = model.OPToYr3 = model.GPToYr3 = model.NPToYr2 = model.OPToYr2 = model.GPToYr2 = model.NPToYr
            = model.OPToYr = model.GPToYr = model.TaxToYr = model.OicToYr = model.OtcToYr = model.FrxToYr = model.OFRToYr = model.RlyToYr = model.IDLToYr = model.SGAToYr = model.IFRToYr = model.MOHToYr 
            = model.DLToYr = model.DepToYr = model.MatToYr = model.ScrToYr = model.PackRevToYr = model.CellRevToYr = model.SalesToYr = dtToday.Month == 1 ? dtToday.AddYears(-1).Year : dtToday.Year;
            
            model.NPToMon3 = model.OPToMon3 = model.GPToMon3 = model.NPToMon2 = model.OPToMon2 = model.GPToMon2 = model.NPToMon
            = model.OPToMon = model.GPToMon = model.TaxToMon = model.OicToMon = model.OtcToMon = model.FrxToMon = model.OFRToMon = model.RlyToMon = model.IDLToMon = model.SGAToMon = model.IFRToMon 
            = model.MOHToMon = model.DLToMon = model.DepToMon = model.MatToMon = model.ScrToMon = model.PackRevToMon = model.CellRevToMon = model.SalesToMon = dtToday.AddMonths(-1).Month;

            model.OQADate2 = model.OQADate1 = model.InvFGDate2 = model.InvFGDate1 = model.InvAmtDate2 = model.InvAmtDate1 = model.InvDate2 = model.InvDate1 = dtToday.Date.ToString("yyyy'-'MM'-'dd");
            model.UPHDate2 = model.UPPHDate2 = model.UPHDate1 = model.UPPHDate1 = dtToday.Date.AddDays(-1).ToString("yyyy'-'MM'-'dd");
            model.ALYear = dtToday.Year.ToString();
            model.ALMonth = dtToday.Month.ToString();
            #endregion

            #region Box Data
            model.Plant = "NVTI";
            model.Activity = "GET_TOTAL_SHIPMENT";
            DataTable dt1 = _eisService.usp_EIS(model);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                model.Box1 = Convert.ToString(dt1.Rows[0]["TOTAL_QTY"]) != "" ? Convert.ToString(dt1.Rows[0]["TOTAL_QTY"]) : "0";
                model.Box2 = Convert.ToString(dt1.Rows[0]["TOTAL_AMT"]) != "" ? Convert.ToString(dt1.Rows[0]["TOTAL_AMT"]) : "0";
            }
            else
            {
                model.Box1 = "0";
                model.Box2 = "0";
            }

            model.Activity = "GET_TOTAL_INVENTORY";
            DataTable dt2 = _eisService.usp_EIS(model);
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                model.Box3 = Convert.ToString(dt2.Rows[0]["TOTAL_AMT"]) != "" ? Convert.ToString(dt2.Rows[0]["TOTAL_AMT"]) : "0";
            }
            else
            {
                model.Box3 = "0";
            }
            #endregion

            #region Shipment Quantity & Amount
            model.Activity = "GET_MATCODE";
            model.MatCodeList = Common.BindDropDown(_eisService.usp_EIS(model));
            ViewBag.ShipmentData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_SHIPMENT_DETAIL", Plant = "NVTI", Param1 = "0", Date1 = model.SqDate1, Date2 = model.SqDate2 }));
            #endregion

            #region Revenue Data (Cell & Pack)
            ViewBag.RevenueData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_REVENUE_ALL_DATA", FromYear = model.CellRevFromYr, FromMonth = model.CellRevFromMon, ToYear = model.CellRevToYr, ToMonth = model.CellRevToMon }));
            #endregion

            #region RFC All Heads Data
            ViewBag.RFCAllData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_RFC_ALL_DATA", FromYear = model.SalesFromYr, FromMonth = model.SalesFromMon, ToYear = model.SalesToYr, ToMonth = model.SalesToMon }));
            #endregion

            #region Outward Freight Data
            ViewBag.OFRData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_OFR_DATA", FromYear = model.OFRFromYr, FromMonth = model.OFRFromMon, ToYear = model.OFRToYr, ToMonth = model.OFRToMon }));
            #endregion

            #region Profit Data (GP,OP,NP)
            ViewBag.ProfitData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT", FromYear = model.GPFromYr, FromMonth = model.GPFromMon, ToYear = model.GPToYr, ToMonth = model.GPToMon }));
            #endregion

            #region Percent Sale Data (GP,OP,NP)
            ViewBag.PercentSaleData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT_PER_SALE", FromYear = model.GPFromYr2, FromMonth = model.GPFromMon2, ToYear = model.GPToYr2, ToMonth = model.GPToMon2 }));
            #endregion

            #region Percent Without Cell Data (GP,OP,NP)
            ViewBag.PercentWithoutCellData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT_PER_WITHOUT_CELL", FromYear = model.GPFromYr3, FromMonth = model.GPFromMon3, ToYear = model.GPToYr3, ToMonth = model.GPToMon3 }));
            #endregion

            //#region Inventory Quantity & Amount
            //ViewBag.InventoryData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_INV_DETAIL", Plant = "NVTI", Division = "PACK", Date1 = model.InvDate1, Date2 = model.InvDate2 }));
            //ViewBag.InventoryFGData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_INV_FG", Plant = "NVTI", Division = "PACK", Date1 = model.InvFGDate1, Date2 = model.InvFGDate2 }));
            //#endregion

            //#region OQA Confirmation
            //ViewBag.OQAConfirmationData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_OQA_CONFIRM", Plant = "NVTI", Date1 = model.OQADate1, Date2 = model.OQADate2 }));
            //#endregion

            #region UPH & UPPH
            model.LineList2 = model.LineList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "5" }));
            model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_KEYID", DocNo = "6" }));
            ViewBag.UPHData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_UPH", Plant = "NVTI", Division = "PACK", Date1 = model.UPHDate1, Date2 = model.UPHDate2, Param1 = "0" }));
            ViewBag.UPPHData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_UPPH", Plant = "NVTI", Division = "PACK", Date1 = model.UPPHDate1, Date2 = model.UPPHDate1, Param1 = "0", Param2 = "0" }));
            #endregion

            #region Auto LU
            ViewBag.AutoLUData = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_AUTOLU", Plant = "BAWAL", Param1 = model.ALYear, Param2 = model.ALMonth }));
            #endregion

            return View(model);
        }

        public JsonResult GetShipmentData(string plant, string materialCode, string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_SHIPMENT_DETAIL", Plant = plant, Param1 = materialCode, Date1 = date1, Date2 = date2 }));
            return Json(data);
        }
        public JsonResult GetRevenueDataByCategory(string category, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_REVENUE_DATA_BY_CATG", Param1 = category, FromYear = fromYear, FromMonth = fromMonth, ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        public JsonResult GetRFCDataByCategory(string category, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_RFC_DATA_BY_CATG", Param1 = category, FromYear = fromYear, FromMonth = fromMonth,  ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        public JsonResult GetOFRData(int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_OFR_DATA", FromYear = fromYear, FromMonth = fromMonth, ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        public JsonResult GetProfitData(int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT", FromYear = fromYear, FromMonth = fromMonth, ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        public JsonResult GetProfitPercentSaleData(int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT_PER_SALE", FromYear = fromYear, FromMonth = fromMonth, ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        public JsonResult GetProfitPercentWithoutCellData(int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_PROFIT_PER_WITHOUT_CELL", FromYear = fromYear, FromMonth = fromMonth, ToYear = toYear, ToMonth = toMonth }));
            return Json(data);
        }
        //public JsonResult GetInventoryData(string plant, string division, string date1, string date2)
        //{
        //    var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_INV_DETAIL", Plant = plant, Division = division, Date1 = date1, Date2 = date2 }));
        //    return Json(data);
        //}
        //public JsonResult GetInvFGData(string plant, string division, string date1, string date2)
        //{
        //    var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_INV_FG", Plant = plant, Division = division, Date1 = date1, Date2 = date2 }));
        //    return Json(data);
        //}
        //public JsonResult GetOQAConfirmationData(string date1, string date2, string plant)
        //{
        //    var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_OQA_CONFIRM", Plant = plant, Date1 = date1, Date2 = date2 }));
        //    return Json(data);
        //}
        public JsonResult GetUPHData(string plant, string division, string date1, string date2, string lineType)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_UPH", Plant = plant, Division = division, Date1 = date1, Date2 = date2, Param1 = lineType }));
            return Json(data);
        }
        public JsonResult GetUPPHData(string plant, string division, string date1, string date2, string lineType, string cust)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_UPPH", Plant = plant, Division = division, Date1 = date1, Date2 = date2, Param1 = lineType, Param2 = cust }));
            return Json(data);
        }
        public JsonResult GetAutoLUData(string plant, string year, string month)
        {
            var data = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS(new EISViewModel() { Activity = "GET_AUTOLU", Plant = plant, Param1 = year, Param2 = month }));
            return Json(data);
        }


        #region Data Entry Screens
        #region Manage Revenue
        public IActionResult ManageRevenue()
        {
            var data = new EISViewModel()
            {
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_REVENUE_DATA" }))
            };
            return View(data);
        }

        [HttpPost]
        public IActionResult ManageRevenue(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_REVENUE";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Yr = model.Yr;
                model.Mon = model.Mon;
                model.Param1 = model.Category;
                model.Amount = model.Amount;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(ManageRevenue));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(ManageRevenue));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(ManageRevenue));
        }

        [HttpPost]
        public JsonResult FillRevenueByDocNo(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel { Activity = "FILL_REVENUE_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Yr = Convert.ToInt32(dt.Rows[0]["YR"]);
                model.Mon = Convert.ToInt32(dt.Rows[0]["MON"]);
                model.Category = Convert.ToString(dt.Rows[0]["CATEGORY"]);
                model.Amount = Convert.ToDecimal(dt.Rows[0]["AMOUNT"]);
            }
            return Json(model);
        }
        #endregion

        #region Manage Manual Entry
        public IActionResult ManualEntryAdd()
        {
            var data = new EISViewModel()
            {
                CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "70" })),
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_MANUAL_ENTRY_DATA" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult ManualEntryAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_MANUAL_ENTRY";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant != null ? model.Plant : model.hdnField1;
                model.Yr = model.Yr != 0 ? model.Yr : Convert.ToInt32(model.hdnField2);
                model.Mon = model.Mon != 0 ? model.Mon : Convert.ToInt32(model.hdnField3);
                model.Param1 = model.Category != null ? model.Category : model.hdnField4;
                model.Amount = model.Amount;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(ManualEntryAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(ManualEntryAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(ManualEntryAdd));
        }

        [HttpPost]
        public JsonResult FillManualEntryByDocNo(int yr, int mon, string plant, string category)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel { Activity = "FILL_MANUAL_ENTRY_BY_DOCNO", Yr = yr, Mon = mon, Plant = plant, Param1 = category });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Yr = Convert.ToInt32(dt.Rows[0]["YR"]);
                model.Mon = Convert.ToInt32(dt.Rows[0]["MON"]);
                model.Category = Convert.ToString(dt.Rows[0]["GL_TYPE"]);
                model.Amount = Convert.ToDecimal(dt.Rows[0]["AMOUNT"]);
            }
            return Json(model);
        }
        #endregion

        #region Manage DL Cost
        public IActionResult DLCostAdd()
        {
            var data = new EISViewModel()
            {
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_DL_COST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult DLCostAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_DL_COST";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Date1 = model.Date1;
                model.Amount = model.Amount;
                model.Amount2 = model.Amount2;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(DLCostAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(DLCostAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(DLCostAdd));
        }
        [HttpPost]
        public JsonResult FillDLCostByDocNo(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel { Activity = "FILL_DLCOST_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Date1 = Convert.ToString(dt.Rows[0]["DATE1"]);
                model.Amount = Convert.ToDecimal(dt.Rows[0]["AMOUNT"]);
                model.Amount2 = Convert.ToDecimal(dt.Rows[0]["AMOUNT2"]);
            }
            return Json(model);
        }
        public JsonResult DeleteDLCost(int docno)
        {
            _eisService.usp_EIS_ADD(new EISViewModel { Activity = "DELETE_DL_COST", DocNo = docno });
            return Json("OK");
        }
        #endregion

        #region Manage UPPH & UPH Plan
        public IActionResult UPHTargetAdd()
        {
            var data = new EISViewModel()
            {
                LineList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "5" })),
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_UPH_TARGET" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult UPHTargetAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_UPH_TARGET";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.Yr = model.Yr;
                model.Mon = model.Mon;
                model.Param1 = model.LineType;
                model.Bottom = model.Bottom;
                model.Basic = model.Basic;
                model.Challenge = model.Challenge;
                model.UserId = User.Identity.GetADId();

                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(UPHTargetAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(UPHTargetAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(UPHTargetAdd));
        }
        [HttpPost]
        public JsonResult FillUPHByDocNo(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_UPH_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Yr = Convert.ToInt32(dt.Rows[0]["YR"]);
                model.Mon = Convert.ToInt32(dt.Rows[0]["MON"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.LineType = Convert.ToString(dt.Rows[0]["LINETYPE"]);
                model.LineType = Convert.ToString(dt.Rows[0]["LINETYPE"]);
                model.Bottom = Convert.ToDecimal(dt.Rows[0]["BOTTOM"]);
                model.Basic = Convert.ToDecimal(dt.Rows[0]["BASIC"]);
                model.Challenge = Convert.ToDecimal(dt.Rows[0]["CHALLENGE"]);
            }
            return Json(model);
        }

        public IActionResult UPPHTargetAdd()
        {
            var data = new EISViewModel()
            {
                LineList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "5" })),
                CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "6" })),
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_UPPH_TARGET" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult UPPHTargetAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_UPPH_TARGET";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.Yr = model.Yr;
                model.Mon = model.Mon;
                model.Param1 = model.Customer;
                model.Param2 = model.LineType;
                model.Bottom = model.Bottom;
                model.Basic = model.Basic;
                model.Challenge = model.Challenge;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(UPPHTargetAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(UPPHTargetAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(UPPHTargetAdd));
        }
        [HttpPost]
        public JsonResult FillUPPHByDocNo(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_UPPH_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.Yr = Convert.ToInt32(dt.Rows[0]["YR"]);
                model.Mon = Convert.ToInt32(dt.Rows[0]["MON"]);
                model.Customer = Convert.ToString(dt.Rows[0]["CUSTOMER"]);
                model.LineType = Convert.ToString(dt.Rows[0]["LINETYPE"]);
                model.Bottom = Convert.ToDecimal(dt.Rows[0]["BOTTOM"]);
                model.Basic = Convert.ToDecimal(dt.Rows[0]["BASIC"]);
                model.Challenge = Convert.ToDecimal(dt.Rows[0]["CHALLENGE"]);
            }
            return Json(model);
        }
        #endregion

        #region Manage Targets
        public IActionResult EISTargetAdd()
        {
            var data = new EISViewModel()
            {
                CategoryList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_VALUE", DocNo = "41" })),
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_TARGETS" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult EISTargetAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_TARGETS";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.Yr = model.Yr;
                model.Mon = model.Mon;
                model.Param1 = model.Category;
                model.Bottom = model.Bottom;
                model.Basic = model.Basic;
                model.Challenge = model.Challenge;
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(EISTargetAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(EISTargetAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(EISTargetAdd));
        }
        [HttpPost]
        public JsonResult FillTarget(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel { Activity = "FILL_TARGET_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Category = Convert.ToString(dt.Rows[0]["SYS_NAME"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.Yr = Convert.ToInt32(dt.Rows[0]["YR"]);
                model.Mon = Convert.ToInt32(dt.Rows[0]["MON"]);
                model.Bottom = Convert.ToDecimal(dt.Rows[0]["BOTTOM"]);
                model.Basic = Convert.ToDecimal(dt.Rows[0]["BASIC"]);
                model.Challenge = Convert.ToDecimal(dt.Rows[0]["CHALLENGE"]);
            }
            return Json(model);
        }
        #endregion

        #region Manage Standard Time
        public IActionResult StandardTimeTargetAdd()
        {
            var data = new EISViewModel()
            {
                GridList = Common.ConvertDataTableToList<EISViewModel>(_eisService.usp_EIS_ADD(new EISViewModel() { Activity = "FILL_LIST_STANDARD_TIME" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult StandardTimeTargetAdd(EISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Activity = "ADD_STANDARD_TIME";
                model.DocNo = Convert.ToString(model.DocNo) != null ? Convert.ToInt32(model.DocNo) : 0;
                model.Plant = model.Plant;
                model.Division = model.Division;
                model.Param1 = model.Param1;
                model.Param2 = Convert.ToString(model.Value1);
                model.UserId = User.Identity.GetADId();
                DataTable dt = _eisService.usp_EIS_ADD(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                        return RedirectToAction(nameof(StandardTimeTargetAdd));
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                        return RedirectToAction(nameof(StandardTimeTargetAdd));
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
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
            return RedirectToAction(nameof(StandardTimeTargetAdd));
        }
        [HttpPost]
        public JsonResult FillStandardTime(int docno)
        {
            var model = new EISViewModel();
            DataTable dt = _eisService.usp_EIS_ADD(new EISViewModel { Activity = "FILL_STANDARD_TIME_BY_DOCNO", DocNo = docno });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.DocNo = Convert.ToInt32(dt.Rows[0]["DOCNO"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.Division = Convert.ToString(dt.Rows[0]["DIVISION"]);
                model.Param1 = Convert.ToString(dt.Rows[0]["MODEL_NAME"]);
                model.Value1 = Convert.ToDecimal(dt.Rows[0]["STD_TIME"]);
            }
            return Json(model);
        }
        #endregion
        #endregion

    }
}
