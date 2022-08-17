using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eConnectV2.ViewModels
{
    public class EISViewModel
    {
        public EISViewModel()
        {
            CategoryList = new List<SelectListItem>();
            LineList = new List<SelectListItem>();
            LineList2 = new List<SelectListItem>();
            CustomerList = new List<SelectListItem>();
            MonthList = new List<SelectListItem>();
            MatCodeList = new List<SelectListItem>();
            GridList = new List<EISViewModel>();
        }

        #region Drop Down List
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> LineList { get; set; }
        public List<SelectListItem> LineList2 { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> MatCodeList { get; set; }
        #endregion


        #region Grid List
        public List<EISViewModel> GridList { get; set; }
        #endregion

        #region Stored Procedure Parameters 
        public string Activity { get; set; }
        public string Plant { get; set; }
        public string Division { get; set; }
        public int FromYear { get; set; }
        public int FromMonth { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        #endregion

        #region Common Parameters 
        public int DocNo { get; set; }
        public string DataDate { get; set; }
        public int Yr { get; set; }
        public int Mon { get; set; }
        public string MonthName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Value1 { get; set; }
        public string GroupName { get; set; }
        public string UserId { get; set; }
        public string TDate { get; set; }

        public string hdnField1 { get; set; }
        public string hdnField2 { get; set; }
        public string hdnField3 { get; set; }
        public string hdnField4 { get; set; }

        public string Category { get; set; }
        public string LineType { get; set; }
        public string Customer { get; set; }
        public decimal Planned { get; set; }
        public double Actual { get; set; }
        public decimal Bottom { get; set; }
        public decimal Basic { get; set; }
        public decimal Challenge { get; set; }
        #endregion

        #region Graphs 
        public string Box1 { get; set; }
        public string Box2 { get; set; }
        public string Box3 { get; set; }
        public string SqPlant { get; set; }
        public string SqMatCode { get; set; }
        public string SqDate1 { get; set; }
        public string SqDate2 { get; set; }

        public int SalesFromYr { get; set; }
        public int SalesFromMon { get; set; }
        public int SalesToYr { get; set; }
        public int SalesToMon { get; set; }

        public int CellRevFromYr { get; set; }
        public int CellRevFromMon { get; set; }
        public int CellRevToYr { get; set; }
        public int CellRevToMon { get; set; }

        public int PackRevFromYr { get; set; }
        public int PackRevFromMon { get; set; }
        public int PackRevToYr { get; set; }
        public int PackRevToMon { get; set; }

        public int ScrFromYr { get; set; }
        public int ScrFromMon { get; set; }
        public int ScrToYr { get; set; }
        public int ScrToMon { get; set; }

        public int MatFromYr { get; set; }
        public int MatFromMon { get; set; }
        public int MatToYr { get; set; }
        public int MatToMon { get; set; }

        public int DepFromYr { get; set; }
        public int DepFromMon { get; set; }
        public int DepToYr { get; set; }
        public int DepToMon { get; set; }

        public int DLFromYr { get; set; }
        public int DLFromMon { get; set; }
        public int DLToYr { get; set; }
        public int DLToMon { get; set; }

        public int MOHFromYr { get; set; }
        public int MOHFromMon { get; set; }
        public int MOHToYr { get; set; }
        public int MOHToMon { get; set; }

        public int IFRFromYr { get; set; }
        public int IFRFromMon { get; set; }
        public int IFRToYr { get; set; }
        public int IFRToMon { get; set; }

        public int SGAFromYr { get; set; }
        public int SGAFromMon { get; set; }
        public int SGAToYr { get; set; }
        public int SGAToMon { get; set; }

        public int IDLFromYr { get; set; }
        public int IDLFromMon { get; set; }
        public int IDLToYr { get; set; }
        public int IDLToMon { get; set; }

        public int RlyFromYr { get; set; }
        public int RlyFromMon { get; set; }
        public int RlyToYr { get; set; }
        public int RlyToMon { get; set; }

        public int OFRFromYr { get; set; }
        public int OFRFromMon { get; set; }
        public int OFRToYr { get; set; }
        public int OFRToMon { get; set; }

        public int FrxFromYr { get; set; }
        public int FrxFromMon { get; set; }
        public int FrxToYr { get; set; }
        public int FrxToMon { get; set; }

        public int OtcFromYr { get; set; }
        public int OtcFromMon { get; set; }
        public int OtcToYr { get; set; }
        public int OtcToMon { get; set; }

        public int OicFromYr { get; set; }
        public int OicFromMon { get; set; }
        public int OicToYr { get; set; }
        public int OicToMon { get; set; }

        public int TaxFromYr { get; set; }
        public int TaxFromMon { get; set; }
        public int TaxToYr { get; set; }
        public int TaxToMon { get; set; }

        public int GPFromYr { get; set; }
        public int GPFromMon { get; set; }
        public int GPToYr { get; set; }
        public int GPToMon { get; set; }

        public int OPFromYr { get; set; }
        public int OPFromMon { get; set; }
        public int OPToYr { get; set; }
        public int OPToMon { get; set; }

        public int NPFromYr { get; set; }
        public int NPFromMon { get; set; }
        public int NPToYr { get; set; }
        public int NPToMon { get; set; }

        public int GPFromYr2 { get; set; }
        public int GPFromMon2 { get; set; }
        public int GPToYr2 { get; set; }
        public int GPToMon2 { get; set; }

        public int OPFromYr2 { get; set; }
        public int OPFromMon2 { get; set; }
        public int OPToYr2 { get; set; }
        public int OPToMon2 { get; set; }

        public int NPFromYr2 { get; set; }
        public int NPFromMon2 { get; set; }
        public int NPToYr2 { get; set; }
        public int NPToMon2 { get; set; }

        public int GPFromYr3 { get; set; }
        public int GPFromMon3 { get; set; }
        public int GPToYr3 { get; set; }
        public int GPToMon3 { get; set; }

        public int OPFromYr3 { get; set; }
        public int OPFromMon3 { get; set; }
        public int OPToYr3 { get; set; }
        public int OPToMon3 { get; set; }

        public int NPFromYr3 { get; set; }
        public int NPFromMon3 { get; set; }
        public int NPToYr3 { get; set; }
        public int NPToMon3 { get; set; }

        public decimal GPAmount { get; set; }
        public decimal OPAmount { get; set; }
        public decimal NPAmount { get; set; }

        public decimal GPPer { get; set; }
        public decimal OPPer { get; set; }
        public decimal NPPer { get; set; }

        public string InvPlant { get; set; }
        public string InvDivision { get; set; }
        public string InvDate1 { get; set; }
        public string InvDate2 { get; set; }

        public string InvPlant2 { get; set; }
        public string InvDivision2 { get; set; }
        public string InvAmtDate1 { get; set; }
        public string InvAmtDate2 { get; set; }

        public string InvPlant3 { get; set; }
        public string InvDivision3 { get; set; }
        public string InvFGDate1 { get; set; }
        public string InvFGDate2 { get; set; }

        public string OQAPlant { get; set; }
        public string OQADate1 { get; set; }
        public string OQADate2 { get; set; }

        public string UPHPlant { get; set; }
        public string UPHDivision { get; set; }
        public string UPHLineType { get; set; }
        public string UPHDate1 { get; set; }
        public string UPHDate2 { get; set; }

        public string UPPHPlant { get; set; }
        public string UPPHDivision { get; set; }
        public string UPPHLineType { get; set; }
        public string UPPHCust { get; set; }
        public string UPPHDate1 { get; set; }
        public string UPPHDate2 { get; set; }

        public string ALPlant { get; set; }
        public string ALYear { get; set; }
        public string ALMonth { get; set; }

        #endregion
    }
}
