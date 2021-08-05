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
    public class DepartmentMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IDepartmentRepo _departmentRepo;
        private IErrorlog _errorlog;
        private readonly IEmrRepo _emrRepo;
        public DepartmentMasterController(IDBConnection iDBConnection, IDepartmentRepo departmentRepo, IErrorlog errorlog, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _departmentRepo = departmentRepo;
            _errorlog = errorlog;
            _emrRepo = emrRepo;
        }
        public async Task<IActionResult> DepartmentMaster()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lst = await _departmentRepo.GetAllDepartment();
                if (lst != null)
                {
                    lst = lst as List<DepartmentMaster>;
                    ViewBag.DepartmentDetail = lst;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult InsertDepartment([Bind]DepartmentView model)
        {
            int result = 0;
            try
            {
                if (model.DeptSeqID == 0)
                    result = InsertNewDepartment(model, true);
                else
                    result = InsertNewDepartment(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("DepartmentMaster", "DepartmentMaster");
        }
        private int InsertNewDepartment(DepartmentView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                DepartmentMaster departmentMaster  = new DepartmentMaster();
                departmentMaster.DepartmentName = model.DepartmentName;
                departmentMaster.HospitalID = HttpContext.Session.GetString("Hospitalid");
                departmentMaster.CreatedDate = timezoneUtility.Gettimezone(Timezoneid);
                departmentMaster.ModifyDate = timezoneUtility.Gettimezone(Timezoneid);
                departmentMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                departmentMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                departmentMaster.Isactive = model.Isactive;
                if (validation == true)
                {
                    result = _departmentRepo.CreateNewDepartment(departmentMaster);
                    string EventName = "New Department Added-" + model.DepartmentName;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    departmentMaster.DeptSeqID = model.DeptSeqID;
                    result = _departmentRepo.UpdateDepartment(departmentMaster);
                    string EventName = "Update Department Master-" + model.DepartmentName;
                    CreateEventManagemnt(EventName);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpPost]
        public JsonResult InsertNewDept([FromBody] string DeptName)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if(DeptName!=null)
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        DepartmentMaster departmentMaster = new DepartmentMaster();
                        departmentMaster.DepartmentName = DeptName;
                        departmentMaster.HospitalID = HttpContext.Session.GetString("Hospitalid");
                        departmentMaster.CreatedDate = timezoneUtility.Gettimezone(Timezoneid);
                        departmentMaster.ModifyDate = timezoneUtility.Gettimezone(Timezoneid);
                        departmentMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        departmentMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                        departmentMaster.Isactive = true;
                        int result = _departmentRepo.CreateNewDepartment(departmentMaster);
                        if (result > 0)
                            isSuccess = true;
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
        private void CreateEventManagemnt(string EventName)
        {
            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo
            {
                EventName = EventName,
                UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            _emrRepo.NewEventCreateion(eventManagemntInfo);

        }
    }
}