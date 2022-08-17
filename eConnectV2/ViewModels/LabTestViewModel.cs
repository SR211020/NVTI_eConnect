using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.ViewModels
{
    public class LabTestViewModel
    {
        public LabTestViewModel()
        {
            SampleList = new List<LabTestViewModel>();
            SampleRunningList = new List<LabTestViewModel>();
        }
        public List<LabTestViewModel> SampleList { get; set; }
        public List<LabTestViewModel> SampleRunningList { get; set; }
        public string Activity { get; set; }
        public int ID { get; set; }
        public string Product { get; set; }
        public string ProductOther { get; set; }
        public string ModelName { get; set; }
        public string PartNumber { get; set; }
        public string SampleQty { get; set; }
        public string TestName { get; set; }
        public string TestConditionIfAny { get; set; }
        public string Department { get; set; }
        public string RequestDate { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public string Status { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string RejectReason { get; set; }
        public string TestStartDate { get; set; }
        public byte[] AttachDocument { get; set; }
        public string DocUId { get; set; }
        public string ReqEmail { get; set; }
    }
}
