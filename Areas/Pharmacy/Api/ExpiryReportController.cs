using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/ExpiryReport")]
    [ApiController]
    public class ExpiryReportController : Controller
    {

        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IExpiryReportRepo _IExpiryReportRepo;

        public ExpiryReportController(IDBConnection dBConnection, IErrorlog errorlog, IExpiryReportRepo expiryReportRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _IExpiryReportRepo = expiryReportRepo;
        }
        [HttpGet("GetExpiryDateDetailsBySearch")]
        public List<CurrentStockInfo> GetExpiryDateDetailsBySearch(int Search)
        {
            List<CurrentStockInfo> lstResult = new List<CurrentStockInfo>();

            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));

                lstResult = _IExpiryReportRepo.GetExpiryDateDetailsBySearch(Search, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }



    }

}
