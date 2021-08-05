using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Repo;
using Emr_web.Models;
using Emr_web.Common;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.Http;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ServiceMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private IServiceRepo _servicerepo;

        public ServiceMasterController(IDBConnection iDBConnection, IErrorlog errorlog, IServiceRepo servicerepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _servicerepo = servicerepo;
        }
        public IActionResult ServiceMaster()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            GetServiceByHospitalID();
            string ser = HttpContext.Session.GetString("Serviceid");
            int ServiceId = Convert.ToInt32(HttpContext.Session.GetString("Serviceid"));
            //if(ServiceId == 0)
            //{
                GetTestAsList(ServiceId);
                ServiceId = 0;
                HttpContext.Session.SetString("Serviceid", ServiceId.ToString());
            //}
            return View();
        }

        private void GetServiceByHospitalID()
        {
            List<ServiceMaster> list = new List<ServiceMaster>();
            ServiceMaster serviceMaster = new ServiceMaster();
            try
            {
                int HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                list = _servicerepo.GetAllService();
                serviceMaster.ServiceId = 0;
                serviceMaster.ServiceName = "All";
                list.Add(serviceMaster);
                list = list.OrderBy(x => x.ServiceId).ToList();
                ViewBag.Service = list;
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            
        }

        [HttpGet]
        public bool OnServiceChange(int ServiceId)
        {
            HttpContext.Session.SetString("Serviceid", ServiceId.ToString());

            return true;
        }

        [HttpGet]
        public bool GetTestAsList(int ServiceId)
        {
            bool issucess = false;
            int HospitalId;
            DataSet ds = new DataSet();
            List<ServiceTestMaster> list = new List<ServiceTestMaster>();
            try
            {
                HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                HttpContext.Session.SetString("Serviceid", ServiceId.ToString());

                if (ServiceId != 0)
                {
                    list = _servicerepo.GetTestListByServiceId(HospitalId, ServiceId);
                    ViewBag.TestByService = list;
                    issucess = true;
                }
                else
                {
                    list = _servicerepo.GetAllTestByHospitalId(HospitalId);
                    ViewBag.TestByService = list;
                    issucess = true;
                }
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            return issucess;
        }

        [HttpPost]
        public bool SaveService([FromBody] ServiceModel model)
        {
            bool issuccess = false;
            ServiceMaster servicemaster = new ServiceMaster();
            string HospitalID;
            int MaxServiceId;
            try
            {
                HospitalID = HttpContext.Session.GetString("Hospitalid");
                MaxServiceId = _servicerepo.GetMaxServiceId(HospitalID);

                servicemaster.ServiceId = MaxServiceId + 1;
                servicemaster.ServiceName = model.ServiceName;
                servicemaster.ServiceShortCode = model.ServiceShortCode;
                servicemaster.IsActive = model.IsActive;
                servicemaster.HospitalID = HttpContext.Session.GetString("Hospitalid");
                _servicerepo.SaveService(servicemaster);
                issuccess = true;
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            
            return issuccess;
        }

        [HttpPost]
        public bool SaveTestByService([FromBody] ServiceModel model)
        {
            ServiceTestMaster testmaster = new ServiceTestMaster();
            bool issuccess = false;
            int MaxTestId;
            string HospitalId;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                HospitalId = HttpContext.Session.GetString("Hospitalid");
                MaxTestId = _servicerepo.GetMaxTestId(HospitalId);

                testmaster.ServiceId = model.ServiceId;
                testmaster.ServiceName = model.ServiceName;
                testmaster.TestId = MaxTestId + 1;
                testmaster.TestName = model.TestName;
                testmaster.TestShortCode = model.TestShortCode;
                testmaster.TestAliasNames = model.TestAlias;
                testmaster.TestRateCommon = model.TestRate;
                testmaster.TestActive = model.TestIsActive;
                testmaster.QTY_Enable = model.EnableQty;
                testmaster.RATE_Enable = model.EnableRate;
                testmaster.CreatedDateTime = timezoneUtility.Gettimezone(Timezoneid);
                testmaster.HospitalID = HttpContext.Session.GetString("Hospitalid");

                _servicerepo.SaveTestByService(testmaster);
                _servicerepo.InsertTestRate(testmaster);
                _servicerepo.UpdateHospitalMaster(testmaster.TestId,testmaster.HospitalID);
                issuccess = true;
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return issuccess;
        }

        [HttpPost]
        public bool UpdateTestByService([FromBody] ServiceModel model)
        {
            ServiceTestMaster testmaster = new ServiceTestMaster();
            bool issucess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                testmaster.ServiceId = model.ServiceId;
                testmaster.ServiceName = model.ServiceName;
                testmaster.TestId = model.TestId;
                testmaster.TestName = model.TestName;
                testmaster.TestShortCode = model.TestShortCode;
                testmaster.TestAliasNames = model.TestAlias;
                testmaster.TestRateCommon = model.TestRate;
                testmaster.TestActive = model.TestIsActive;
                testmaster.QTY_Enable = model.EnableQty;
                testmaster.RATE_Enable = model.EnableRate;
                testmaster.CreatedDateTime = timezoneUtility.Gettimezone(Timezoneid);
                testmaster.HospitalID = HttpContext.Session.GetString("Hospitalid");

                _servicerepo.UpdateTestByService(testmaster);
                _servicerepo.UpdateTestRate(testmaster);
                issucess = true;
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return issucess;
        }

        [HttpPost]
        public bool Check_ServiceExist(string ServiceName)
        {
            bool issuccess = false;
            try
            {
                string HospitalId = HttpContext.Session.GetString("Hospitalid");
                issuccess = _servicerepo.CheckService_Exist(ServiceName,HospitalId);
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            return issuccess;
        }
    }
}