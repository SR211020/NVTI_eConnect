using eConnectV2.Extensions;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class MiscController : Controller
    {
        private readonly IMiscService _miscService;
        private readonly ICommonService _commonService;

        public MiscController(IMiscService miscService, ICommonService commonService)
        {
            _miscService = miscService;
            _commonService = commonService;
        }

        #region Slider
        public ActionResult Slider()
        {
            return View(new MiscViewModel());
        }
        [HttpPost]
        public JsonResult SliderImage()
        {
            var data = new List<MiscViewModel>();
            DataTable dt = _miscService.usp_Misc(new MiscViewModel { Activity = "SLIDER" });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow rows in dt.Rows)
                {
                    if (Convert.ToString(rows["DOCNAME"]) != "")
                    {
                        byte[] bEmpImage = (byte[])(rows["DOCNAME"]);
                        data.Add(new MiscViewModel
                        {
                            DocName = "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length)
                        });
                    }
                    else
                    {
                        data.Add(new MiscViewModel
                        {
                            DocName = ""
                        });
                    }

                }
            }
            return Json(data);
        }
        public ActionResult SliderIndex()
        {
            int total = 0;
            var dt = Common.ConvertDataTableToList<MiscViewModel>(_miscService.usp_Misc(new MiscViewModel { Activity = "SLIDER_IMAGE_COUNT" }));
            if (dt != null || dt.Count > 0)
            {
                total = dt.Count;
            }
            ViewBag.Total = total;
            return View(new MiscViewModel());
        }
        public ActionResult AddImage()
        {
            var model = new MiscViewModel();
            DataTable dt = _miscService.usp_Misc(new MiscViewModel { Activity = "SLIDER" });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow rows in dt.Rows)
                {
                    if (Convert.ToString(rows["DOCNAME"]) != "")
                    {
                        byte[] bEmpImage = (byte[])(rows["DOCNAME"]);
                        model.ImageList.Add(new MiscViewModel
                        {
                            DocName = "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length),
                            Param1 = Convert.ToString(dt.Rows[0]["PARAM1"])
                        });
                    }
                    else
                    {
                        model.ImageList.Add(new MiscViewModel
                        {
                            DocName = "",
                            Param1 = Convert.ToString(dt.Rows[0]["PARAM1"])
                        });
                    }

                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddImage(MiscViewModel model, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                //// Convert Image to Bytes ////
                var fileName = Image;
                byte[] imageBytes = { };
                if (fileName != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileName.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                }

                DataTable dt = _miscService.usp_Misc(new MiscViewModel
                {
                    Activity = "ADDIMAGE",
                    BImage = imageBytes
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    int iFlag = Convert.ToInt32(dt.Rows[0]["FLAG"]);

                    if (iFlag > 0)
                    {
                        if (iFlag == 1)
                        {
                            TempData["Result"] = Common.ResultSuccess;
                            TempData["Message"] = Common.ResultUpdateMessage;
                        }
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                        return RedirectToAction(nameof(AddImage));
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(AddImage));


            //string date = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            //string user = User.Identity.GetADId();
            //if (files.Length > 0)
            //{
            //    var fileName = Path.GetFileName(files.FileName);
            //    var fileExtension = Path.GetExtension(fileName);

            //    string name = String.Concat("SDR_" + date, fileExtension);
            //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\doc\Slider", name);

            //    using FileStream fileStream = new(filePath, FileMode.Create);
            //    await files.CopyToAsync(fileStream);
            //    var dt = _miscService.usp_Misc(new MiscViewModel { Activity = "ADDIMAGE", DocName = name, UserID = user });
            //}
            //ViewBag.ImageList = Common.ConvertDataTableToList<MiscViewModel>(_miscService.usp_Misc(new MiscViewModel { Activity = "VIEWIMAGE" }));
            //return RedirectToAction(nameof(AddImage));
        }
        public JsonResult DeleteImage(string param1)
        {
            var dt = Common.ConvertDataTableToList<MiscViewModel>(_miscService.usp_Misc(new MiscViewModel { Activity = "DELETE_IMAGE", Param1 = param1 }));
            return Json("0");
        }
        #endregion

        public IActionResult AppUrl()
        {
            var model = new MiscViewModel()
            {
                RecordList = Common.ConvertDataTableToList<MiscViewModel>(_miscService.usp_Misc(new MiscViewModel() { Activity = "ECONNECT_URL", Param1 = User.Identity.GetDeptCode() }))
            };
            return View(model);
        }

    }
}
