using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class ITViewModel
    {
        public ITViewModel()
        {
            PlantList = new List<SelectListItem>();
            EmployeeList = new List<SelectListItem>();
            DepartmentList = new List<SelectListItem>();
            TypeList = new List<SelectListItem>();
            CategoryList = new List<SelectListItem>();
            SubCategoryList = new List<SelectListItem>();
            MakeList = new List<SelectListItem>();
            VendorList = new List<SelectListItem>();
            AreaList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            HistoryList = new List<ITViewModel>();
            ApproverList = new List<SelectListItem>();
        }

        #region Lists
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> SubCategoryList { get; set; }
        public List<SelectListItem> MakeList { get; set; }
        public List<SelectListItem> VendorList { get; set; }
        public List<SelectListItem> AreaList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<ITViewModel> HistoryList { get; set; }
        public List<SelectListItem> ApproverList { get; set; }
        #endregion

        public int RequestNo { get; set; }
        public int SrNo { get; set; }
        public string Type { get; set; }
        public string Plant { get; set; }
        public string EmpCode { get; set; }
        public string EmpCode2 { get; set; }
        public string EmpName { get; set; }
        public string EmpName2 { get; set; }
        public string DeptCode { get; set; }
        public string DeptCode2 { get; set; }
        public string DeptName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string ExtNo { get; set; }
        public string Priority { get; set; }
        public string ProbCatg { get; set; }
        public string SubCatg { get; set; }
        public string Subject { get; set; }
        public string Solution { get; set; }
        public string DocName { get; set; }
        public string DocNameEdit { get; set; }
        public string Descr1 { get; set; }
        public string Area { get; set; }

        public string Remarks { get; set; }
        public string UserID { get; set; }
        public string UserType { get; set; }
        public string TDate { get; set; }
        public int Status { get; set; }
        public string StatusDescr { get; set; }
        public string ApprID { get; set; }
        public string ApprName { get; set; }
        public string ApprStatus { get; set; }
        public string ApprType { get; set; }
        public string DeptHOD { get; set; }
        public string ITHOD { get; set; }
        public string ITEngineer { get; set; }

        public int HDCount1 { get; set; }
        public int HDCount2 { get; set; }
        public int HDCount3 { get; set; }
        public int HDTotal { get; set; }

        public int Count1 { get; set; }
        public int Count2 { get; set; }
        public int Count3 { get; set; }
        public string TotalTime { get; set; }

        #region Graphs
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Closed { get; set; }
        public string GroupName { get; set; }
        #endregion

        #region IT Asset
        public string EquipCatg { get; set; }
        public string Make { get; set; }
        public string ModelName { get; set; }
        public string InvoiceNo { get; set; }
        public string PONo { get; set; }
        public string VendorName { get; set; }
        public int Quantity { get; set; }
        public string Amount { get; set; }
        public string DeviceSerialNo { get; set; }
        public string PermanentAssetNo { get; set; }
        public string Processor { get; set; }
        public string CPUSpeed { get; set; }
        public string RAM { get; set; }
        public string HDDType { get; set; }
        public string HDDCapacity { get; set; }
        public string DVD { get; set; }
        public string OSVersion { get; set; }

        #endregion

        #region HD User Create
        public string ReqFor { get; set; }
        public string IdType { get; set; }
        public string EmailAccess { get; set; }
        public string Roles { get; set; }
        public string AdId { get; set; }
        public string EmailId { get; set; }
        public string BIS { get; set; }
        public string MES { get; set; }
        public string SAP { get; set; }
        public string Asset { get; set; }
        #endregion

        #region Stored Procedure Parameters
        public string Activity { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Date3 { get; set; }
        public string Date4 { get; set; }
        public string Date5 { get; set; }
        public string Date6 { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        #endregion
    }
}
