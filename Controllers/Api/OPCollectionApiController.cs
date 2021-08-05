using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using System.Text.RegularExpressions;
using System.Data;
using Emr_web.Common;
using System.Globalization;

namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OPCollectionApiController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IOPCollectionRepo _oPCollectionRepo;
        public OPCollectionApiController(IDBConnection dBConnection, IErrorlog errorlog, IOPCollectionRepo oPCollectionRepo)
        {
            _IDBConnection = dBConnection;
            _errorlog = errorlog;
            _oPCollectionRepo = oPCollectionRepo;
        }
        [HttpGet("GetOPCollectionReport")]
        public string GetOPCollectionReport(string fromdate, string todate)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {
                string FromDate = "";
                string ToDate = "";
                if (fromdate != "")
                {
                    DateTime dtFrom = DateTime.Now;
                    FromDate = getDataformat(fromdate, dtFrom).ToString("yyyy-MM-dd 00:00:00");
                }
                if (todate != "")
                {
                    DateTime dtTodate = DateTime.Now;
                    ToDate = getDataformat(todate, dtTodate).ToString("yyyy-MM-dd 23:59:59");
                }
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                dt = _oPCollectionRepo.Get_AllPatient_CollectionReport(FromDate, ToDate, HospitalId);
                dt1 = _oPCollectionRepo.Get_AllPatient_DueCollectionReport(FromDate, ToDate, HospitalId);
                dt2 = _oPCollectionRepo.Get_AllPatient_RefundCollectionReport(FromDate, ToDate, HospitalId);
                dt2.TableName = "dtcollectionRefund";
                dt1.TableName = "dtcollectionallDue";
                dt.TableName = "dtcollectionall";
                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return ds.GetXml();
        }
        public static DateTime getDataformat(string DateValue, DateTime date)
        {
            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            try
            {
                if (DateValue != null || DateValue != "")
                {
                    IFormatProvider cultureDDMMYYYY = new CultureInfo("fr-Fr", true);
                    IFormatProvider cultureMMDDYYYY = new CultureInfo("en-US", true);
                    DateTime currentDate = DateTime.Now;
                    IFormatProvider culture = cultureDDMMYYYY;
                    DateTime.TryParse(DateValue, culture, DateTimeStyles.NoCurrentDateDefault, out date);
                }
            }
            catch (Exception ex)
            {

            }
            return date;
        }
    }
}
