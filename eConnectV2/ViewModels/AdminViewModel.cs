using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class AdminViewModel
    {
        public AdminViewModel()
        {
            PlantList = new List<SelectListItem>();
            MenuList = new List<AdminViewModel>();
            SubMenuList = new List<AdminViewModel>();
            DeptList = new List<AdminViewModel>();
            EmpList = new List<AdminViewModel>();
            CodeList = new List<AdminViewModel>();
            CodeDefList = new List<AdminViewModel>();
            ConfigList = new List<AdminViewModel>();
            ContactList = new List<AdminViewModel>();
            DDLEmpList = new List<SelectListItem>();
        }
        public List<SelectListItem> PlantList { get; set; }
        public List<AdminViewModel> MenuList { get; set; }
        public List<AdminViewModel> SubMenuList { get; set; }
        public List<AdminViewModel> DeptList { get; set; }
        public List<AdminViewModel> EmpList { get; set; }
        public List<AdminViewModel> CodeList { get; set; }
        public List<AdminViewModel> CodeDefList { get; set; }
        public List<AdminViewModel> ConfigList { get; set; }
        public List<SelectListItem> DDLEmpList { get; set; }
        public List<AdminViewModel> ContactList { get; set; }

        #region param list
        public string Activity { get; set; }
        public string UserId { get; set; }
        public byte[] BImage { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string Param7 { get; set; }
        public string Param8 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }

        #endregion

        public int MenuId { get; set; }
        public int SubMenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public string Url { get; set; }
        public int Status { get; set; }
        public int MenuOrder { get; set; }
        public int HasChild { get; set; }
        public string SubMenuName { get; set; }

        public int DeptId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string HodADID { get; set; }
        public string HodName { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public string EmpAdId { get; set; }
        public string EmpEmail { get; set; }
        public string Plant { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string LandLine { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string ApprId { get; set; }
        public string ApprName { get; set; }

        public int Code { get; set; }
        public string CodeDesc { get; set; }
        public int CodeDefKeyId { get; set; }
        public string CodeDefDesc { get; set; }
        public string CodeDefValue { get; set; }

        public int ConfigId { get; set; }
        public string Module { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string ConfigValue { get; set; }
        public int Lvl { get; set; }

        public int SrNo { get; set; }
        public string Contact { get; set; }
        public string ExtNo { get; set; }
        public string ImageName { get; set; }
    }
}
