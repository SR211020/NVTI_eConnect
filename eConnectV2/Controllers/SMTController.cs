using eConnectV2.Extensions;
using eConnectV2.Filters;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eConnectV2.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ActionFilterConfig))]
    public class SMTController : Controller
    {
        private readonly ISMTService _smtService;
        private readonly ICommonService _commonService;
        public SMTController(ISMTService smtService, ICommonService commonService)
        {
            _smtService = smtService;
            _commonService = commonService;
        }

        #region SMT MATERIAL TRACK
        public IActionResult Dashboard()
        {
            var model = new SMTViewModel();
            var data = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "GET_SMT_DASHBOARD",
                Param1 = "OVERVIEW"
            }));
            if (data.Count > 0)
            {
                model.TotalStoreMaterial = data.FirstOrDefault().TotalStoreMaterial;
                model.TotalDispatchMaterial = data.FirstOrDefault().TotalDispatchMaterial;
                model.TotalSmtReceived = data.FirstOrDefault().TotalSmtReceived;
                model.TotalSmtConsumed = data.FirstOrDefault().TotalSmtConsumed;
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult DashboardData(int subtractMonth)
        {
            var data = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "GET_SMT_DASHBOARD",
                Param1 = "MATERIAL_FLOW",
                Param2 = subtractMonth.ToString()
            }));
            return Json(data);
        }

        public IActionResult StoreAdmin()
        {
            var yearList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "GET_REC_YEAR" })); //{V=3,T=2020},{V=3,T=2021}, 
            yearList.Add(new SelectListItem() { Value = "4", Text = "All" });
            return View(new SMTViewModel()
            {
                SMTMaterialList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel() { Activity = "STORE_ADMIN", Param1 = "0" })),
                YearList = yearList
            });
        }
        [HttpPost]
        public IActionResult StoreAdmin(SMTViewModel model)
        {
            if (Convert.ToInt32(model.TimeFrame) > 4) { model.Year = model.TimeFrame; model.TimeFrame = "3"; }
            model.SMTMaterialList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "STORE_ADMIN",
                Param1 = model.TimeFrame?.Trim(),
                Param2 = model.Year?.Trim(),
                QRCode = model.QRCode?.Trim(),
                MaterialCategory = model.MaterialCategory?.Trim(),
                VendorName = model.VendorName?.Trim()
            }));
            var yearList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "GET_REC_YEAR" })); //{V=3,T=2020},{V=3,T=2021}, 
            yearList.Add(new SelectListItem() { Value = "4", Text = "All" });
            model.YearList = yearList;
            return View(model);
        }

        public IActionResult AddMaterial()
        {
            var model = new SMTViewModel();
            BindMaterialVendor(ref model);
            return View(model);
        }
        [HttpPost]
        public IActionResult AddMaterial(SMTViewModel model)
        {
            if (ModelState.IsValid)
            {
                int res = Common.CompareDate(model.MfgDate, model.Expdate);
                int expiry = Common.CompareDate(DateTime.Today, model.Expdate);
                if (model.MfgDate == model.Expdate)
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = "Expiry date and manufacturing date can not be same";
                }
                else if (res <= 0)
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = "Expiry date should be greather than manufacturing date.";
                }
                else if (expiry < 0)
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = "Material has been expired";
                }
                else
                {
                    var data = _smtService.usp_SMT(new SMTViewModel()
                    {
                        Activity = "STORE_RECD",
                        MaterialNo = model.MaterialNo.ToString().Trim(),
                        PartNo = model.PartNo.ToString().Trim(),
                        VendorName = model.VendorNameHdn.ToString().Trim(),
                        MaterialCategory = model.MaterialCategory.ToString().Trim(),
                        Date1 = model.MfgDate,
                        Date2 = model.Expdate,
                        EmpAdId = User.Identity.GetADId()
                    });
                    TempData["QRCode"] = data.Rows[0]["QRCODE"].ToString().Trim();
                    TempData["QRCodeImage"] = Common.GenerateQrCodeByText(data.Rows[0]["QRCODE"].ToString().Trim());
                    TempData["Result"] = data.Rows[0]["FLAG"].ToString().Trim();
                    TempData["Message"] = data.Rows[0]["MSG"].ToString().Trim();
                    TempData["WeekNo"] = data.Rows[0]["WEEKNO"].ToString().Trim();
                    if (data.Rows[0]["FLAG"].ToString().Trim() == Common.ResultError)
                    {
                        TempData["QRCodeImage"] = null;
                        BindMaterialVendor(ref model, model.MaterialCategory);
                        return View(model);
                    }
                    return RedirectToAction(nameof(AddMaterial));
                }
            }
            BindMaterialVendor(ref model, model.MaterialCategory);
            return View(model);
        }
        private SMTViewModel BindMaterialVendor(ref SMTViewModel model, string materialCode = null)
        {
            model.MaterialCategoryList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "FILL_VENDOR_CATG" }));
            if (!string.IsNullOrEmpty(materialCode))
            {
                model.SMTVendorList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "FILL_VENDOR_BY_CATG", MaterialCategory = model.MaterialCategory }));
            }
            return model;
        }

        public IActionResult MaterialDispatch()
        {
            var model = new SMTViewModel();
            BindMaterialVendor(ref model);
            BindLast50Transaction(ref model, "STORE");
            return View(model);
        }
        [HttpPost]
        public IActionResult MaterialDispatch(SMTViewModel model)
        {
            if (ModelState.IsValid)
            {
                var data = _smtService.usp_SMT(new SMTViewModel()
                {
                    Activity = "STORE_DISP",
                    MaterialCategory = model.MaterialCategory.ToString().Trim(),
                    PartNo = model.PartNo.ToString().Trim(),
                    QRCode = model.QRCode.ToString().Trim(),
                    EmpAdId = User.Identity.GetADId()
                });
                TempData["Result"] = data.Rows[0]["FLAG"].ToString().Trim();
                TempData["Message"] = data.Rows[0]["MSG"].ToString().Trim();
                if (data.Rows[0]["FLAG"].ToString().Trim() == Common.ResultError)
                {
                    BindMaterialVendor(ref model, model.MaterialCategory);
                    BindLast50Transaction(ref model, "STORE");
                    return View(model);
                }
                return RedirectToAction(nameof(MaterialDispatch));
            }
            BindMaterialVendor(ref model, model.MaterialCategory);
            BindLast50Transaction(ref model, "STORE");
            return View(model);
        }
        private void BindLast50Transaction(ref SMTViewModel model, string location)
        {
            model.SMTMaterialList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel() { Activity = "LAST_50_TRAN", Param1 = location }));
        }

        public IActionResult SmtAdmin()
        {
            var yearList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "GET_REC_YEAR" }));
            yearList.Add(new SelectListItem() { Value = "4", Text = "All" });
            return View(new SMTViewModel()
            {
                SMTMaterialList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel() { Activity = "SMT_ADMIN", Param1 = "0" })),
                YearList = yearList
            });
        }
        [HttpPost]
        public IActionResult SmtAdmin(SMTViewModel model)
        {
            if (Convert.ToInt32(model.TimeFrame) > 4) { model.Year = model.TimeFrame; model.TimeFrame = "3"; }
            model.SMTMaterialList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "SMT_ADMIN",
                Param1 = model.TimeFrame?.Trim(),
                Param2 = model.Year?.Trim(),
                QRCode = model.QRCode?.Trim(),
                MaterialCategory = model.MaterialCategory?.Trim(),
                VendorName = model.VendorName?.Trim()
            }));
            var yearList = Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "GET_REC_YEAR" })); //{V=3,T=2020},{V=3,T=2021}, 
            yearList.Add(new SelectListItem() { Value = "4", Text = "All" });
            model.YearList = yearList;
            return View(model);
        }

        public IActionResult SmtMaterial()
        {
            var model = new SMTViewModel();
            model.Action = "Receive";
            BindMaterialVendor(ref model);
            return View(model);
        }
        [HttpPost]
        public IActionResult SmtMaterial(SMTViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.EmpAdId = User.Identity.GetADId();
                if (model.Action == "Receive")
                {
                    model.Activity = "SMT_RECD";
                }
                else
                {
                    model.Activity = "SMT_CONSUME";
                    if (model.PartNo == "315130000024") //special case of GLUE
                    {
                        model.Activity = "SMT_CONSUME_GLUE";
                        model.SubQRCode = model.QRCode?.Trim();
                    }
                }
                var data = _smtService.usp_SMT(new SMTViewModel()
                {
                    Activity = model.Activity,
                    //MaterialCategory = model.MaterialCategory?.ToString().Trim(),
                    //PartNo = model.PartNo?.ToString().Trim(),
                    //QRCode = model.QRCode?.ToString().Trim(),
                    MaterialCategory = model.MaterialCategory.ToString().Trim(),
                    PartNo = model.PartNo.ToString().Trim(),
                    QRCode = model.QRCode.ToString().Trim(),
                    SubQRCode = model.SubQRCode.ToString().Trim(),
                    EmpAdId = User.Identity.GetADId()
                });
                if (data.Rows[0]["FLAG"].ToString().Trim() == Common.ResultSuccess && model.Activity == "SMT_RECD")
                {
                    model.WeekNo = data.Rows[0]["WEEKNO"].ToString().Trim();
                    var subQRCodeList = MaterialSplit(model);
                    TempData["SubQRCodeList"] = subQRCodeList;
                }
                TempData["Result"] = data.Rows[0]["FLAG"].ToString().Trim();
                TempData["Message"] = data.Rows[0]["MSG"].ToString().Trim();
            }
            BindMaterialVendor(ref model, model.MaterialCategory);
            return View(model);
        }
        private List<SplitMaterial> MaterialSplit(SMTViewModel model)
        {
            model.Activity = "SPLIT_MATERIAL";
            var subQRCodeList = new List<SplitMaterial>();
            int splitNo = Convert.ToInt32(model.Split);
            for (int i = 1; i <= splitNo; i++)
            {
                model.SubSeqNo = i.ToString();
                model.SubQRCode = model.QRCode.Trim() + "-S" + i;
                subQRCodeList.Add(new SplitMaterial { SubQRCode = model.SubQRCode, ImgSubQRCode = Common.GenerateQrCodeByText(model.SubQRCode), WeekNo = model.WeekNo });
                _ = _smtService.usp_SMT(model);
            }
            return subQRCodeList;
        }


        #region Jquery method
        [HttpPost]
        public JsonResult GenerateQRCode(string qrcode)
        {
            if (!string.IsNullOrEmpty(qrcode)) { return Json(Common.GenerateQrCodeByText(qrcode)); }
            return Json("");
        }
        [HttpPost]
        public JsonResult GetMaterialSuppliers(string material)
        {
            return Json(Common.BindDropDown(_smtService.usp_SMT(new SMTViewModel() { Activity = "FILL_VENDOR_BY_CATG", MaterialCategory = material })));
        }
        [HttpPost]
        public JsonResult BlockMaterial(string qrcode, string partno, string mcode, string area = null)
        {
            var data = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "MAT_BLOCK",
                QRCode = qrcode,
                Param1 = area,
                PartNo = partno,
                MaterialCategory = mcode,
                EmpAdId = User.Identity.GetADId()
            }));
            return Json(data);
        }
        [HttpPost]
        public JsonResult BlockSubMaterial(string qrcode, string subqrcode)
        {
            var data = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "SUBMAT_BLOCK",
                QRCode = qrcode,
                SubQRCode = subqrcode,
                EmpAdId = User.Identity.GetADId()
            }));
            return Json(data);
        }
        public JsonResult GetSplitMaterial(string qrcode)
        {
            var data = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT(new SMTViewModel()
            {
                Activity = "GET_SUBQRCODE",
                QRCode = qrcode
            }));
            return Json(data);
        }
        #endregion


        #endregion

        #region SMT BAKING PROCESS

        #region Manage Baking Model
        public IActionResult BakingModelAdd()
        {
            var data = new SMTViewModel()
            {
                ModelList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "FILL_LIST_BAKING_MODEL" })),
                Action = "ADD"
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult BakingModelAdd(SMTViewModel model)
        {
            int result = 0;
            if (ModelState.IsValid)
            {
                result = ManageModel(model);
                if (result == 1)
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = Common.ResultSuccessMessage;
                    return RedirectToAction(nameof(BakingModelAdd));
                }
                else if (result == 2)
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = Common.ResultUpdateMessage;
                    return RedirectToAction(nameof(BakingModelAdd));
                }
                else if (result == 0)
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.AlreadyExist;
                    model.ModelList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "FILL_LIST_BAKING_MODEL" }));
                    return View(model);
                }
            }
            TempData["Result"] = Common.ResultError;
            TempData["Message"] = Common.SomethingWentWrong;
            model.ModelList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "FILL_LIST_BAKING_MODEL" }));
            return View(model);
        }
        private int ManageModel(SMTViewModel model)
        {
            var dt = _smtService.usp_SMT_BAKING(new SMTViewModel()
            {
                Activity = "ADD_UPD_BAKING_MODEL",
                Param1 = model.Action,
                PartNo = model.PartNo,
                ModelName = model.ModelName,
                UserId = User.Identity.GetADId().ToString()
            });
            return Convert.ToInt32(dt.Rows[0]["FLAG"]);
        }
        [HttpPost]
        public JsonResult FillBakingModelByPartNo(string partNo)
        {
            var model = new SMTViewModel();
            model.Activity = "FILL_BAKING_MODEL_BY_PARTNO";
            model.PartNo = partNo;
            DataTable dt = _smtService.usp_SMT_BAKING(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.PartNo = Convert.ToString(dataRow["PARTNO"]);
                model.ModelName = Convert.ToString(dataRow["MODELNAME"]);
            }
            return Json(model);
        }

        public IActionResult DeleteBakingModel(string id)
        {
            var dt = _smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "DELETE_BAKING_MODEL", PartNo = id, UserId = User.Identity.GetADId().ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = "Material is baked successfully" });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        #endregion

        #region Add Baking Process
        public IActionResult BakingProcessAdd()
        {
            var data = new SMTViewModel()
            {
                Action = "ADD",
                DDLPartList = Common.BindDropDown(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "DDL_PARTNO" })),
                BakedListDetail = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "GET_BAKING_ITEM_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult BakingProcessAdd(SMTViewModel model)
        {
            int result = 0;
            if (ModelState.IsValid)
            {
                result = ManageBake(model);
                if (result == 1)
                {
                    TempData["Result"] = Common.ResultSuccess;
                    TempData["Message"] = Common.ResultSuccessMessage;
                    return RedirectToAction(nameof(BakingProcessAdd));
                }
                else if (result == 0)
                {
                    TempData["Result"] = Common.ResultError;
                    TempData["Message"] = Common.AlreadyExist;
                    model.BakedListDetail = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "GET_BAKING_LIST" }));
                    return View(model);
                }
            }
            TempData["Result"] = Common.ResultError;
            TempData["Message"] = Common.SomethingWentWrong;
            model.BakedListDetail = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "GET_BAKING_LIST" }));
            return View(model);
        }
        private int ManageBake(SMTViewModel model)
        {
            var dt = _smtService.usp_SMT_BAKING(new SMTViewModel()
            {
                Activity = "ADD_UPD_BAKING_ITEM",
                Param1 = model.MatCode,
                Param2 = model.MfgDate,
                Param3 = model.LotNo,
                Param4 = model.PartNo,
                Param5 = model.Quantity,
                Param6 = User.Identity.GetADId().ToString(),
                Param7 = model.Action,
                Param8 = model.Remarks,
                Param9 = model.InOperator.ToUpper()
            });
            return Convert.ToInt32(dt.Rows[0]["FLAG"]);
        }
        public JsonResult UpdateBakingItem(string matCode, string remarks, string operatorId)
        {
            var dt = _smtService.usp_SMT_BAKING(new SMTViewModel()
            {
                Activity = "ADD_UPD_BAKING_ITEM",
                Action = "Update",
                Param1 = matCode,
                Param6 = User.Identity.GetADId().ToString(),
                Param8 = remarks,
                Param9 = operatorId
            });
            if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultSuccess, msg = "Material is baked successfully" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "3")
            {
                return Json(new { result = Common.ResultError, msg = "Material is not baked yet !" });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }

        public IActionResult BakingProcessView()
        {
            var data = new SMTViewModel();
            DateTime dtToday = DateTime.Now;
            var startDate = new DateTime(dtToday.Year, dtToday.Month, 1);

            data.Date1 = data.Date1 = startDate.ToString("dd'-'MMM'-'yyyy");
            data.Date2 = data.Date2 = dtToday.ToString("dd'-'MMM'-'yyyy");

            data.DDLPartList = Common.BindDropDown(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "DDL_PARTNO" }));
            data.BakingList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "FILL_BAKING_PROCESS", Date1 = data.Date1, Date2 = data.Date2 }));
            return View(data);
        }
        [HttpPost]
        public IActionResult BakingProcessView(SMTViewModel model)
        {
            model.DDLPartList = Common.BindDropDown(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "DDL_PARTNO" }));
            model.BakingList = Common.ConvertDataTableToList<SMTViewModel>(_smtService.usp_SMT_BAKING(new SMTViewModel() { Activity = "FILL_BAKING_PROCESS", Date1 = model.Date1, Date2 = model.Date2, Param1 = model.LotNo, Param2 = model.PartNo }));
            return View(model);
        }


        public JsonResult BindMatDetail(string matCode)
        {
            var model = new SMTViewModel();
            model.Activity = "GET_ITEM_DETAIL";
            model.Param1 = matCode;
            DataTable dt = _smtService.usp_SMT_BAKING(model);
            foreach (DataRow dataRow in dt.Rows)
            {
                model.MatCode = Convert.ToString(dataRow["MATCODE"]);
                model.LotNo = Convert.ToString(dataRow["LOTNO"]);
                model.PartNo = Convert.ToString(dataRow["PARTNO"]);
                model.MfgDate = Convert.ToString(dataRow["MFGDATE"]);
                model.Quantity = Convert.ToString(dataRow["QUANTITY"]);
                model.Param1 = Convert.ToString(dataRow["PARAM1"]);
            }
            return Json(model);
        }
        #endregion

        #endregion
    }
}
