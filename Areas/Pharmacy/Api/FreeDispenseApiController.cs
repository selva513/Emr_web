using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/[controller]")]
    [ApiController]
    public class FreeDispenseApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IFreeDispenseRepo _freeDispenseRepo ;
        private readonly INewInvoiceRepo _newInvoiceRepo;

        public FreeDispenseApiController(IDBConnection dBConnection, IErrorlog errorlog, IFreeDispenseRepo freeDispenseRepo, INewInvoiceRepo newInvoiceRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _freeDispenseRepo = freeDispenseRepo;
            _newInvoiceRepo = newInvoiceRepo;
        }

        [HttpGet("GetGroups")]
        public FreeGroup[] GetGroups()
        {
            List<FreeGroup> lstgroup = new List<FreeGroup>();
            try
            {
                lstgroup = _freeDispenseRepo.GetDefaultGroups();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstgroup.ToArray();
        }
        [HttpGet("GetFreeDrugSearchByFreeText")]
        public JsonResult GetFreeDrugSearchByFreeText(string SearchTearm, string StoreName)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugFreeSearch> lstResult = new List<DrugFreeSearch>();
            if (!string.IsNullOrWhiteSpace(SearchTearm))
            {
                var EmptySearch = SearchTearm.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < EmptySearch.Length; i++)
                {
                    if (i == 0)
                    {
                        SearchTearm = EmptySearch[0];
                    }
                    if (i == 1)
                    {
                        SearchTearm = EmptySearch[1];
                    }
                }
            }
            try
            {
                lstResult = _freeDispenseRepo.GetFreeDrugSearchByFreeText(SearchTearm, HospitalID, StoreName);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetLast100FreeBillHeader")]
        public JsonResult GetLast100FreeBillHeader(string StoreName)
        {
            List<BillHeader> lstResult = new List<BillHeader>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _freeDispenseRepo.GetLast100FreeBillHeader(StoreName, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetFreeCashBillHeaderByBillNo")]
        public JsonResult GetFreeCashBillHeaderByBillNo(long BillNumber)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            try
            {
                billHeaders = _freeDispenseRepo.GetFreeCashBillHeaderByBillNo(BillNumber);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(billHeaders);
        }
        [HttpGet("GetFreeCashBillBySearch")]
        public JsonResult GetFreeCashBillBySearch(string Search)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            try
            {
                billHeaders = _freeDispenseRepo.GetFreeCashBillBySearch(Search);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(billHeaders);
        }
        [HttpGet("GetFreeStoreName")]
        public JsonResult GetFreeStoreName()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<StoreNameInfo> lstResult = new List<StoreNameInfo>();
            try
            {
                lstResult = _newInvoiceRepo.GetStoreName(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
    }
}
