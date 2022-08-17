using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eConnectV2.ViewModels
{
    public class KaizenViewModel
    {
        public KaizenViewModel()
        {
            DDLDeptList = new List<SelectListItem>();
            DDLApproverList = new List<SelectListItem>();
            DDLMonthList = new List<SelectListItem>();
            DDLFinYearList = new List<SelectListItem>();
            DDLImpactList = new List<SelectListItem>();
            DDLKaizenTypeList = new List<SelectListItem>();
            DDLPillarList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            HistoryList = new List<KaizenViewModel>();
        }
        public List<SelectListItem> DDLDeptList { get; set; }
        public List<SelectListItem> DDLApproverList { get; set; }
        public List<SelectListItem> DDLMonthList { get; set; }
        public List<SelectListItem> DDLFinYearList { get; set; }
        public List<SelectListItem> DDLImpactList { get; set; }
        public List<SelectListItem> DDLKaizenTypeList { get; set; }
        public List<SelectListItem> DDLPillarList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<KaizenViewModel> HistoryList { get; set; }        

        #region Param List
        public string Activity { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Date3 { get; set; }
        public string Date4 { get; set; }
        public int DocNo { get; set; }
        public int SrNo { get; set; }
        public string RegNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Plant { get; set; }
        public string Month { get; set; }
        public string FinYearCode { get; set; }
        public string MacArea { get; set; }
        public string KaizenName { get; set; }
        public string RespPillar { get; set; }
        public string HDDone { get; set; }
        public string CircleNo { get; set; }
        public string DocName { get; set; }
        public string Impact { get; set; }
        public string SelectedImpact { get; set; }
        public string Year { get; set; }
        public string KaizenType { get; set; }
        public string CostSaving { get; set; }
        public string MPWorthy { get; set; }
        public int Status { get; set; }
        public string StatusDescr { get; set; }
        public string TDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string ApprId { get; set; }
        public string ApprName { get; set; }
        public string Remarks { get; set; }
        public int Total { get; set; }
        #endregion

    }
}
