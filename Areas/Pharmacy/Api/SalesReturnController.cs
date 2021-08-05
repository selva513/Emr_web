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
    [Route("api/SalesReturnApi")]
    [ApiController]
    public class SalesReturnController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ISalesReturnRepo _salesReturnRepo;
        private readonly ICashBillRepo _cashBillRepo;

        public SalesReturnController(IDBConnection dBConnection, IErrorlog errorlog, ISalesReturnRepo salesReturnRepo, ICashBillRepo cashBillRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _salesReturnRepo = salesReturnRepo;
            _cashBillRepo = cashBillRepo;
        }

        //[HttpGet("GetRetBillDrugDetailByBillNo")]

        //public JsonResult GetRetBillDrugDetailByBillNo(long BILLNO, string storeName)
        //{
        //    List<SalesReturn> lstRetDetail = new List<SalesReturn>();
        //    List<SalesReturn> lstDrugDetail = new List<SalesReturn>();

        //    try
        //    {
        //        lstRetDetail = _salesReturnRepo.GetSalesRetDetailByBillNo(BILLNO);
        //        for (int i = 0; i < lstRetDetail.Count(); i++)
        //        {
        //            storeName = lstRetDetail[i].PH_CSH_PROCESSKEY;
        //            if (storeName == "PHBYSOP")
        //            {
        //                storeName = "OPPHARMACY";
        //                break;
        //            }
        //            if (storeName == "PHBYSIP")
        //            {
        //                storeName = "IPPHARMACY";
        //                break;
        //            }
        //            if (storeName == "PHBYIRD")
        //            {
        //                storeName = "MAINSTORE";
        //                break;
        //            }
        //        }
        //        lstDrugDetail = _salesReturnRepo.GetRetBillDrugDetailByBillNo(BILLNO, storeName);
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorlog.WriteErrorLog(ex.ToString());
        //    }
        //    return Json(new { Result = lstRetDetail, ResultDrug = lstDrugDetail });

        //}
        //[HttpPost("CreateSalesReturnHDR")]
        //public long CreateSalesReturnHDR([FromBody] SalesInfo salesInfo)
        //{
        //    long retValue = 0;
        //    try
        //    {
        //        retValue = _salesReturnRepo.CreateSalesReturnHDR(salesInfo);

        //    }
        //    catch (Exception ex)
        //    {
        //        _errorlog.WriteErrorLog(ex.ToString());
        //    }
        //    return retValue;
        //}

        [HttpGet("GetStoreDeatailsByHospitalId")]
        public List<SalesReturn> GetStoreDeatailsByHospitalId()
        {
            List<SalesReturn> lstReturn = new List<SalesReturn>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstReturn = _salesReturnRepo.GetStoreDeatailsByHospitalId(HospitalID);
            }

            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstReturn;
        }

        [HttpGet("GetRetBillDrugDetailByBillNo")]
        public JsonResult GetRetBillDrugDetailByBillNo(long BillNo)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            List<CashBillDeatilsInfo> cashBillDeatilsInfos = new List<CashBillDeatilsInfo>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(BillNo);
                if(billHeaders.Count() > 0)
                {
                    cashBillDeatilsInfos = _cashBillRepo.GetCashBillDeatilsByBillNo(BillNo);
                }
               
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos });
        }
    }
}
