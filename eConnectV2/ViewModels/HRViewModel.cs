using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class HRViewModel
    {
        public HRViewModel()
        {
            QuestionList = new List<HRViewModel>();
            QuestionList2 = new List<HRViewModel>();
            DDLOptionList = new List<SelectListItem>();
            DDLOptionList2 = new List<SelectListItem>();
            DDLOptionList3 = new List<SelectListItem>();
            EmpList = new List<SelectListItem>();
            DueOnboardSurveyList = new List<HRViewModel>();
        }
        public List<SelectListItem> EmpList { get; set; }
        public List<HRViewModel> QuestionList { get; set; }
        public List<HRViewModel> QuestionList2 { get; set; }
        public List<SelectListItem> DDLOptionList { get; set; }
        public List<SelectListItem> DDLOptionList2 { get; set; }
        public List<SelectListItem> DDLOptionList3 { get; set; }
        public List<HRViewModel> DueOnboardSurveyList { get; set; }

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
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        #endregion

        public string RatingFinYear { get; set; }
        public string DefaultFinYear { get; set; }
        public string DefaultDivision { get; set; }
        public string Ans1 { get; set; }
        public string Ans2 { get; set; }
        public string Ans3 { get; set; }
        public string Ans4 { get; set; }
        public string Ans5 { get; set; }
        public string Ans6 { get; set; }
        public string Ans7 { get; set; }
        public string Ans8 { get; set; }
        public string SubQue { get; set; }
        public string EmpCode { get; set; }


        #region onboarding
        public string SurveyType { get; set; }
        public string EMPNAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPTNAME { get; set; }
        public string EMAILID { get; set; }
        public string DOJ { get; set; }
        public string DAY5_SURVEY { get; set; }
        public string DAY60_SURVEY { get; set; }
        public string DAY120_SURVEY { get; set; }
        public string DAY5 { get; set; }
        public string DAY60 { get; set; }
        public string DAY120 { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string DimensionCategory { get; set; }
        public string ExperiencePillar { get; set; }
        public string ReasonCategory { get; set; }
        public string Remark { get; set; }
        #endregion
    }
}
