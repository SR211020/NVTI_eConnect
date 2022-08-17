using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class AttendanceViewModel
    {
        public AttendanceViewModel()
        {
            EmployeeList = new List<AttendanceViewModel>();
        }

        #region Lists
        public List<AttendanceViewModel> EmployeeList { get; set; }
        #endregion

        public string Category { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string ContactNo { get; set; }
        public string ImageName { get; set; }


        #region Stored Procedure Parameters
        public string Activity { get; set; }
        public string Department { get; set; }
        public int Status { get; set; }
        public string UserID { get; set; }
        #endregion
    }
}
