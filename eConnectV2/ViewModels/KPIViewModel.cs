using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class KPIViewModel
    {
        public KPIViewModel()
        {
            DepartmentList = new List<SelectListItem>();
            EmployeeList = new List<SelectListItem>();
            CategoryList = new List<SelectListItem>();
            HodDepartmentList = new List<SelectListItem>();

            KpiList = new List<KPIViewModel>();
            FinYearList = new List<SelectListItem>();
            //KpiStatusList = new List<SelectListItem>();
        }

        public string Flag { get; set; }
        public string Msg { get; set; }

        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> HodDepartmentList { get; set; }

        public List<KPIViewModel> KpiList { get; set; }
        public List<SelectListItem> FinYearList { get; set; }
        //public List<SelectListItem> KpiStatusList { get; set; }

        public string Activity { get; set; }
        //public string Module { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DefaultFinYearCode { get; set; }
        public string FinYearCode { get; set; }
        public string FinYearName { get; set; }
        public string HodCode { get; set; }
        public string Status { get; set; }


        //public string TDATE { get; set; }
        public string AdId { get; set; }
        //public bool IsKpiUserLoggedIn { get; set; }
        //public bool IsKpiDetailSave { get; set; }
        public string KpiType { get; set; }
        public string Param1 { get; set; }
        public string ConfigCategory1 { get; set; }
        //public string ConfigCategory2 { get; set; }
        public string ConfigValue { get; set; }
        //public string ConfigLevel { get; set; }
        public string KpiNo { get; set; }

        public string HodName { get; set; }
        public string Score { get; set; }
        public string SrNo { get; set; }

        public string KpiName { get; set; }
        public string RespDeptCode { get; set; }
        public string RespDeptName { get; set; }
        public string BottomTarget { get; set; }
        public string BasicTarget { get; set; }
        public string ChallengeTarget { get; set; }
        public string Weightage { get; set; }
        ////public string TotalWeightage { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        //public string Remarks { get; set; }
        public string KpiStatusCode { get; set; }
        //public string KpiStatusName { get; set; }

        public string IsKPIApprove { get; set; }
        //public string ApproverType { get; internal set; }
        public string ApproverCode { get; set; }
        //public string ApproverName { get; set; }
        ////public bool IsFinApprover { get; set; }
        ////public string FinApproverCode { get; set; }
        ////public string FinApproverName { get; set; }
        //public bool IsAll { get; set; }
        public byte[] AttachDocument { get; set; }
        public string DocUId { get; set; }

        public string APR_BT { get; set; }
        public string MAY_BT { get; set; }
        public string JUN_BT { get; set; }
        public string JUL_BT { get; set; }
        public string AUG_BT { get; set; }
        public string SEP_BT { get; set; }
        public string OCT_BT { get; set; }
        public string NOV_BT { get; set; }
        public string DEC_BT { get; set; }
        public string JAN_BT { get; set; }
        public string FEB_BT { get; set; }
        public string MAR_BT { get; set; }

        public string APR_BS { get; set; }
        public string MAY_BS { get; set; }
        public string JUN_BS { get; set; }
        public string JUL_BS { get; set; }
        public string AUG_BS { get; set; }
        public string SEP_BS { get; set; }
        public string OCT_BS { get; set; }
        public string NOV_BS { get; set; }
        public string DEC_BS { get; set; }
        public string JAN_BS { get; set; }
        public string FEB_BS { get; set; }
        public string MAR_BS { get; set; }

        public string APR_CH { get; set; }
        public string MAY_CH { get; set; }
        public string JUN_CH { get; set; }
        public string JUL_CH { get; set; }
        public string AUG_CH { get; set; }
        public string SEP_CH { get; set; }
        public string OCT_CH { get; set; }
        public string NOV_CH { get; set; }
        public string DEC_CH { get; set; }
        public string JAN_CH { get; set; }
        public string FEB_CH { get; set; }
        public string MAR_CH { get; set; }


        public string APR_IN { get; set; }
        public string MAY_IN { get; set; }
        public string JUN_IN { get; set; }
        public string JUL_IN { get; set; }
        public string AUG_IN { get; set; }
        public string SEP_IN { get; set; }
        public string OCT_IN { get; set; }
        public string NOV_IN { get; set; }
        public string DEC_IN { get; set; }
        public string JAN_IN { get; set; }
        public string FEB_IN { get; set; }
        public string MAR_IN { get; set; }

        public string APR_CALC { get; set; }
        public string MAY_CALC { get; set; }
        public string JUN_CALC { get; set; }
        public string JUL_CALC { get; set; }
        public string AUG_CALC { get; set; }
        public string SEP_CALC { get; set; }
        public string OCT_CALC { get; set; }
        public string NOV_CALC { get; set; }
        public string DEC_CALC { get; set; }
        public string JAN_CALC { get; set; }
        public string FEB_CALC { get; set; }
        public string MAR_CALC { get; set; }

        //public string HodDept { get; set; }

        public string M1 { get; set; }
        public string M2 { get; set; }
        public string M3 { get; set; }
        public string M4 { get; set; }
        public string M5 { get; set; }
        public string M6 { get; set; }
        public string M7 { get; set; }
        public string M8 { get; set; }
        public string M9 { get; set; }
        public string M10 { get; set; }
        public string M11 { get; set; }
        public string M12 { get; set; }
        public string Month { get; set; }
        public double Calc { get; set; }
        public string InputVal { get; set; }
    }
}
