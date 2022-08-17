using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace eConnectV2.ViewModels
{
    public class BISViewModel
    {
        public BISViewModel()
        {
            GroupList = new List<SelectListItem>();
            LineNoList = new List<SelectListItem>();
            ShiftList = new List<SelectListItem>();
        }

        public List<SelectListItem> GroupList { get; set; }
        public List<SelectListItem> LineNoList { get; set; }
        public List<SelectListItem> ShiftList { get; set; }

        public DataTable dt { get; set; }

        public int DocNo { get; set; }
        public string Plant { get; set; }
        public string Division { get; set; }
        public string CustomerName { get; set; }
        public string LineNo { get; set; }
        public string LineType { get; set; }
        public string ModelName { get; set; }
        public string Shift { get; set; }
        public int Variance1 { get; set; }
        public int Variance2 { get; set; }
        public int Variance3 { get; set; }
        public int Variance4 { get; set; }
        public int Variance5 { get; set; }
        public int Variance6 { get; set; }
        public int Variance7 { get; set; }
        public int Variance8 { get; set; }

        public int Plan { get; set; }
        public int Actual { get; set; }
        public int StdHC { get; set; }
        public int ActualHC { get; set; }
        public int UphTarget { get; set; }
        public decimal UpphTarget { get; set; }
        public string UpphDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

        public decimal CellCount { get; set; }
        public decimal MappingCount { get; set; }
        public decimal PackSizeCount { get; set; }
        public decimal FtCount { get; set; }
        public decimal PackingCount { get; set; }
        public decimal ScreenPrintCount { get; set; }
        public decimal SpiCount { get; set; }
        public decimal AoiCount { get; set; }
        public decimal CoatingCount { get; set; }

        public int Total { get; set; }
        public int TotalPass { get; set; }
        public int NG { get; set; }
        public int FirstPass { get; set; }
        public int SecondPass { get; set; }
        public decimal FPY { get; set; }
        public string hdnField1 { get; set; }
        public string hdnField2 { get; set; }
        public string hdnField3 { get; set; }


        #region Stored Procedure Parameters
        public string Activity { get; set; }
        public string UserId { get; set; }
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
    }

}
