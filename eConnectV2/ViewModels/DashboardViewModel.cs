using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eConnectV2.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            AreaList = new List<SelectListItem>();
            CustomerList = new List<SelectListItem>();
            WODetails = new List<DashboardViewModel>();
        }

        #region Drop Down List
        public List<SelectListItem> AreaList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        #endregion

        #region Grid List
        public List<DashboardViewModel> WODetails { get; set; }
        #endregion

        public string Division { get; set; }
        public string WorkOrder { get; set; }
        public string ItemId { get; set; }
        public string ModelName { get; set; }
        public string Customer { get; set; }
        public string StatusDescr { get; set; }
        public decimal Total { get; set; }
        public int Finished { get; set; }
        public int Pending { get; set; }
        public decimal Percentage { get; set; }

        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
    }
}
