using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BizLayer.Repo;
using BizLayer.Domain;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Navigations;
using Emr_web.Models;
using Emr_web.Common;
using System.Data;

namespace Emr_web.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IPatientDashboardRepo _patientdashboardrepo;
        private readonly IVitlasRepo _vitlasRepo;

        public PatientDashboardController(IDBConnection iDBConnection, IErrorlog errorlog, IPatientDashboardRepo patientdashboardrepo, IVitlasRepo vitlasRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _patientdashboardrepo = patientdashboardrepo;
            _vitlasRepo = vitlasRepo;
        }
        public IActionResult PatientDashboard()
        {
            List<PatientDashboard> list = new List<PatientDashboard>();
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;

            string HospitalID = HttpContext.Session.GetString("Hospitalid");
            list = _patientdashboardrepo.GetPatientByHospitalId(HospitalID);
            ViewBag.PatientList = list;
            return View();
        }

        [HttpGet]
        //public bool GetPatientDetailsById(string PatientId)
        public string GetPatientDetailsById(string PatientId)
        {
            //bool issuccess = false;
            //List<PatientDashboard> list = new List<PatientDashboard>();
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {
               //dt1 = _patientdashboardrepo.GetPatientDetailsById(PatientId);
               //dt2 = _patientdashboardrepo.GetPatientVitals(PatientId);

                ds.Tables.Add(dt1);
                ds.Tables.Add(dt2);
                //ViewBag.PatientDetails = list
                //issuccess = true;
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            //return issuccess;
            return ds.GetXml();
        }

        [HttpGet]
        public JsonResult MultiLineChartData(string PatientID)
        {
            Chart _chart = new Chart
            {
                labels = _vitlasRepo.GetVitalsDateByPatID(PatientID).ToArray(),
                datasets = new List<Datasets>()
            };
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "BP Systolic",
                data = _vitlasRepo.GetBPSystolicByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(255,99,132,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "BP Diastolic",
                data = _vitlasRepo.GetBPDiastolicByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(70,191,189,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Temperature",
                data = _vitlasRepo.GetTemperatureByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,180,92,1)" },
                borderWidth = "3"
            });
            _chart.datasets = _dataSet;
            return Json(_chart);
        }
    }
}