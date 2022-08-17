using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Data;
using eConnectV2.Extensions;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using eConnectV2.Helpers;


namespace eConnectV2.Controllers
{
    public class BISController : Controller
    {
        private readonly IBISService _bisService;

        public BISController(IBISService bisService)
        {
            _bisService = bisService;
        }

        #region ManageUPPHPlan View

        public IActionResult ManageUPPHPlan()
        {
            var model = new BISViewModel();
            model.Plant = HttpContext.Request.Query["plant"].ToString().ToUpper() == "MN" ? "MANESAR" : "BAWAL";
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));

            model.Activity = "FILL_GRID_UPPH";
            model.dt = _bisService.usp_UPPH(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ManageUPPHPlan(BISViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Plant = model.Plant;
                model.Activity = "ADD_PLAN";
                model.Division = Convert.ToString(model.Division) != null ? Convert.ToString(model.Division) : Convert.ToString(model.hdnField1);
                model.LineNo = Convert.ToString(model.LineNo) != null ? Convert.ToString(model.LineNo) : Convert.ToString(model.hdnField2);
                model.Shift = Convert.ToString(model.Shift) != null ? Convert.ToString(model.Shift) : Convert.ToString(model.hdnField3);
                model.Param1 = Convert.ToString(model.Variance1) != "" ? Convert.ToString(model.Variance1) : "0";
                model.Param2 = Convert.ToString(model.Variance2) != "" ? Convert.ToString(model.Variance2) : "0";
                model.Param3 = Convert.ToString(model.Variance3) != "" ? Convert.ToString(model.Variance3) : "0";
                model.Param4 = Convert.ToString(model.Variance4) != "" ? Convert.ToString(model.Variance4) : "0";
                model.Param5 = Convert.ToString(model.Variance5) != "" ? Convert.ToString(model.Variance5) : "0";
                model.Param6 = Convert.ToString(model.Variance6) != "" ? Convert.ToString(model.Variance6) : "0";
                model.Param7 = Convert.ToString(model.Variance7) != "" ? Convert.ToString(model.Variance7) : "0";
                model.Param8 = Convert.ToString(model.Variance8) != "" ? Convert.ToString(model.Variance8) : "0";
                model.UserId = User.Identity.GetADId();
                DataTable dt = _bisService.usp_UPPH(model);
                if(dt != null && dt.Rows.Count > 0)
                {
                    Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if(iFlag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (iFlag == 2)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultUpdateMessage;
                    }
                    else if (iFlag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;

                        model.Activity = "FILL_SHIFT";
                        model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));

                        model.Activity = "FILL_GRID_UPPH";
                        model.dt = _bisService.usp_UPPH(model);
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
            return RedirectToAction(nameof(ManageUPPHPlan));
        }

        #endregion


        public IActionResult UPPHAuto()
        {
            var model = new BISViewModel();
            model.UpphDate = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.Plant = HttpContext.Request.Query["plant"].ToString().ToUpper() == "MN" ? "MANESAR" : "BAWAL";
            model.CustomerName = HttpContext.Request.Query["cust"].ToString().ToUpper();
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return View(model);
        }

        public IActionResult UPPHAuto2()
        {
            var model = new BISViewModel();
            model.Plant = HttpContext.Request.Query["plant"].ToString().ToUpper() == "MN" ? "MANESAR" : "BAWAL";
            model.CustomerName = HttpContext.Request.Query["cust"].ToString().ToUpper();
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return View(model);
        }

        public IActionResult UPPHManual()
        {
            var model = new BISViewModel();
            model.Plant = HttpContext.Request.Query["plant"].ToString().ToUpper() == "MN" ? "MANESAR" : "BAWAL";
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return View(model);
        }

        public IActionResult UPPHAll()
        {
            var model = new BISViewModel();
            model.Plant = HttpContext.Request.Query["plant"].ToString().ToUpper() == "MN" ? "MANESAR" : "BAWAL";
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return View(model);
        }

        public IActionResult SMTFPYAuto()
        {
            var model = new BISViewModel();
            return View(model);
        }

        public IActionResult SMTFPYManual()
        {
            var model = new BISViewModel();
            model.Activity = "FILL_SHIFT";
            model.ShiftList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return View(model);
        }

        #region Get Auto & Manual UPPH For All Battery Lines Bawal
        [HttpPost]
        public JsonResult GetShiftDetailsByTime(string plant, string currTime)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "GET_SHIFT_DETAILS";
            model.Param1 = currTime;
            DataTable dt = _bisService.usp_UPPH(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.Shift = dataRow["SHIFT"].ToString().Trim();
                model.FromTime = Convert.ToString(dataRow["START_TIME"]).Trim();
                model.ToTime = Convert.ToString(dataRow["END_TIME"]).Trim();
                model.Variance1 = Convert.ToInt32(dataRow["VARIANCE"]);
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult GetShiftByTime(string plant, string currTime)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "BIND_SHIFT_BY_TIME";
            model.Param1 = currTime;
            DataTable dt = _bisService.usp_UPPH(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.Shift = dataRow["SHIFT"].ToString().Trim();
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult GetShiftDetails(string plant, string shift)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "GET_SHIFT_TIME";
            model.Shift = shift;
            DataTable dt = _bisService.usp_UPPH(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.FromTime = Convert.ToString(dataRow["START_TIME"]).Trim();
                model.ToTime = Convert.ToString(dataRow["END_TIME"]).Trim();
                model.Variance1 = Convert.ToInt32(dataRow["VARIANCE"]);
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult FillLineByDivision(string plant, string division)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_LINE_BY_DIVISION";
            model.Division = division;
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }
        [HttpPost]
        public JsonResult FillLineByCustomer(string plant, string division, string customer)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_LINE_BY_CUSTOMER";
            model.Division = division;
            model.CustomerName = customer.ToUpper();
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }
        [HttpPost]
        public JsonResult FillLineByCustomerAndShift(string plant, string division, string customer, string shift)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_LINE_BY_CUST_SHIFT";
            model.Division = division;
            model.Shift = shift;
            model.CustomerName = customer.ToUpper();
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }

        [HttpPost]
        public JsonResult FillUPPHDataBySrNo(string plant, int docno)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_UPPH_BY_SRNO";
            model.DocNo = docno;
            DataTable dt = _bisService.usp_UPPH(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.Division = Convert.ToString(dataRow["DIVISION"]);
                model.CustomerName = Convert.ToString(dataRow["CUSTOMER"]);
                model.LineNo = Convert.ToString(dataRow["LINE_NO"]);
                model.LineType = Convert.ToString(dataRow["LINE_TYPE"]);
                model.ModelName = Convert.ToString(dataRow["MODEL_NAME"]);
                model.Shift = Convert.ToString(dataRow["SHIFT"]);
                model.Plan = Convert.ToInt32(dataRow["PLAN"]);
                model.UphTarget = Convert.ToInt32(dataRow["UPH"]);
                model.StdHC = Convert.ToInt32(dataRow["STD_HC"]);
                model.ActualHC = Convert.ToInt32(dataRow["ACTUAL_HC"]);
                model.UpphTarget = Convert.ToDecimal(dataRow["UPPH"]);
                model.Variance1 = Convert.ToInt32(dataRow["VARIANCE1"]);
                model.Variance2 = Convert.ToInt32(dataRow["VARIANCE2"]);
                model.Variance3 = Convert.ToInt32(dataRow["VARIANCE3"]);
                model.Variance4 = Convert.ToInt32(dataRow["VARIANCE4"]);
                model.Variance5 = Convert.ToInt32(dataRow["VARIANCE5"]);
                model.Variance6 = Convert.ToInt32(dataRow["VARIANCE6"]);
                model.Variance7 = Convert.ToInt32(dataRow["VARIANCE7"]);
                model.Variance8 = Convert.ToInt32(dataRow["VARIANCE8"]);
            }
            return Json(model);
        }

