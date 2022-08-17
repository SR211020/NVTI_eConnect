using eConnectV2.Extensions;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using eConnectV2.Helpers;
using System.Globalization;


namespace eConnectV2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IMESService _mesService;
        private readonly ICommonService _commonService;

        public DashboardController(IMESService mesService, ICommonService commonService)
        {
            _mesService = mesService;
            _commonService = commonService;
        }
        public IActionResult WODashboard()
        {
            DashboardViewModel model = new();
            DateTime dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, 1);
            model.Date1 = dt.ToString("dd'-'MMM'-'yyyy");
            model.Date2 = DateTime.Now.ToString("dd'-'MMM'-'yyyy");
            model.CustomerList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "69" }));

            #region WO Status Chart
            ViewBag.WOStatusData = Common.ConvertDataTableToList<DashboardViewModel>(_mesService.usp_Reports(new MESViewModel() { Activity = "GET_WO_COUNT_BY_STATUS", Param1 = "SMT", Param2 = "0", Date1 = DateTime.Parse(model.Date1).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture), Date2 = DateTime.Parse(model.Date2).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture)}));
            #endregion
            return View(model);
        }

        public JsonResult GetWOStatus(string division, string customer, string date1, string date2)
        {
            var data = Common.ConvertDataTableToList<DashboardViewModel>(_mesService.usp_Reports(new MESViewModel() { Activity = "GET_WO_COUNT_BY_STATUS", Param1 = division, Param2 = customer, Date1 = DateTime.Parse(date1).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture), Date2 = DateTime.Parse(date2).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture)}));
            return Json(data);
        }

        public JsonResult FillWOCountStatusWise(string division, string customer, string date1, string date2)
        {
            var data = new List<DashboardViewModel>();
            DataTable dt = _mesService.usp_Reports(new MESViewModel
            {
                Activity = "GET_WO_COUNT",
                Param1 = division,
                Param2 = customer,
                Date1 = DateTime.Parse(date1).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture),
                Date2 = DateTime.Parse(date2).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture)
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new DashboardViewModel
                {
                    ModelName = Convert.ToString(datarow["MODEL_NAME"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    Total = Convert.ToInt32(datarow["TOTAL"])
                });
            }
            return Json(data);
        }

        public JsonResult FillWODetails(string division, string customer, string date1, string date2)
        {
            var data = new List<DashboardViewModel>();
            DataTable dt = _mesService.usp_Reports(new MESViewModel
            {
                Activity = "GET_WO_DETAILS",
                Param1 = division,
                Param2 = customer,
                Date1 = DateTime.Parse(date1).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture),
                Date2 = DateTime.Parse(date2).ToString("yyyy'/'MM'/'dd", CultureInfo.InvariantCulture)
            });
            foreach (DataRow datarow in dt.Rows)
            {
                data.Add(new DashboardViewModel
                {
                    WorkOrder = Convert.ToString(datarow["WO"]),
                    ItemId = Convert.ToString(datarow["WORKITEMID"]),
                    ModelName = Convert.ToString(datarow["MODEL_NAME"]),
                    StatusDescr = Convert.ToString(datarow["STATUSDESCR"]),
                    Date1 = Convert.ToString(datarow["CREATEDATE"]),
                    Total = Convert.ToInt32(datarow["SCHEDULED_QTY"]),
                    Finished = Convert.ToInt32(datarow["FINISHED_QTY"]),
                    Pending = Convert.ToInt32(datarow["PENDING_QTY"]),
                    Percentage = Convert.ToDecimal(datarow["PERCENTAGE"])
                });
            }
            return Json(data);
        }
    }
}
