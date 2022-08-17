using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class DTViewModel
    {
        public DTViewModel()
        {
            EmpTypeList = new List<SelectListItem>();
            EmpGradeList = new List<SelectListItem>();
            EmpStatusList = new List<SelectListItem>();
            ManPowerList = new List<DTViewModel>();
            StationList = new List<SelectListItem>();
            QuestionList = new List<DTViewModel>();
            OJTDocumentList = new List<DTViewModel>();
            TrainerList = new List<SelectListItem>();
            EmpList = new List<SelectListItem>();
            ManPowerDDLList = new List<SelectListItem>();
            OJTAttendanceList = new List<DTViewModel>();
            ApproverList = new List<DTViewModel>();
            CustomerDDLList = new List<SelectListItem>();
        }
        public List<SelectListItem> EmpTypeList { get; set; }
        public List<SelectListItem> EmpGradeList { get; set; }
        public List<SelectListItem> EmpStatusList { get; set; }
        public List<DTViewModel> ManPowerList { get; set; }
        public List<SelectListItem> StationList { get; set; }
        public List<DTViewModel> QuestionList { get; set; }
        public List<DTViewModel> OJTDocumentList { get; set; }
        public List<SelectListItem> TrainerList { get; set; }
        public List<SelectListItem> EmpList { get; set; }
        public List<SelectListItem> ManPowerDDLList { get; set; }
        public List<DTViewModel> OJTAttendanceList { get; set; }
        public List<DTViewModel> ApproverList { get; set; }
        public List<SelectListItem> CustomerDDLList { get; set; }












        public string Activity { get; set; }
        public int ShowActiveManPowerList { get; set; }
        public string AadharNo { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public string Qualification { get; set; }
        public string DOJ { get; set; }
        public string EmpType { get; set; }
        public string EmpStatus { get; set; }
        public string Grade { get; set; }
        public string Reason { get; set; }
        public bool IsEmpReJoin { get; set; }
        public string OldEmpCode { get; set; } = "";
        public string AdId { get; set; }
        public bool IsEmpStatusLeft { get; set; }
        public string Station { get; set; }
        public string DOL { get; set; }
        public string DocUId { get; set; }
        public int Id { get; set; }
        public string Que { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Ans { get; set; }
        public string Action { get; set; }
        public byte[] AttachDocument { get; set; }
        public string TrainingDate { get; set; }
        public string TrainingFor { get; set; }
        public string Trainer { get; set; }
        public string EmpCodes { get; set; }
        public string Impact { get; set; }
        public string SelectedImpact { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryApprover { get; set; }
        public string Approver1 { get; set; }
        public string Approver2 { get; set; }
        public string Approver3 { get; set; }
        public string Approver4 { get; set; }
        public string Approver5 { get; set; }
        public string Approver6 { get; set; }
        public string TestConductedByEmpCode { get; set; }
        public string TestConductedByEmpName { get; set; }
        public string TestEmpCode { get; set; }
        public string TestEmpName { get; set; }
        public string TestStation { get; set; }
        public string TestStationName { get; set; }
        public string TestCurrentGrade { get; set; }
        public string TestCustomer { get; set; }
        public string TestCustomerName { get; set; }
        public string TestForGrade { get; set; }
        public string SelectedAnsCode { get; set; }
        public string SelectedAnsName { get; set; }
        public int AttemptedAns { get; set; }
        public string AnsText { get; set; }
        public int IsAnsCorrect { get; set; } = 0;
        public int TestScore { get; set; }
        public int TestScoreOutof { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string TestResult { get; set; }










        public string CertificationDate { get; set; }
        //public string EmpGrade { get; set; }
        public string SkillSet { get; set; }





        


        
        

        //public int QueId { get; set; }
        
        


        
        //public int CustId { get; set; }
        

       
       

        
        public int OJTAttendId { get; set; }



        
        
       
        public string TestDate { get; set; }
        

       
        
       

    }
}
