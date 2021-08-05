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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MainMenuController : Controller
    {
        private IDBConnection _IDBConnection;
        private IMainMenuRepo _mainMenuRepo;
        private IErrorlog _errorlog;
        public MainMenuController(IDBConnection iDBConnection, IMainMenuRepo mainMenuRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _mainMenuRepo = mainMenuRepo;
            _errorlog = errorlog;
        }
        public async Task<IActionResult> MainMenu()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lst = await _mainMenuRepo.GetAllMainMenu();
                if (lst != null)
                {
                    lst = lst as List<MainMenuDetail>;
                    ViewBag.Mainmenudetail = lst;
                }
            }
            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertMainMenu([Bind]MainMenuView model)
        {
            int result = 0;
            try
            {
                if (model.M_Menu_id == 0)
                    result = InsertNewMainMenu(model,true);
                else
                    result = InsertNewMainMenu(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("MainMenu", "MainMenu");
        }
        private int InsertNewMainMenu(MainMenuView model,bool validation)
        {
            int result = 0;
            try
            {
                MainMenuDetail mainMenuDetail = new MainMenuDetail();
                mainMenuDetail.M_Menuname = model.M_Menuname;
                mainMenuDetail.M_Seq = _mainMenuRepo.GetMaxSeqid() + 1;
                mainMenuDetail.M_Class = null;
                mainMenuDetail.M_Icon = null;
                mainMenuDetail.M_Href = null;
                mainMenuDetail.M_ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mainMenuDetail.M_CreatedUser = "";
                mainMenuDetail.M_ModifiedUser = "";
                mainMenuDetail.M_IsActive = model.M_IsActive;
                if(validation==true)
                    result = _mainMenuRepo.CreateNewMenu(mainMenuDetail);
                else
                {
                    mainMenuDetail.M_Menu_id = model.M_Menu_id;
                    result = _mainMenuRepo.UpdateMenu(mainMenuDetail);
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