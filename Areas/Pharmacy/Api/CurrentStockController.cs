using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/CurrentStockApi")]
    [ApiController]

    public class CurrentStockController : Controller
    {

        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ICurrentStockRepo _currentStockRepo;
        private readonly IDrugMastersRepo _IDrugMasterRepo;
        private readonly IInstrumentDIRepo _instrumentDIRepo;

        public CurrentStockController(IDBConnection dBConnection, IErrorlog errorlog, ICurrentStockRepo currentStockRepo ,
            IDrugMastersRepo drugMastersRepo,IInstrumentDIRepo instrumentDIRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _currentStockRepo = currentStockRepo;
            _IDrugMasterRepo = drugMastersRepo;
            _instrumentDIRepo = instrumentDIRepo;
        }

        [HttpGet("GetCurrentStockDetailsBySearch")]
        public List<CurrentStockInfo> GetCurrentStockDetailsBySearch(string storeName, string Search)
        {
            List<CurrentStockInfo> lstResult = new List<CurrentStockInfo>();
            try
            {
                lstResult = _currentStockRepo.GetCurrentStockDetailsBySearch(storeName, Search);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }

        #region Habib
        [HttpGet("GetStockStatementProductList")]
        public JsonResult GetStockStatementProductList(long ItemCatId, string StoreName, string Search)
        {
            List<CurrentStockInfo> lstProduct = new List<CurrentStockInfo>();
            List<InstrumentCurrentStockInfo> lstSurgical = new List<InstrumentCurrentStockInfo>();
            long HospitalId = 0;
            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (ItemCatId == 1)
                {
                    lstProduct = _currentStockRepo.GetStockStatementProductList(ItemCatId, StoreName, Search, HospitalId);
                    return Json(lstProduct);
                }
                else if(ItemCatId == 3)
                {
                    lstSurgical = _instrumentDIRepo.GetSurgicalStockStatement(ItemCatId, StoreName, Search, HospitalId);
                    return Json(lstSurgical);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("StockStatementApiController GetProductList " + ex.ToString());
            }

            return Json(lstProduct);
        }

        [HttpGet("GetStockMovementDetailsByDrugCode")]
        public List<StockMovementDetails> GetStockMovementDetailsByDrugCode(long DrugCode)
        {
            List<StockMovementDetails> lstStatement = new List<StockMovementDetails>();
            try
            {
                lstStatement = _currentStockRepo.GetStockMovementDetailsByDrugCode(DrugCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("GetStockMovementDetailsByDrugCode " + ex.ToString());
            }

            return lstStatement;
        }
        #endregion

        [HttpGet("GetCurrentStockDetailsByHospitalId")]
        public List<CurrentStockInfo> GetCurrentStockDetailsByHospitalId()
        {
            List<CurrentStockInfo> lstresult = new List<CurrentStockInfo>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstresult = _currentStockRepo.GetCurrentStockDetailsByHospitalId(HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog( ex.ToString());
            }

            return lstresult;
        }
        [HttpGet("GetCurrentStockDetailsBySeqId")]
        public JsonResult  GetCurrentStockDetailsBySeqId(int SeqId)
        {

            int retValue = 0;
            List<DrugMaster> lstresult = new List<DrugMaster>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retValue = _currentStockRepo.GetCurrentStockDetailsBySeqId(SeqId , HospitalId);
                
                if (retValue > 0)
                {
                    long PH_ITEM_DrugCode = SeqId;
                    lstresult = _IDrugMasterRepo.GetDrugMasterDetailsByDrugCode(PH_ITEM_DrugCode);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Json(new { Result = lstresult }) ;
           
        }
        [HttpGet("GetStockMovementByDate")]
        public JsonResult GetStockMovementByDate(string Start, string To,int DrugCode,string Waherhoues)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            DateTime FromDate = DateTime.Now;
            DateTime StartDate = GetDataformat(Start, FromDate);
            DateTime ToDate = GetDataformat(To, FromDate);
            Start = StartDate.ToString("yyyy-MM-dd 00:00:00");
            To = ToDate.ToString("yyyy-MM-dd 23:59:50");
            List<StockMovementInfo> lstResult = new List<StockMovementInfo>();
            try
            {
                lstResult = _currentStockRepo.GetStockMovementsByCond(DrugCode, Start, To, HospitalId, Waherhoues);
                if (lstResult.Count > 0)
                {
                    for(int count = 0; count < lstResult.Count; count++)
                    {
                        string Batch = lstResult[count].BatchNum;
                        int StockQty = _currentStockRepo.GetCureentStockByCond(DrugCode, Batch, Waherhoues);
                        lstResult[count].TotalStock = StockQty;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult });
        }
        public static DateTime GetDataformat(string DateValue, DateTime date)
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
        [HttpGet("GetStockMovementDetailsByDate")]
        public List<StockMovementInfo> GetStockMovementDetailsByDate(string Start, string To, int DrugCode, string Waherhoues,string BatchNo)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            DateTime FromDate = DateTime.Now;
            DateTime StartDate = GetDataformat(Start, FromDate);
            DateTime ToDate = GetDataformat(To, FromDate);
            Start = StartDate.ToString("yyyy-MM-dd 00:00:00");
            To = ToDate.ToString("yyyy-MM-dd 23:59:50");
            List<StockMovementInfo> lstResult = new List<StockMovementInfo>();
            try
            {
                    lstResult = _currentStockRepo.GetStockMovementDetailsByDrugCodeAndStore(Start, To, HospitalId, DrugCode, Waherhoues, BatchNo);
             
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("CreateStockStatementByPrint")]
        public JsonResult CreateStockStatementByPrint(string StoreName)
        {
            List<CurrentStockInfo> lstResult = new List<CurrentStockInfo>();
            List<ClientDeatilsInfo> lstClientResult = new List<ClientDeatilsInfo>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _currentStockRepo.CreateStockStatementByPrint(StoreName, HospitalId);
                lstClientResult = _currentStockRepo.GetClientDetailsById( HospitalId);
            
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("CreateStockStatementByPrint lstResult " + ex.ToString());
            }

            return Json(new { lstResult = lstResult , lstClientResult = lstClientResult }) ;
        }
    }
}
