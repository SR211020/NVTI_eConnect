using eConnectV2.Extensions;
using eConnectV2.Filters;
using eConnectV2.Helpers;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace eConnectV2.Controllers
{
    [Authorize]
    //[ServiceFilter(typeof(ActionFilterConfig))]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ICommonService _commonService;
        public AdminController(IAdminService adminService, ICommonService commonService)
        {
            _adminService = adminService;
            _commonService = commonService;
        }

        #region Menu
        public IActionResult Menu()
        {
            var data = new AdminViewModel()
            {
                Status = 1,
                HasChild = 1,
                MenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult Menu(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_MENU",
                    Param1 = model.MenuName,
                    Param2 = model.MenuIcon,
                    Param3 = model.HasChild.ToString(),
                    Param4 = model.Status.ToString(),
                    Param5 = model.Url,
                    Param6 = model.MenuOrder.ToString(),
                    Param7 = User.Identity.GetADId().ToString(),
                    Param8 = model.MenuId.ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.MenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_LIST" }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(Menu));
        }
        public IActionResult DeleteMenu(int id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_MENU", Param1 = id.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult IsMenuInUsed(int id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "IS_MENU_INUSE", Param1 = id.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = false, responseMsg = Common.RecordAlreadyInUse });
            }
            else
            {
                return Json(new { success = true, responseMsg = "" });

            }
        }
        public JsonResult BindMenuDetail(int id)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_DETAIL", Param1 = id.ToString() })) });
        }
        public JsonResult MenuUpdate(string menuId, string menuName, string menuIcon, string status, string hasChild, string url, string menuOrder)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_MENU",
                Param1 = menuName,
                Param2 = menuIcon,
                Param3 = status,
                Param4 = hasChild,
                Param5 = url,
                Param6 = menuOrder,
                Param7 = User.Identity.GetADId().ToString(),
                Param8 = menuId
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region Manage SubMenu
        [DecryptQueryStringParameter]
        public IActionResult SubMenu(string id)
        {
            int menuId = Convert.ToInt32(id);
            if (menuId > 0)
            {
                var data = new AdminViewModel()
                {
                    Status = 1,
                    MenuId = menuId,
                    MenuName = _adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_DETAIL", Param1 = id }).Rows[0]["MENUNAME"].ToString(),
                    SubMenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_SUBMENU_LIST", Param1 = id }))
                };
                return View(data);
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SubMenuDirectAccessNotAllowed;
                return RedirectToAction(nameof(Menu));
            }
        }
        [HttpPost]
        public IActionResult SubMenu(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_SUBMENU",
                    Param1 = model.MenuId.ToString(),
                    Param2 = model.SubMenuName,
                    Param3 = model.Url,
                    Param4 = model.Status.ToString(),
                    Param5 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.SubMenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_SUBMENU_LIST", Param1 = model.MenuId.ToString() }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            var dataString = CustomQueryStringHelper.EncryptString(null, nameof(SubMenu), "Admin", new { id = model.MenuId.ToString() });
            return LocalRedirect(dataString);
        }
        public IActionResult DeleteSubMenu(int id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_SUBMENU", Param1 = id.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult IsSubMenuInUsed(int id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "IS_SUBMENU_INUSE", Param1 = id.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = false, responseMsg = Common.RecordAlreadyInUse });
            }
            else
            {
                return Json(new { success = true, responseMsg = "" });
            }
        }
        public JsonResult BindSubMenuDetail(int id)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_SUBMENU_DETAIL", Param1 = id.ToString() })) });
        }
        public JsonResult SubMenuUpdate(string menuId, string subMenuName, string subMenuId, string status, string url)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_SUBMENU",
                Param1 = subMenuName,
                Param2 = subMenuId,
                Param3 = status,
                Param4 = url,
                Param5 = User.Identity.GetADId().ToString(),
                Param6 = menuId
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { result = Common.ResultSuccess, msg = Common.ResultUpdateMessage });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region Manage Department
        public IActionResult Department()
        {
            var data = new AdminViewModel()
            {
                Status = 1,
                DeptList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_DEPT_LIST" })),
                DDLEmpList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_BY_AD_EMPCODE" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult Department(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_DEPT",
                    Param1 = model.DeptCode,
                    Param2 = model.DeptName,
                    Param3 = model.HodADID,
                    Param4 = model.Status.ToString(),
                    Param5 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.DeptList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_DEPT_LIST" }));
                        model.DDLEmpList = Common.BindDropDown(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_DDL_EMP_LIST" }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(Department));
        }
        public IActionResult DeleteDept(string id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_DEPT", Param1 = id });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult BindDeptDetail(string id)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_DEPT_DETAIL", Param1 = id })) });
        }
        public JsonResult DeptUpdate(string deptCode, string deptName, string hodADID, string status, string id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_DEPT",
                Param1 = deptCode,
                Param2 = deptName,
                Param3 = hodADID,
                Param4 = status,
                Param5 = User.Identity.GetADId().ToString(),
                Param6 = id
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region Manage Code Type
        public IActionResult CodeType()
        {
            var data = new AdminViewModel()
            {
                Status = 1,
                CodeList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult CodeType(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_CODE",
                    Param1 = model.Code.ToString(),
                    Param2 = model.CodeDesc,
                    Param3 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.CodeList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_LIST" }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(CodeType));
        }
        public IActionResult DeleteCode(string id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_CODE", Param1 = id });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult BindCodeDetail(string id)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_DETAIL", Param1 = id })) });
        }
        public JsonResult CodeUpdate(string code, string codeDesc)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_CODE",
                Param1 = code,
                Param2 = codeDesc,
                Param3 = User.Identity.GetADId().ToString()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region Manage CodeDef
        [DecryptQueryStringParameter]
        public IActionResult CodeDef(string id)
        {
            int code = Convert.ToInt32(id);
            if (code > 0)
            {
                var codeDetail = _adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_DETAIL", Param1 = id });
                var data = new AdminViewModel()
                {
                    Status = 1,
                    Code = code,
                    CodeDesc = codeDetail.Rows[0]["CODEDESC"].ToString(),
                    CodeDefList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_DEF_LIST", Param1 = id }))
                };
                return View(data);
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.CodeDefDirectAccessNotAllowed;
                return RedirectToAction(nameof(CodeType));
            }
        }
        [HttpPost]
        public IActionResult CodeDef(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_CODE_DEF",
                    Param1 = model.Code.ToString(),
                    Param2 = model.CodeDefKeyId.ToString(),
                    Param3 = model.CodeDefDesc,
                    Param4 = model.CodeDefValue,
                    Param5 = model.Status.ToString(),
                    Param6 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.CodeDefList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_DEF_LIST", Param1 = model.Code.ToString() }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            var dataString = CustomQueryStringHelper.EncryptString(null, nameof(CodeDef), "Admin", new { id = model.Code.ToString() });
            return LocalRedirect(dataString);
        }
        public IActionResult DeleteCodeDef(int code, int keyId)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_CODE_DEF", Param1 = code.ToString(), Param2 = keyId.ToString() });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult BindCodeDefDetail(int code, int keyId)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CODE_DEF_DETAIL", Param1 = code.ToString(), Param2 = keyId.ToString() })) });
        }
        public JsonResult CodeDefUpdate(string code, string keyId, string descr, string value, string status)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_CODE_DEF",
                Param1 = code,
                Param2 = keyId,
                Param3 = descr,
                Param4 = value,
                Param5 = status,
                Param6 = User.Identity.GetADId().ToString()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { result = Common.ResultSuccess, msg = Common.ResultUpdateMessage });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region Manage Config
        public IActionResult Config()
        {
            var data = new AdminViewModel()
            {
                ConfigList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONFIG_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult Config(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_CONFIG",
                    Param1 = model.Module,
                    Param2 = model.Category1,
                    Param3 = model.Category2,
                    Param4 = model.ConfigValue,
                    Param5 = model.Lvl.ToString(),
                    Param6 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.ConfigList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONFIG_LIST" }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(Config));
        }
        public IActionResult DeleteConfig(string id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_CONFIG", Param1 = id });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult BindConfigDetail(string id)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONFIG_DETAIL", Param1 = id })) });
        }
        public JsonResult ConfigUpdate(string id, string module, string category1, string category2, string val, string lvl)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_CONFIG",
                Param1 = id,
                Param2 = module,
                Param3 = category1,
                Param4 = category2,
                Param5 = val,
                Param6 = lvl,
                Param7 = User.Identity.GetADId().ToString()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region View Employee
        public IActionResult Employee()
        {
            var data = new AdminViewModel()
            {
                EmpList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_EMP_LIST" }))
            };
            return View(data);
        }
        #endregion

        #region Manage Contacts
        public IActionResult Contact()
        {
            var data = new AdminViewModel()
            {
                ContactList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONTACT_LIST" }))
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult Contact(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_CONTACT",
                    Param1 = model.EmpName,
                    Param2 = model.DeptName,
                    Param3 = model.Contact,
                    Param4 = model.EmpEmail,
                    Param5 = model.Plant,
                    Param6 = User.Identity.GetADId().ToString()
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    var flag = Convert.ToInt32(dt.Rows[0]["FLAG"]);
                    if (flag == 1)
                    {
                        TempData["Result"] = Common.ResultSuccess;
                        TempData["Message"] = Common.ResultSuccessMessage;
                    }
                    else if (flag == 3)
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.AlreadyExist;
                        model.ConfigList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONTACT_LIST" }));
                        return View(model);
                    }
                    else
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = Common.SomethingWentWrong;
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(Contact));
        }
        public IActionResult DeleteContact(string id)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_CONTACT", Param1 = id });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                return Json(new { success = true, responseMsg = Common.RecordDeletedSuccess });
            }
            else
            {
                return Json(new { success = false, responseMsg = Common.SomethingWentWrong });
            }
        }
        public JsonResult BindContactDetail(string srNo)
        {
            return Json(new { result = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_CONTACT_DETAIL", Param1 = srNo })) });
        }
        public JsonResult ContactUpdate(string srNo, string empName, string deptName, string contact, string empEmail, string plant)
        {
            var dt = _adminService.usp_Admin(new AdminViewModel()
            {
                Activity = "UPDATE_CONTACT",
                Param1 = srNo,
                Param2 = empName,
                Param3 = deptName,
                Param4 = contact,
                Param5 = empEmail,
                Param6 = plant,
                Param7 = User.Identity.GetADId().ToString()
            });
            if (dt.Rows[0]["FLAG"].ToString() == "1")
            {
                TempData["Result"] = Common.ResultSuccess;
                TempData["Message"] = Common.ResultUpdateMessage;
                return Json(new { result = Common.ResultSuccess, msg = "" });
            }
            else if (dt.Rows[0]["FLAG"].ToString() == "2")
            {
                return Json(new { result = Common.ResultError, msg = Common.AlreadyExist });
            }
            return Json(new { result = Common.ResultError, msg = Common.SomethingWentWrong });
        }
        #endregion

        #region MenuPermission
        public IActionResult MenuPermission()
        {
            var data = new AdminViewModel()
            {
                DDLEmpList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_EMP_BY_AD_EMPCODE1" })),
                MenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_LIST" })).Where(x => x.Status == 1 && x.MenuId < 1000).ToList(),
                SubMenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_ALL_SUBMENU_LIST" })),
            };
            foreach (var item in data.MenuList)
            {
                item.SubMenuList = data.SubMenuList.Where(a => a.MenuId == item.MenuId).OrderBy(x => x.SubMenuName).ToList();
            }
            return View(data);
        }
        public JsonResult GetMenuMapping(string empAdId)
        {
            var data = new AdminViewModel()
            {
                MenuList = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENUACCESS_LIST", Param1 = empAdId }))
            };
            return Json(data);
        }
        public JsonResult UpdateMenuMapping(int[] Submenu, int[] Allmenu, int[] MenuIDs, string EmpAdId)
        {
            Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "DELETE_PREV_MENUACCESS", Param1 = EmpAdId }));
            int i = 0;
            foreach (var submenuid in Submenu)
            {
                _adminService.usp_Admin(new AdminViewModel()
                {
                    Activity = "ADD_MENUACCESS",
                    Param1 = Allmenu[i].ToString(),
                    Param2 = submenuid.ToString(),
                    Param3 = EmpAdId,
                    Param4 = User.Identity.GetADId()
                });
                i += 1;
            }
            foreach (var menuId in MenuIDs)
            {
                var isExists = Allmenu.Contains(menuId);
                if (!isExists)
                {
                    var subMenuExistsInMenu = Common.ConvertDataTableToList<AdminViewModel>(_adminService.usp_Admin(new AdminViewModel() { Activity = "GET_MENU_DETAIL", Param1 = menuId.ToString() }));
                    if (subMenuExistsInMenu != null)
                    {
                        if (subMenuExistsInMenu.FirstOrDefault().HasChild == 0)
                        {
                            _adminService.usp_Admin(new AdminViewModel()
                            {
                                Activity = "ADD_MENUACCESS",
                                Param1 = menuId.ToString(),
                                Param2 = "0",
                                Param3 = EmpAdId,
                                Param4 = User.Identity.GetADId()
                            });
                        }
                    }
                }
            }
            return Json("Success");
        }
        #endregion

        #region Profile
        public IActionResult Profile()
        {
            var model = new AdminViewModel();
            model.PlantList = Common.BindDropDown(_commonService.usp_Master(new CommonViewModel() { Activity = "FILL_OPTION_DESCR", DocNo = "2" }));
            DataTable dt = _adminService.usp_Admin(new AdminViewModel { Activity = "GET_PROFILE_DATA", UserId = User.Identity.GetADId() });
            if (dt != null && dt.Rows.Count > 0)
            {
                model.EmpAdId = Convert.ToString(dt.Rows[0]["ADID"]);
                model.EmpCode = Convert.ToString(dt.Rows[0]["EMPCODE"]);
                model.EmpName = Convert.ToString(dt.Rows[0]["EMPNAME"]);
                model.Plant = Convert.ToString(dt.Rows[0]["PLANT"]);
                model.DeptName = Convert.ToString(dt.Rows[0]["DEPTNAME"]);
                model.Contact = Convert.ToString(dt.Rows[0]["CONTACTNO_O"]);
                model.ExtNo = Convert.ToString(dt.Rows[0]["LANDLINE"]);
                model.EmpEmail = Convert.ToString(dt.Rows[0]["EMAILID_O"]);
                if (Convert.ToString(dt.Rows[0]["PHOTO"]) != "")
                {
                    byte[] bEmpImage = (byte[])(dt.Rows[0]["PHOTO"]);
                    model.ImageName = "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length);
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Profile(AdminViewModel model, IFormFile FuplImage)
        {
            if (ModelState.IsValid)
            {
                if (model.EmpEmail != null)
                {
                    if (!model.EmpEmail.Contains("@"))
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = "Please Enter valid Email ID";
                        return RedirectToAction(nameof(Profile));
                    }
                    if (model.EmpEmail.Split("@")[1] != "nvtpower.com")
                    {
                        TempData["Result"] = Common.ResultError;
                        TempData["Message"] = "Please Enter Company Email ID";
                        return RedirectToAction(nameof(Profile));
                    }
                }
                //// Convert Image to Bytes ////
                var fileName = FuplImage;
                byte[] imageBytes = { };
                if (fileName != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileName.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                }

                DataTable dt = _adminService.usp_Admin(new AdminViewModel
                {
                    Activity = "UPDATE_PROFILE_DATA",
                    UserId = User.Identity.GetADId(),
                    Param1 = model.EmpEmail,
                    Param2 = model.Contact,
                    Param3 = model.ExtNo,
                    Param4 = model.Plant,
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
                        return RedirectToAction(nameof(Profile));
                    }
                }
            }
            else
            {
                TempData["Result"] = Common.ResultError;
                TempData["Message"] = Common.SomethingWentWrong;
            }
            return RedirectToAction(nameof(Profile));
        }

        public JsonResult GetProfileImage()
        {
            var data = new List<AdminViewModel>();
            DataTable dt = _adminService.usp_Admin(new AdminViewModel { Activity = "GET_PROFILE_DATA", UserId = User.Identity.GetADId() });
            if (dt != null && dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0]["PHOTO"]) != "")
                {
                    byte[] bEmpImage = (byte[])(dt.Rows[0]["PHOTO"]);
                    data.Add(new AdminViewModel()
                    {
                        ImageName = "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length)
                    });
                }
                else
                {
                    data.Add(new AdminViewModel()
                    {
                        ImageName = ""
                    });
                }
            }

            return Json(data);
        }
        #endregion
    }
}
