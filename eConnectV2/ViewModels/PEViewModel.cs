using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eConnectV2.ViewModels
{
    public class PEViewModel
    {
        public PEViewModel()
        {
            BinList = new List<PEViewModel>();
            CustomerList = new List<SelectListItem>();
            ModelNameList = new List<SelectListItem>();
            LineNoList = new List<SelectListItem>();
            ScrapCatgList = new List<SelectListItem>();
            ScrapSubCatgList = new List<SelectListItem>();
            MCatgList = new List<SelectListItem>();
            ProdList = new List<PEViewModel>();
        }


        public List<PEViewModel> BinList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> ModelNameList { get; set; }
        public List<SelectListItem> LineNoList { get; set; }
        public List<SelectListItem> ScrapCatgList { get; set; }
        public List<SelectListItem> ScrapSubCatgList { get; set; }
        public List<SelectListItem> MCatgList { get; set; }
        public List<PEViewModel> ProdList { get; set; }

        public int DocNo { get; set; }
        public string Activity { get; set; }
        public string Customer { get; set; }
        public string ModelName { get; set; }
        public string ModelNameHdn { get; set; }
        public string Line { get; set; }
        public string ScrapCatg { get; set; }
        public string ScrapSubCatg { get; set; }
        public string MCatg { get; set; }
        public string Shift { get; set; }
        public string BarCode { get; set; }
        public string OperatorId { get; set; }
        public string Remarks { get; set; }
        public string UserId { get; set; }
        public string ScrapDate { get; set; }
        public string ProdDate { get; set; }
        public int Quantity { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
    }
}
