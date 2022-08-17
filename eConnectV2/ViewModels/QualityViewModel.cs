using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class QualityViewModel
    {
        public QualityViewModel()
        {
            EmployeeList = new List<SelectListItem>();
            PlantList = new List<SelectListItem>();
            DivisionList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            CustomerList = new List<SelectListItem>();
            ApproverList = new List<SelectListItem>();
            MakeList = new List<SelectListItem>();
            DepartmentList = new List<QualityViewModel>();
            HistoryList = new List<QualityViewModel>();
            CertificateList = new List<QualityViewModel>();
        }

        #region Data Binding Lists
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> DivisionList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> ApproverList { get; set; }
        public List<QualityViewModel> DepartmentList { get; set; }
        public List<SelectListItem> MakeList { get; set; }
        public List<QualityViewModel> HistoryList { get; set; }
        public List<QualityViewModel> CertificateList { get; set; }
        #endregion

        public string Activity { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Date3 { get; set; }
        public string Date4 { get; set; }

        public int RequestNo { get; set; }
        public int SrNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string ContactNo { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DeptHOD { get; set; }
        public string HODName { get; set; }

        public string Remarks { get; set; }
        public int Status { get; set; }
        public string StatusDescr { get; set; }
        public string UserType { get; set; }
        public string UserID { get; set; }
        public string TDate { get; set; }
        public string ApprID { get; set; }
        public string ApprStatus { get; set; }
        public string ApprType { get; set; }
        public string ApprName { get; set; }
        public string NextApprID { get; set; }
        public string NextApprType { get; set; }
        public string Appr1 { get; set; }
        public string Appr2 { get; set; }

        public string Type { get; set; }
        public string Reason { get; set; }
        public string DocNo { get; set; }
        public string DocName { get; set; }
        public string DocChanges { get; set; }
        public string ChangeRequireIn { get; set; }
        public string ChangeDocNo { get; set; }
        public string ChangeDocName { get; set; }
        public string DCC { get; set; }
        public string DCCDocNo { get; set; }
        public string DCCDocName { get; set; }
        public string DCCReqType { get; set; }
        public string DCCStatus { get; set; }
        public string File_Name { get; set; }

        public string Plant { get; set; }
        public string Division { get; set; }
        public string ModelName { get; set; }
        public string PartNo { get; set; }
        public string Qty { get; set; }
        public string Customer { get; set; }
        public string NatureOfIssue { get; set; }
        public string ActionProposed { get; set; }
        public string RootCause { get; set; }
        public string CorrectiveAction { get; set; }
        

        public string Category { get; set; }
        public string EquipId { get; set; }
        public string CalibrationStatus { get; set; }
        public string CertificateStatus { get; set; }
        public string Description { get; set; }
        public string Make { get; set; }
        public string EquipModel { get; set; }
        public string Unit { get; set; }
        public string DueDays { get; set; }
        public string Agency { get; set; }
        public string Location { get; set; }
        public string MacName { get; set; }
        public string LeastCnt { get; set; }
        public string FullRange { get; set; }
        public string OperatingRange { get; set; }
        public string BoxId { get; set; }
        public int Period { get; set; }
        public decimal Quantity { get; set; }

        public string Product { get; set; }
        public string ScrapStatus { get; set; }
        public int ScrapCount1 { get; set; }
        public int ScrapCount2 { get; set; }

    }
}
