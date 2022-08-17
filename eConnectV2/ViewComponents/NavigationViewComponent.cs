using eConnectV2.Helpers;
using eConnectV2.Models;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using eConnectV2.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace eConnectV2.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ICommonService _commonService;
        private readonly HttpContext _hcontext;
        public NavigationViewComponent(IHttpContextAccessor httpContextAccessor, ICommonService commonService)
        {
            _hcontext = httpContextAccessor.HttpContext;
            _commonService = commonService;
        }
        public IViewComponentResult Invoke()
        {
            var navModel = new NavigationModel();
            var navItemList = new List<ListItem>();
            var navItem = new ListItem();

            DataTable dtMenu = _commonService.usp_LogIn(new LoginViewModel() { Activity = "FILL_MENU", UserId = _hcontext.User.Identity.GetADId() });
            if (dtMenu != null && dtMenu.Rows.Count > 0)
            {
                foreach (DataRow menuRow in dtMenu.Rows)
                {
                    string strMenuId = Convert.ToString(menuRow["MENUID"]);
                    navItem.Title = Convert.ToString(menuRow["MENUNAME"]);
                    navItem.Icon = Convert.ToString(menuRow["MENUICON"]);
                    string strHasChild = Convert.ToString(menuRow["HASCHILD"]);
                    if (strHasChild == "0")
                    {
                        navItem.Href = Convert.ToString(menuRow["URL"]);
                    }
                    else
                    {
                        navItem.Items.AddRange(BindSubMenu(strMenuId));
                    }
                    navItemList.Add(navItem);
                    navItem = new ListItem();
                }
            }
            return View(navModel.Seed(navItemList));

            //var _dtMenus = _commonService.usp_LogIn(new LoginViewModel() { Activity = "GET_ACTIVE_MENUS" });
            //var _subMenuslist = Common.ConvertDataTableToList<AdminViewModel>(_commonService.usp_LogIn(new LoginViewModel() { Activity = "GET_ACTIVE_SUBMENUS" }));
            //var _menuLink = Common.ConvertDataTableToList<AdminViewModel>(_commonService.usp_LogIn(new LoginViewModel() { Activity = "FILL_MENU_BY_ADID", UserId = _hcontext.User.Identity.GetADId() }));

            //foreach (DataRow row in _dtMenus.Rows)
            //{
            //    int menuId = Convert.ToInt32(row["MENUID"]);
            //    bool exists = _menuLink.Any(x => x.MenuId == menuId);
            //    if (exists)
            //    {
            //        navItem.Title = row["MENUNAME"].ToString();
            //        navItem.Icon = row["MENUICON"].ToString();
            //        var subMenuId = row["HASCHILD"].ToString();
            //        if (subMenuId == "0")
            //        {
            //            navItem.Href = row["URL"].ToString();
            //        }
            //        else
            //        {
            //            var subitemlist = _subMenuslist.Where(x => x.MenuId == Convert.ToInt32(row["MENUID"])).ToList();
            //            navItem.Items.AddRange(BindSubMenuItem(subitemlist, _subMenuslist, _menuLink, menuId));
            //        }
            //        navItemList.Add(navItem);
            //        navItem = new ListItem();
            //    }
            //}
            //return View(navModel.Seed(navItemList));
        }

        private IEnumerable<ListItem> BindSubMenu(string strMenuId)
        {
            var navItemList = new List<ListItem>();
            DataTable dtSubMenu = _commonService.usp_LogIn(new LoginViewModel() { Activity = "FILL_SUBMENU", UserId = _hcontext.User.Identity.GetADId(), Param1 = strMenuId });
            if (dtSubMenu != null && dtSubMenu.Rows.Count > 0)
            {
                foreach (DataRow smRow in dtSubMenu.Rows)
                {
                    navItemList.Add(new ListItem()
                    {
                        Title = Convert.ToString(smRow["SUBMENUNAME"]),
                        Href = Convert.ToString(smRow["URL"])
                    });
                }
            }
            return navItemList;
        }



        //private IEnumerable<ListItem> BindSubMenuItem(List<AdminViewModel> subitemlist, List<AdminViewModel> subMenuslist, List<AdminViewModel> _menuLink, int menuId)
        //{
        //    var navItemList = new List<ListItem>();
        //    foreach (var item in subitemlist)
        //    {
        //        bool exists = _menuLink.Any(x => x.MenuId == menuId && x.SubMenuId == item.SubMenuId);
        //        if (exists)
        //        {
        //            var subitemdetails = subMenuslist.Where(x => x.SubMenuId == item.SubMenuId).FirstOrDefault();
        //            navItemList.Add(new ListItem()
        //            {
        //                Title = subitemdetails.SubMenuName,
        //                Href = subitemdetails.Url
        //            });
        //        }
        //    }
        //    return navItemList;
        //}
    }
}