        [HttpPost]
        public JsonResult UpdateStatus(string plant, int docno, string status)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "UPDATE_STATUS";
            model.DocNo = docno;
            model.Param1 = status;
            DataTable dt = _bisService.usp_UPPH(model);
            if (dt != null && dt.Rows.Count > 0)
            {
                Int32 iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                if (iFlag == 1)
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = Common.ResultUpdateMessage;
                    return Json(new { status = "1", msg = "" }); ;
                }
            }
            return Json(new { status = "0", msg = Common.SomethingWentWrong }); ;
        }

        [HttpPost]
        public JsonResult FillLineForUPPH(string plant, string division)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_LINE_UPPH";
            model.Division = division;
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetPlan(string plant, string lineNo, string shift)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "GET_PLAN";
            model.LineNo = lineNo;
            model.Shift = shift;
            DataTable dt = _bisService.usp_UPPH(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.StdHC = Convert.ToInt32(dataRow["STD_HC"]);
                model.ActualHC = Convert.ToInt32(dataRow["ACTUAL_HC"]);
                model.Plan = Convert.ToInt32(dataRow["PLAN"]);
                model.UphTarget = Convert.ToInt32(dataRow["UPH"]);
                model.UpphTarget = Convert.ToDecimal(dataRow["UPPH"]);
                model.FromTime = Convert.ToString(dataRow["START_TIME"]).Trim();
                model.ToTime = Convert.ToString(dataRow["END_TIME"]).Trim();
                model.Variance1 = Convert.ToInt32(dataRow["VARIANCE1"]);
                model.Variance2 = Convert.ToInt32(dataRow["VARIANCE2"]);
                model.Variance3 = Convert.ToInt32(dataRow["VARIANCE3"]);
                model.Variance4 = Convert.ToInt32(dataRow["VARIANCE4"]);
                model.Variance5 = Convert.ToInt32(dataRow["VARIANCE5"]);
                model.Variance6 = Convert.ToInt32(dataRow["VARIANCE6"]);
                model.Variance7 = Convert.ToInt32(dataRow["VARIANCE7"]);
                model.Variance8 = Convert.ToInt32(dataRow["VARIANCE8"]);
            }
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetBatteryUPPHHourly(string plant, string lineNo, string upphDate, string shift, string fromTime, string toTime)
        {
            DateTime dtToday = DateTime.Now;
            DateTime dtSelected = Convert.ToDateTime(upphDate);

            DateTime dtStart = Convert.ToDateTime(upphDate + " " + fromTime);
            DateTime dtEnd = Convert.ToDateTime(upphDate + " " + toTime);

            if (shift != "C")
            {
                while (dtStart < dtEnd)
                {
                    DateTime dtFrom = dtStart;
                    DateTime dtTo;
                    if (dtStart.AddHours(1) < dtEnd)
                    {
                        dtTo = dtStart.AddHours(1);
                    }
                    else
                    {
                        dtTo = dtEnd;
                    }

                    if (dtToday.Date == dtSelected.Date)
                    {
                        TimeSpan difference = dtToday.Subtract(dtStart);
                        if (difference.TotalMinutes > 0)
                        {
                            dtStart = dtStart.AddHours(1);
                            GetBatteryUPPHData(plant, lineNo, dtFrom, dtTo);
                        }
                        else
                        {
                            break;
                        }
                        //if (dtStart.Hour <= dtToday.Hour)
                        //{
                        //    dtStart = dtStart.AddHours(1);
                        //    GetBatteryUPPHData(plant, lineNo, dtFrom, dtTo);
                        //}
                        //else
                        //{
                        //    break;
                        //}
                    }
                    else
                    {
                        dtStart = dtStart.AddHours(1);
                        GetBatteryUPPHData(plant, lineNo, dtFrom, dtTo);
                    }
                }
            }
            else
            {
                if (dtSelected.Date != dtToday.Date)
                {
                    dtEnd = dtEnd.AddDays(1);
                    while (dtStart < dtEnd)
                    {
                        DateTime dtFrom = dtStart;
                        DateTime dtTo;
                        if (dtStart.AddHours(1) < dtEnd)
                        {
                            dtTo = dtStart.AddHours(1);
                        }
                        else
                        {
                            dtTo = dtEnd;
                        }

                        GetBatteryUPPHData(plant, lineNo, dtFrom, dtTo);
                        dtStart = dtStart.AddHours(1);
                    }
                }
                else
                {
                    dtStart = Convert.ToDateTime(upphDate + " " + fromTime);
                    dtEnd = Convert.ToDateTime(upphDate + " " + toTime);

                    if (dtToday.Hour > 12)
                    {
                        dtEnd = dtEnd.AddDays(1);
                    }
                    else
                    {
                        dtStart = dtStart.AddDays(-1);
                    }
                    while (dtStart < dtEnd)
                    {
                        DateTime dtFrom = dtStart;
                        DateTime dtTo;
                        if (dtStart.AddHours(1) < dtEnd)
                        {
                            dtTo = dtStart.AddHours(1);
                        }
                        else
                        {
                            dtTo = dtEnd;
                        }

                        GetBatteryUPPHData(plant, lineNo, dtFrom, dtTo);
                        if (dtStart.Date == dtToday.Date)
                        {
                            if (dtStart.Hour < dtToday.Hour)
                            {
                                dtStart = dtStart.AddHours(1);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            dtStart = dtStart.AddHours(1);
                        }
                    }
                }
            }
            var data = Common.ConvertDataTableToList<BISViewModel>((DataTable)TempData["myDt"]);
            return Json(data);
        }

        public void GetBatteryUPPHData(string plant, string lineNo, DateTime startDate, DateTime endDate)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "GET_BATTERY_UPPH_HOURLY";
            model.LineNo = lineNo;
            model.Date1 = startDate.ToString("dd-MMM-yyyy HH:mm");
            model.Date2 = endDate.ToString("dd-MMM-yyyy HH:mm");

            DataTable dtUpph = _bisService.usp_UPPH(model);
            if (dtUpph != null && dtUpph.Rows.Count > 0)
            {
                DataRow dr = dtUpph.Rows[0];
                Int32 iCell = Convert.ToString(dr["CellCount"]) != "" ? Convert.ToInt32(dr["CellCount"]) : 0;
                Int32 iLinking = Convert.ToString(dr["MappingCount"]) != "" ? Convert.ToInt32(dr["MappingCount"]) : 0;
                Int32 iPackSize = Convert.ToString(dr["PackSizeCount"]) != "" ? Convert.ToInt32(dr["PackSizeCount"]) : 0;
                Int32 iFT = Convert.ToString(dr["FTCount"]) != "" ? Convert.ToInt32(dr["FTCount"]) : 0;
                Int32 iPacking = Convert.ToString(dr["PackingCount"]) != "" ? Convert.ToInt32(dr["PackingCount"]) : 0;

                AddBatteryUPPHData(startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"), iCell, iLinking, iPackSize, iFT, iPacking);
            }
        }

        public void AddBatteryUPPHData(string strFromTime, string strToTime, int iCell, int iLinking, int iPackSize, int iFT, int iPacking)
        {
            DataTable myDt = new DataTable();
            if (TempData["myDt"] == null)
            {
                myDt = CreateDataTable();
            }
            else
            {
                myDt = (DataTable)TempData["myDt"];
            }
            try
            {
                DataRow dr = myDt.NewRow();
                dr["FromTime"] = strFromTime;
                dr["ToTime"] = strToTime;
                dr["CellCount"] = iCell;
                dr["MappingCount"] = iLinking;
                dr["PackSizeCount"] = iPackSize;
                dr["FtCount"] = iFT;
                dr["PackingCount"] = iPacking;
                myDt.Rows.Add(dr);
                TempData["myDt"] = myDt;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FromTime", typeof(string));
            dt.Columns.Add("ToTime", typeof(string));
            dt.Columns.Add("CellCount", typeof(decimal));
            dt.Columns.Add("MappingCount", typeof(decimal));
            dt.Columns.Add("PackSizeCount", typeof(decimal));
            dt.Columns.Add("FtCount", typeof(decimal));
            dt.Columns.Add("PackingCount", typeof(decimal));
            return dt;
        }

        #endregion


        #region Get UPPH For All Battery Lines Bawal - View Name (UPPHAll)

        [HttpPost]
        public JsonResult GetBatteryUPPHAllLines(string plant, string lineNo, string shift, DateTime startDate, DateTime endDate)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "GET_BATTERY_UPPH_ALL";
            model.LineNo = lineNo;
            model.Shift = shift;
            model.Date1 = startDate.ToString("dd-MMM-yyyy HH:mm");
            model.Date2 = endDate.ToString("dd-MMM-yyyy HH:mm");
            return Json(Common.ConvertDataTableToList<BISViewModel>(_bisService.usp_UPPH(model)));
        }
        #endregion


        #region Get Auto FPY Calculation for SMT
        [HttpPost]
        public JsonResult FillSMTLinesForFPY(string plant)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_SMT_FPY_LINES";
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }

        public JsonResult FillSMTAllLinesForFPY(string plant)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = "FILL_SMT_FPY_LINES_ALL";
            model.LineNoList = Common.BindDropDown(_bisService.usp_UPPH(model));
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetSMTFPYHourly(string plant, string mode, string lineNo, string fpyDate, string shift, string fromTime, string toTime)
        {
            DateTime dtToday = DateTime.Now;
            DateTime dtSelected = Convert.ToDateTime(fpyDate);

            DateTime dtStart = Convert.ToDateTime(fpyDate + " " + fromTime);
            DateTime dtEnd = Convert.ToDateTime(fpyDate + " " + toTime);

            if (shift == "0")
            {
                dtStart = Convert.ToDateTime(fpyDate + " 00:00");
                if (dtSelected.Date != dtToday.Date)
                {
                    dtEnd = dtStart.AddDays(1);
                }
                else
                {
                    dtEnd = DateTime.Now;
                }
                while (dtStart < dtEnd)
                {
                    DateTime dtFrom = dtStart;
                    DateTime dtTo;
                    if (dtStart.AddHours(1) < dtEnd)
                    {
                        dtTo = dtStart.AddHours(1);
                    }
                    else
                    {
                        dtTo = dtEnd;
                    }
                    dtStart = dtStart.AddHours(1);
                    GetSMTFPYData(plant, mode, lineNo, dtFrom, dtTo);
                }
            }
            else
            {
                if (shift != "C")
                {
                    while (dtStart < dtEnd)
                    {
                        DateTime dtFrom = dtStart;
                        DateTime dtTo;
                        if (dtStart.AddHours(1) < dtEnd)
                        {
                            dtTo = dtStart.AddHours(1);
                        }
                        else
                        {
                            dtTo = dtEnd;
                        }

                        if (dtToday.Date == dtSelected.Date)
                        {
                            TimeSpan difference = dtToday.Subtract(dtStart);
                            if (difference.TotalMinutes > 0)
                            {
                                dtStart = dtStart.AddHours(1);
                                GetSMTFPYData(plant, mode, lineNo, dtFrom, dtTo);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            dtStart = dtStart.AddHours(1);
                            GetSMTFPYData(plant, mode, lineNo, dtFrom, dtTo);
                        }
                    }
                }
                else
                {
                    if (dtSelected.Date != dtToday.Date)
                    {
                        dtEnd = dtEnd.AddDays(1);
                        while (dtStart < dtEnd)
                        {
                            DateTime dtFrom = dtStart;
                            DateTime dtTo;
                            if (dtStart.AddHours(1) < dtEnd)
                            {
                                dtTo = dtStart.AddHours(1);
                            }
                            else
                            {
                                dtTo = dtEnd;
                            }

                            GetSMTFPYData(plant, mode, lineNo, dtFrom, dtTo);
                            dtStart = dtStart.AddHours(1);
                        }
                    }
                    else
                    {
                        dtStart = Convert.ToDateTime(fpyDate + " " + fromTime);
                        dtEnd = Convert.ToDateTime(fpyDate + " " + toTime);

                        if (dtToday.Hour > 12)
                        {
                            dtEnd = dtEnd.AddDays(1);
                        }
                        else
                        {
                            dtStart = dtStart.AddDays(-1);
                        }
                        while (dtStart < dtEnd)
                        {
                            DateTime dtFrom = dtStart;
                            DateTime dtTo;
                            if (dtStart.AddHours(1) < dtEnd)
                            {
                                dtTo = dtStart.AddHours(1);
                            }
                            else
                            {
                                dtTo = dtEnd;
                            }

                            GetSMTFPYData(plant, mode, lineNo, dtFrom, dtTo);
                            if (dtStart.Date == dtToday.Date)
                            {
                                if (dtStart.Hour < dtToday.Hour)
                                {
                                    dtStart = dtStart.AddHours(1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                dtStart = dtStart.AddHours(1);
                            }
                        }
                    }
                }
            }
            
            var data = Common.ConvertDataTableToList<BISViewModel>((DataTable)TempData["myDt"]);
            return Json(data);
        }

        public void GetSMTFPYData(string plant, string mode, string lineNo, DateTime startDate, DateTime endDate)
        {
            var model = new BISViewModel();
            model.Plant = plant;
            model.Activity = mode == "AUTO" ? "GET_SMT_FPY" : "GET_SMT_FPY_ALL";
            model.LineNo = lineNo;
            model.Date1 = startDate.ToString("dd-MMM-yyyy HH:mm");
            model.Date2 = endDate.ToString("dd-MMM-yyyy HH:mm");

            DataTable dtUpph = _bisService.usp_UPPH(model);
            if (dtUpph != null && dtUpph.Rows.Count > 0)
            {
                Int32 iTotal = 0;
                Int32 iTotalPass = 0;
                Int32 iNG = 0;
                Decimal dFPY = 0;
                Int32 iFirstPass = 0;
                Int32 iSecondPass = 0;
                if (mode == "AUTO")
                {
                    DataRow dr = dtUpph.Rows[0];
                    iTotal = Convert.ToString(dr["TOTAL"]) != "" ? Convert.ToInt32(dr["TOTAL"]) : 0;
                    iTotalPass = Convert.ToString(dr["TOTAL_PASS"]) != "" ? Convert.ToInt32(dr["TOTAL_PASS"]) : 0;
                    iNG = Convert.ToString(dr["NG"]) != "" ? Convert.ToInt32(dr["NG"]) : 0;
                    dFPY = Convert.ToString(dr["FPY"]) != "" ? Convert.ToDecimal(dr["FPY"]) : 0;
                    
                }
                else
                {
                    DataRow dr = dtUpph.Rows[0];
                    iTotal = Convert.ToString(dr["TOTAL"]) != "" ? Convert.ToInt32(dr["TOTAL"]) : 0;
                    iFirstPass = Convert.ToString(dr["FIRST_PASS"]) != "" ? Convert.ToInt32(dr["FIRST_PASS"]) : 0;
                    iSecondPass = Convert.ToString(dr["SECOND_PASS"]) != "" ? Convert.ToInt32(dr["SECOND_PASS"]) : 0;
                }
                AddSMTFPYData(startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"), iTotal, iTotalPass, iNG, dFPY, iFirstPass, iSecondPass);
            }
        }

        public void AddSMTFPYData(string strFromTime, string strToTime, int iTotal, int iTotalPass, int iNG, Decimal dFPY, int iFirstPass, int iSecondPass)
        {
            DataTable myDt = new DataTable();
            if (TempData["myDt"] == null)
            {
                myDt = CreateDT_SMTFPY();
            }
            else
            {
                myDt = (DataTable)TempData["myDt"];
            }
            try
            {
                DataRow dr = myDt.NewRow();
                dr["FromTime"] = strFromTime;
                dr["ToTime"] = strToTime;
                dr["TOTAL"] = iTotal;
                dr["TOTALPASS"] = iTotalPass;
                dr["NG"] = iNG;
                dr["FPY"] = dFPY;
                dr["FIRSTPASS"] = iFirstPass;
                dr["SECONDPASS"] = iSecondPass;
                myDt.Rows.Add(dr);
                TempData["myDt"] = myDt;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public DataTable CreateDT_SMTFPY()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FromTime", typeof(string));
            dt.Columns.Add("ToTime", typeof(string));
            dt.Columns.Add("TOTAL", typeof(int));
            dt.Columns.Add("TOTALPASS", typeof(int));
            dt.Columns.Add("NG", typeof(int));
            dt.Columns.Add("FPY", typeof(decimal));
            dt.Columns.Add("FIRSTPASS", typeof(int));
            dt.Columns.Add("SECONDPASS", typeof(int));
            return dt;
        }
        #endregion

    }
}
