using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class MiscViewModel
    {
        public MiscViewModel()
        {
            ImageList = new List<MiscViewModel>();
            RecordList = new List<MiscViewModel>();
        }

        #region Data Binding Lists
        public List<MiscViewModel> ImageList { get; set; }
        public List<MiscViewModel> RecordList { get; set; }
        #endregion

        public byte[] BImage { get; set; }


        public string UserID { get; set; }
        public string DocName { get; set; }
        public string Remarks { get; set; }


        #region Stored Procedure Parameters
        public string Activity { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        #endregion
    }
}
