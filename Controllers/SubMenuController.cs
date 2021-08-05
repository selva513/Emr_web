using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Common;
using Emr_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.Authorization;
namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SubMenuController : Controller
    {
        private IDBConnection _IDBConnection;
        private ISubMenuRepo _subMenuRepo;
        private IErrorlog _errorlog;
        public SubMenuController(IDBConnection iDBConnection, ISubMenuRepo subMenuRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _subMenuRepo = subMenuRepo;
            _errorlog = errorlog;
        }
        public async Task<IActionResult> SubMenu()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                GetAllHospital();
                var lst = await _subMenuRepo.GetAllSubMenu();
                if (lst != null)
                {
                    lst = lst as List<SubMenuDetail>;
                    ViewBag.Submenudetail = lst;
                }
            }
            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        private void GetAllHospital()
        {
            try
            {
                List<MainMenuDetail> list = _subMenuRepo.GetMainmenuList();
                ViewBag.MainMenu = list;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertSubMenu([Bind]SubMenuView model)
        {
            int result = 0;
            try
            {
                if (model.S_Menu_id == 0)
                    result = InsertNewSubMenu(model, true);
                else
                    result = InsertNewSubMenu(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("SubMenu", "SubMenu");
        }
        private int InsertNewSubMenu(SubMenuView model, bool validation)
        {
            int result = 0;
            try
            {
                SubMenuDetail subMenuDetail = new SubMenuDetail();
                subMenuDetail.S_MenuName = model.S_MenuName;
                subMenuDetail.S_MainMenu_id = model.S_MainMenu_id;
                subMenuDetail.S_Access = null;
                subMenuDetail.S_Add = null;
                subMenuDetail.S_Modify = null;
                subMenuDetail.S_Delete = null;
                subMenuDetail.S_View = null;
                subMenuDetail.S_Seq = _subMenuRepo.GetMaxSeqid(subMenuDetail.S_MainMenu_id) + 1;
                subMenuDetail.S_Menu_Link = model.S_Menu_Link;
                subMenuDetail.S_Class = null;
                subMenuDetail.S_ModifiedDatetime= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                subMenuDetail.S_CreatedUser = "";
                subMenuDetail.S_ModifiedUser = "";
                subMenuDetail.S_IsActive = model.S_IsActive;
                if (validation == true)
                    result = _subMenuRepo.CreateNewSubMenu(subMenuDetail);
                else
                {
                    subMenuDetail.S_Menu_id = model.S_Menu_id;
                    result = _subMenuRepo.UpdateSubMenu(subMenuDetail);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
    }
}