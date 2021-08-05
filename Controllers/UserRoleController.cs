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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserRoleController : Controller
    {
        private IDBConnection _IDBConnection;
        private IUserRoleRepo _userRoleRepo;
        private IErrorlog _errorlog;
        public UserRoleController(IDBConnection iDBConnection, IUserRoleRepo userRoleRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _userRoleRepo = userRoleRepo;
            _errorlog = errorlog;
        }
        public IActionResult UserRole()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
        [HttpPost]
        public JsonResult InsertRole([FromBody] RoleMasterView data)
        {
            int roleid = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    RoleMasterDetail roleMasterDetail = new RoleMasterDetail();
                    roleMasterDetail.Rolename = data.Rolename;
                    roleMasterDetail.HospitalID = data.HospitalID;
                    roleMasterDetail.ClinicID = data.ClinicID;
                    roleMasterDetail.AllowedDiscount = 0;
                    roleMasterDetail.IsDiscountallowed = null;
                    roleMasterDetail.CreatedByRoleId = 0;
                    roleMasterDetail.IsAdminRole = null;
                    roleMasterDetail.AdminRoleId = 0;
                    roleMasterDetail.MyPatientData = data.MyPatientData;
                    roleMasterDetail.AllPatientData = data.AllPatientData;
                    roleMasterDetail.AllDoctorData = data.AllDoctorData;
                    roleMasterDetail.DocumentUpload = data.DocumentUpload;
                    roleMasterDetail.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    roleMasterDetail.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    roleMasterDetail.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    roleMasterDetail.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    roleMasterDetail.IsActive = data.IsActive;
                    roleid = _userRoleRepo.CreateNewRole(roleMasterDetail);

                    for (int i = 0; i < data.Menugroup.Length; i++)
                    {
                        MenuGroupDetail menuGroupDetail = new MenuGroupDetail();
                        menuGroupDetail.G_Roleid = roleid;
                        menuGroupDetail.G_MainMenuid = data.Menugroup[i].G_MainMenuid;
                        menuGroupDetail.G_SubMenuid = data.Menugroup[i].G_SubMenuid;
                        menuGroupDetail.G_IsMainMenu = data.Menugroup[i].G_IsMainMenu;
                        menuGroupDetail.G_Access = data.Menugroup[i].G_Access;
                        menuGroupDetail.G_Add = data.Menugroup[i].G_Add;
                        menuGroupDetail.G_Edit = null;
                        menuGroupDetail.G_Delete = data.Menugroup[i].G_Delete;
                        menuGroupDetail.G_View = data.Menugroup[i].G_View;
                        menuGroupDetail.G_Verify = data.Menugroup[i].G_Verify;
                        menuGroupDetail.G_ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        menuGroupDetail.G_CreatedUser = HttpContext.Session.GetString("Userseqid");
                        menuGroupDetail.G_ModifiedUser = HttpContext.Session.GetString("Userseqid");
                        menuGroupDetail.G_IsActive = true;
                        if (menuGroupDetail.G_Access == false && menuGroupDetail.G_Add == false && menuGroupDetail.G_Delete == false && menuGroupDetail.G_View == false && menuGroupDetail.G_Verify == false)
                        {

                        }
                        else
                        {
                            bool isSuccess = _userRoleRepo.InsertMenuGroup(menuGroupDetail);
                        }
                            
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(roleid);
        }
        [HttpPost]
        public JsonResult InsertRoleandmenu([FromBody] MenuGroupView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    MenuGroupDetail menuGroupDetail = new MenuGroupDetail();
                    menuGroupDetail.G_Roleid = data.G_Roleid;
                    menuGroupDetail.G_MainMenuid = data.G_MainMenuid;
                    menuGroupDetail.G_SubMenuid = data.G_SubMenuid;
                    menuGroupDetail.G_IsMainMenu = data.G_IsMainMenu;
                    menuGroupDetail.G_Access = data.G_Access;
                    menuGroupDetail.G_Add = data.G_Add;
                    menuGroupDetail.G_Edit = null;
                    menuGroupDetail.G_Delete = data.G_Delete;
                    menuGroupDetail.G_View = data.G_View;
                    menuGroupDetail.G_Verify = data.G_Verify;
                    menuGroupDetail.G_ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    menuGroupDetail.G_CreatedUser = HttpContext.Session.GetString("Userseqid");
                    menuGroupDetail.G_ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    menuGroupDetail.G_IsActive = true;
                    if (menuGroupDetail.G_Access == false && menuGroupDetail.G_Add == false && menuGroupDetail.G_Delete == false && menuGroupDetail.G_View == false && menuGroupDetail.G_Verify == false)
                    {

                    }
                    else
                        isSuccess = _userRoleRepo.InsertMenuGroup(menuGroupDetail);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }
        [HttpPost]
        public JsonResult Update_Role([FromBody] RoleMasterView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    RoleMasterDetail roleMasterDetail = new RoleMasterDetail();
                    roleMasterDetail.Roleseqid = data.Roleseqid;
                    //roleMasterDetail.HospitalID = data.HospitalID;
                    //roleMasterDetail.ClinicID = data.ClinicID;
                    roleMasterDetail.AllowedDiscount = 0;
                    roleMasterDetail.IsDiscountallowed = null;
                    roleMasterDetail.CreatedByRoleId = 0;
                    roleMasterDetail.IsAdminRole = null;
                    roleMasterDetail.AdminRoleId = 0;
                    roleMasterDetail.MyPatientData = data.MyPatientData;
                    roleMasterDetail.AllPatientData = data.AllPatientData;
                    roleMasterDetail.AllDoctorData = data.AllDoctorData;
                    roleMasterDetail.DocumentUpload = data.DocumentUpload;
                    roleMasterDetail.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    roleMasterDetail.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    roleMasterDetail.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    roleMasterDetail.IsActive = data.IsActive;
                    isSuccess = _userRoleRepo.UpdateRole(roleMasterDetail);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }
        [HttpPost]
        public JsonResult UpdatemenuGroup([FromBody] MenuGroupView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    MenuGroupDetail menuGroupDetail = new MenuGroupDetail();
                    menuGroupDetail.G_Roleid = data.G_Roleid;
                    menuGroupDetail.G_MainMenuid = data.G_MainMenuid;
                    menuGroupDetail.G_SubMenuid = data.G_SubMenuid;
                    menuGroupDetail.G_IsMainMenu = data.G_IsMainMenu;
                    menuGroupDetail.G_Access = data.G_Access;
                    menuGroupDetail.G_Add = data.G_Add;
                    menuGroupDetail.G_Edit = null;
                    menuGroupDetail.G_Delete = data.G_Delete;
                    menuGroupDetail.G_View = data.G_View;
                    menuGroupDetail.G_Verify = data.G_Verify;
                    menuGroupDetail.G_ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    menuGroupDetail.G_CreatedUser = HttpContext.Session.GetString("Userseqid");
                    menuGroupDetail.G_ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    menuGroupDetail.G_IsActive = true;
                    long isexistrole = _userRoleRepo.CheckExistMenugroup(menuGroupDetail);
                    if (isexistrole == 0)
                    {
                        if (menuGroupDetail.G_Access == false && menuGroupDetail.G_Add == false && menuGroupDetail.G_Delete == false && menuGroupDetail.G_View == false && menuGroupDetail.G_Verify == false)
                        {

                        }
                        else
                            isSuccess = _userRoleRepo.InsertMenuGroup(menuGroupDetail);
                    }
                    else
                    {
                        isSuccess = _userRoleRepo.UpdateMenuGroup(menuGroupDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }
    }
}