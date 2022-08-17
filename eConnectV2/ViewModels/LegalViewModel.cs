using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class LegalViewModel
    {
        public LegalViewModel()
        {
            DDLDeptList = new List<SelectListItem>();
            DDLCategoryList = new List<SelectListItem>();
            ContractList = new List<LegalViewModel>();
            ContractHistory = new List<LegalViewModel>();
        }
        public List<SelectListItem> DDLDeptList { get; set; }
        public List<SelectListItem> DDLCategoryList { get; set; }
        public List<LegalViewModel> ContractList { get; set; }
        public List<LegalViewModel> ContractHistory { get; set; }

        #region Repository
        public int DocNo { get; set; }
        public string Vendor { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Plant { get; set; }
        public string ContractCategory { get; set; }
        public string ContractName { get; set; }
        public string ContractNature { get; set; }
        public string ContractValue { get; set; }
        public string Currency { get; set; }
        public string MasterContract { get; set; }
        public string ContractNumber { get; set; }
        public string Email { get; set; }
        public string ExeDate { get; set; }
        public string EffDate { get; set; }
        public string ExpDate { get; set; }
        //public string DocName { get; set; }
        public string DocDraft { get; set; }
        public string DocSigned { get; set; }
        public string Remark { get; set; }
        public string ServiceDescription { get; set; }
        public int DaysLeft { get; set; }
        public int Flag { get; set; }
        public string UserId { get; set; }

        #endregion

        #region param list
        public string Activity { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string Param7 { get; set; }
        public string Param8 { get; set; }
        public string Param9 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Date3 { get; set; }
        public string Date4 { get; set; }
        public string Date5 { get; set; }
        public string Date6 { get; set; }
        #endregion
    }
}