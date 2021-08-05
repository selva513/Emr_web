using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Emr_web.Controllers
{
    public class FunctionalStatusController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IFunctionalRepo _functionalRepo;
        public FunctionalStatusController(IDBConnection iDBConnection,IFunctionalRepo functionalRepo)
        {
            _IDBConnection = iDBConnection;
            _functionalRepo = functionalRepo;
        }
        [HttpGet]
        public JsonResult CreateNewFunctionalSatus(string FunctionalStatus)
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            FunctionalStatusMaster statusMaster = new FunctionalStatusMaster
            {
               FunctionalStatus= FunctionalStatus,
               HospitalID=HospitalID,
               ModifieDatetime= timezoneUtility.Gettimezone(Timezoneid),
               CreateUser = logins[0].UserSeqid.ToString(),
               ModifieUser= logins[0].UserSeqid.ToString()
            };
            long AddResult = _functionalRepo.CreateNewFunctionalStatus(statusMaster);
            return Json(statusMaster);
        }
        [HttpGet]
        public JsonResult GetLatestPatientHistoryByPatientID(string PickListName)
        {
            List<FunPickListDetails> lstResult = new List<FunPickListDetails>();
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _functionalRepo.GetFunctionalStatusByPickListName(PickListName, HospitalID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                FunLstDtl = lstResult
            });
        }
        [HttpPost]
        public JsonResult SaveFunctionalPickList([FromBody] FunctionalStatusView functionalStatusView)
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            FunPickListHeader funPickListHeader = new FunPickListHeader
            {
                PickListName=functionalStatusView.PickListName,
                CreateUser=logins[0].Userid,
                HospitalID=HospitalID
            };
            bool isSuccess = _functionalRepo.AddNewFunPicLstHdr(funPickListHeader);
            if (isSuccess)
            {
                if (functionalStatusView.FunStatsuDtl.Length > 0)
                {
                    _functionalRepo.DeleteFunPickLstDtl(funPickListHeader.SeqID);
                    for(int count = 0; count < functionalStatusView.FunStatsuDtl.Length; count++)
                    {
                        FunPickListDetails funPickListDetails = new FunPickListDetails
                        {
                           HeaderSeqID= funPickListHeader.SeqID,
                           FuncationalStatus=functionalStatusView.FunStatsuDtl[count].FuncationalStatus
                        };
                        int Result = _functionalRepo.AddNewFuncationlDtl(funPickListDetails);
                    }
                }
            }
            return Json(functionalStatusView);
        }
    }
}