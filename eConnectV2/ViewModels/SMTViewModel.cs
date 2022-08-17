using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class SMTViewModel
    {
        public SMTViewModel()
        {
            SMTVendorList = new List<SelectListItem>();
            MaterialCategoryList = new List<SelectListItem>();
            YearList = new List<SelectListItem>();
            SMTMaterialList = new List<SMTViewModel>();

            #region BAKING PROCESS 
            BakingList = new List<SMTViewModel>();
            BakedListDetail = new List<SMTViewModel>();
            ModelList = new List<SMTViewModel>();
            DDLPartList = new List<SelectListItem>();
            #endregion
        }
        public List<SelectListItem> SMTVendorList { get; set; }
        public List<SelectListItem> MaterialCategoryList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SMTViewModel> SMTMaterialList { get; set; }

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
        #endregion

        #region result Set
        public int TotalStoreMaterial { get; set; } = 0;
        public int TotalDispatchMaterial { get; set; } = 0;
        public int TotalSmtReceived { get; set; } = 0;
        public int TotalSmtConsumed { get; set; } = 0;
        public string Mon { get; set; }
        public string Rec { get; set; }
        public string Con { get; set; }

        public string Employee { get; set; }
        public string TDate { get; set; }
        public string EmpAdId { get; set; }
        public string VendorName { get; set; }

        public string MaterialCategory { get; set; }
        public string VendorNameHdn { get; set; }
        public string PartNo { get; set; }
        public string MfgDate { get; set; }
        public string Expdate { get; set; }
        public string QRCode { get; set; }
        public string Action { get; set; }
        public string Remarks { get; set; }
        public string BarCode { get; set; }
        public int Flag { get; set; }
        public string Msg { get; set; }
        public string WeekNo { get; set; }
        public string StoreRecdDate { get; set; }
        public string StoreDispatch { get; set; }
        public string StoreDispatchDate { get; set; }
        public string IsBlocked { get; set; }
        public string BlockDate { get; set; }
        public string SmtRecdDate { get; set; }
        public string SmtConsumeDate { get; set; }
        public string SmtConsume { get; set; }
        public string MaterialNo { get; set; }
        public string TimeFrame { get; set; }
        public string Year { get; set; }
        public string SubQRCode { get; set; }
        public string SubSeqNo { get; set; }
        public string Split { get; set; }


        #region BAKING PROCESS 
        public List<SMTViewModel> BakingList { get; set; }
        public List<SMTViewModel> BakedListDetail { get; set; }
        public List<SMTViewModel> ModelList { get; set; }
        public List<SelectListItem> DDLPartList { get; set; }
        public int DocNo { get; set; }
        public string ModelName { get; set; }
        public string InOperator { get; set; }
        public string OutOperator { get; set; }
        public string UserId { get; set; }

        public string hdnField1 { get; set; }
        public string MatCode { get; set; }
        public string LotNo { get; set; }
        public int BakingPartNo { get; set; }
        public string Quantity { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int LeftTime { get; set; }
        #endregion

        #endregion
    }

    public class SplitMaterial
    {
        public string SubQRCode { get; set; }
        public string ImgSubQRCode { get; set; }
        public string WeekNo { get; set; }
    }
}
