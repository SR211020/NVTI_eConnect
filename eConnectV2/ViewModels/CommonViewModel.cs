using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class CommonViewModel
    {
        public CommonViewModel()
        {
            DropDownList = new List<SelectListItem>();
        }

        public List<SelectListItem> DropDownList { get; set; }

        #region Stored Procedure Parameters 
        public string Activity { get; set; }
        public string DocNo { get; set; }
        public string Plant { get; set; }
        public string Dept { get; set; }
        public string UserId { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        #endregion
    }
}
