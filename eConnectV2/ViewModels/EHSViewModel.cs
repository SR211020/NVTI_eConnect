using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;


namespace eConnectV2.ViewModels
{
    public class EHSViewModel
    {
        public EHSViewModel()
        {
            CategoryList = new List<SelectListItem>();
            TypeList = new List<SelectListItem>();
            HistoryList = new List<EHSViewModel>();
        }

        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public List<EHSViewModel> HistoryList { get; set; }


        public int RequestNo { get; set; }
        public int SrNo { get; set; }
        public string Plant { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Type { get; set; }
        public string Injury { get; set; }
        public string InjuryType { get; set; }
        public string InjuryTypeOther { get; set; }
        public string NurshingStaff { get; set; }
        public string RestGiven { get; set; }
        public string Refer { get; set; }
        public string AadharNo { get; set; }
        public string Quantity { get; set; }
        public string Treatment { get; set; }
        public string Condition { get; set; }
        public string Time1 { get; set; }
        public string Time2 { get; set; }
        public string Remarks { get; set; }
        public int Total { get; set; }
        public string DataDate { get; set; }

        public string Activity { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string TDate { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
    }
}
