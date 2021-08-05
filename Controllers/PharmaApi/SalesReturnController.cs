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

namespace Emr_web.Controllers.PharmaApi
{
    [Route("api/[controller]")]
    [Route("Pharma/SalesReturn")]
    public class SalesReturnController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly ISalesReturnRepo _salesReturnRepo;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        public SalesReturnController(IDBConnection dBConnection, IErrorlog errorlog, ICashBillRepo cashBillRepo,ISalesReturnRepo salesReturnRepo, INewInvoiceRepo newInvoiceRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _cashBillRepo = cashBillRepo;
            _salesReturnRepo = salesReturnRepo;
            _newInvoiceRepo = newInvoiceRepo;
        }
        [HttpGet("GetRetBillDrugDetailByBillNo")]
        public JsonResult GetRetBillDrugDetailByBillNo(long BillNo)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            List<CashBillDeatilsInfo> cashBillDeatilsInfos = new List<CashBillDeatilsInfo>();
            try
            {
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(BillNo);
                cashBillDeatilsInfos = _cashBillRepo.GetCashBillDeatilsByBillNo(BillNo);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos });
        }
        [HttpPost("RetBillSave")]
        public JsonResult RetBillSave([FromBody] SalesReturnInfo salesReturnInfo)
        {
            List<ReturnBillHeader> listRetHeader = new List<ReturnBillHeader>();
            List<ReturnBillDeatils> listRetDtls = new List<ReturnBillDeatils>();
            if (salesReturnInfo.returnDrugInfos.Length > 0)
            {
                List<BillHeader> billHeaders = new List<BillHeader>();
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(salesReturnInfo.BillNo);
                DateTime ExDate = DateTime.Now;
                DateTime RunningExpiryDate = GetDataformat(billHeaders[0].PH_CSH_BILLDT, ExDate);
                
                try
                {
                    for(int count = 0; count < salesReturnInfo.returnDrugInfos.Length; count++)
                    {
                        string storeName = salesReturnInfo.StoreName;
                        string Batch = salesReturnInfo.returnDrugInfos[count].Batch;
                        int DrugCode = salesReturnInfo.returnDrugInfos[count].DrugCode;
                        List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                        if (currentStockTimes.Count > 0)
                        {
                            byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                            currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                            if (currentStockTimes.Count > 0)
                            {
                            }
                            else
                            {
                                currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                            }
                            int Qty = salesReturnInfo.returnDrugInfos[count].ReturnQty;
                            int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                            string Remarks = "Sales Return";
                            string Reference = salesReturnInfo.BillNo.ToString();
                            int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                            ExDate = DateTime.Now;
                            RunningExpiryDate = GetDataformat(salesReturnInfo.returnDrugInfos[count].ExpiryDate, ExDate);
                            StockMovement stockMovement = new StockMovement
                            {
                                PH_RUN_PROCESSIDKEY = "PHBYORD",
                                PH_RUN_STORENAME = storeName,
                                PH_RUN_DRUGCODE = DrugCode,
                                PH_RUN_STOCK_TRANSACTVALUE = Qty,
                                PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                                PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                                PH_RUN_BATCHNO = Batch,
                                PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                PH_RUN_DOC_HDRNO = 1,
                                PH_RUN_DOC_DTLNO = 1,
                                PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                                PH_RUN_FINYEAR = "20-21",
                                PH_RUN_TRANSACT_ISACTIVE = true,
                                PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                                PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            };
                            int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                        }
                        else
                        {
                            long CurrentStockID = _newInvoiceRepo.GetCurrentStockSeqID();
                            ExDate = DateTime.Now;
                            RunningExpiryDate = GetDataformat(salesReturnInfo.returnDrugInfos[count].ExpiryDate, ExDate);
                            CurrentStockInfo currentStockInfo = new CurrentStockInfo
                            {
                                PH_CUR_SEQID = (int)CurrentStockID,
                                PH_CUR_DRUGCODE = salesReturnInfo.returnDrugInfos[count].DrugCode,
                                PH_CUR_STOCK_BATCHNO = salesReturnInfo.returnDrugInfos[count].Batch,
                                PH_CUR_OLDDRUGCODE = "",
                                PH_CUR_DRUGBRANDNAME = salesReturnInfo.returnDrugInfos[count].DrugName,
                                PH_CUR_OPSEQID = 0,
                                PH_CUR_STOCK = salesReturnInfo.returnDrugInfos[count].ReturnQty,
                                PH_CUR_STOCKUOM = "",
                                PH_CUR_STOCK_INLOCK = 0,
                                PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                PH_CUR_STOCK_PURCHCOST = salesReturnInfo.returnDrugInfos[count].Rate,
                                PH_CUR_STOCK_BILLINGPRICE = salesReturnInfo.returnDrugInfos[count].MRP,
                                PH_CUR_STOCKISZERO = false,
                                PH_CUR_STOCK_STORENAME = storeName,
                                PH_CUR_LAST_PROCESSKEY = "Invoice",
                                PH_CUR_LAST_TRANSID = "",
                                PH_CUR_isROWACTIVE = true,
                                PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                remarks = "Sales Return",
                                HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                                Reference=salesReturnInfo.BillNo.ToString()
                            };
                            long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                            if (CurrentsockInsert > 0)
                            {

                                long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                int Qty = salesReturnInfo.returnDrugInfos[count].ReturnQty;
                                int CurrentStock = Qty;
                                StockMovement stockMovement = new StockMovement
                                {
                                    PH_RUN_PROCESSIDKEY = "PHBYORD",
                                    PH_RUN_STORENAME = storeName,
                                    PH_RUN_DRUGCODE = DrugCode,
                                    PH_RUN_STOCK_TRANSACTVALUE = salesReturnInfo.returnDrugInfos[count].ReturnQty,
                                    PH_RUN_STOCK_AFTERTRANSACT = salesReturnInfo.returnDrugInfos[count].ReturnQty,
                                    PH_RUN_STOCK_LEFTOUTINBATCH = salesReturnInfo.returnDrugInfos[count].ReturnQty,
                                    PH_RUN_BATCHNO = Batch,
                                    PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                    PH_RUN_DOC_HDRNO = 1,
                                    PH_RUN_DOC_DTLNO = 1,
                                    PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                                    PH_RUN_FINYEAR = "20-21",
                                    PH_RUN_TRANSACT_ISACTIVE = true,
                                    PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                                    PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                };
                                int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                            }
                        }
                    }
                    SalesInfo salesInfo = new SalesInfo
                    {
                        PH_RET_BILLDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        PH_RET_PATID = billHeaders[0].PH_CSH_PATID,
                        PH_RET_PATNAME = billHeaders[0].PH_CSH_PATNAME,
                        PH_RET_PHONE = billHeaders[0].PH_CSH_PHONE,
                        PH_RET_NETTAMOUNT = salesReturnInfo.NetAmount,
                        PH_RET_ROUNDOFF = salesReturnInfo.RoundAmount,
                        PH_RET_TAXAMOUNT = salesReturnInfo.Tax,
                        PH_RET_CONCESSION = salesReturnInfo.DisCount,
                        PH_RET_TOTAMOUNT = salesReturnInfo.TotalAmount,
                        PH_RET_FINYEAR = "20-21",
                        PH_RET_CASHRECIVEDAMT = salesReturnInfo.NetAmount,
                        PH_RET_ADVANCEADJUSTED = 0,
                        PH_RET_ADVANCEVOUCHERNO = 0,
                        PH_RET_ISactive = true,
                        PH_RET_CreateDUser = HttpContext.Session.GetString("Userseqid"),
                        PH_RET_REFUNDAMT = 0,
                        PH_RET_CURREFUNDAMT = 0,
                        PH_REF_DOCTOR = billHeaders[0].PH_REF_DOCTOR,
                        PH_RET_CURRENTPENDING_AMOUNT = 0,
                        PH_RET_CCARDRECIEVEDAMT = 0,
                        PH_RET_PENDINGTOPAY = 0,
                        PH_CSH_BILLNO = salesReturnInfo.BillNo,
                        PH_CSH_BILLDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                        StoreName= salesReturnInfo.StoreName
                    };
                    long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                    long RetHeaderID = _salesReturnRepo.CreateSalesReturnHDR(salesInfo, HospitalId);
                    if (RetHeaderID > 0)
                    {
                        for (int count = 0; count < salesReturnInfo.returnDrugInfos.Length; count++)
                        {
                            ExDate = DateTime.Now;
                            RunningExpiryDate = GetDataformat(salesReturnInfo.returnDrugInfos[count].ExpiryDate, ExDate);
                            salesInfo.PH_RET_BILLNO = RetHeaderID;
                            salesInfo.PH_RETDTL_DRUGCODE = salesReturnInfo.returnDrugInfos[count].DrugCode;
                            salesInfo.PH_RETDTL_DRUG_QTY= salesReturnInfo.returnDrugInfos[count].ReturnQty;
                            salesInfo.PH_RETDTL_DRUGBATCHNO = salesReturnInfo.returnDrugInfos[count].Batch;
                            salesInfo.PH_RETDTL_DRUGEXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd");
                            salesInfo.PH_RETDTL_DRUG_AMTEACH = salesReturnInfo.returnDrugInfos[count].MRP;
                            salesInfo.PH_RETDTL_DRUG_ROWTOTALAMT = salesReturnInfo.returnDrugInfos[count].Amount;
                            salesInfo.PH_RETDTL_DRUG_CONCESSION_AMT = 0;
                            salesInfo.PH_RETDTL_DRUG_TAXPERCENT = salesReturnInfo.returnDrugInfos[count].TaxPrecentage;
                            salesInfo.PH_RET_TAXAMOUNT = salesReturnInfo.returnDrugInfos[count].Tax;
                            salesInfo.PH_RET_NETTAMOUNT= salesReturnInfo.returnDrugInfos[count].NetAmount;
                            salesInfo.PH_RETDTL_DRUGSTOCK_BEFOREDISPENSE = salesReturnInfo.returnDrugInfos[count].Qty;
                            long retDtlResult = _salesReturnRepo.CreateSalesReturnDTL(salesInfo);
                        }
                        listRetHeader = _salesReturnRepo.GetRetBillHeaderByBillNo(RetHeaderID, HospitalId);
                        listRetDtls = _salesReturnRepo.GetReturnDtlByBillNo(RetHeaderID, HospitalId);
                    }
                }
                catch(Exception ex)
                {

                }
            }
            return Json(new { PrintHeader = listRetHeader, PrintDeatils = listRetDtls });
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
        [HttpGet("GetSelectedReturnBill")]
        public JsonResult GetSelectedReturnBill(long BillNo)
        {
            List<ReturnBillHeader> listRetHeader = new List<ReturnBillHeader>();
            List<ReturnBillDeatils> listRetDtls = new List<ReturnBillDeatils>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                listRetHeader = _salesReturnRepo.GetRetBillHeaderByBillNo(BillNo, HospitalId);
                listRetDtls = _salesReturnRepo.GetReturnDtlByBillNo(BillNo, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = listRetHeader, PrintDeatils = listRetDtls });
        }
        [HttpGet("GetRetBillHeaderTop100")]
        public JsonResult GetRetBillHeaderTop100()
        {
            List<ReturnBillHeader> listRetHeader = new List<ReturnBillHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                listRetHeader = _salesReturnRepo.GetRetBillHeaderTop100(HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(listRetHeader);
        }
        #region ABdullah
        [HttpGet("GetRetBillDetailByBillNo")]
        public List<ReturnBillHeader> GetRetBillDetailByBillNo(long BillNo)
        {
            List<ReturnBillHeader> listResult= new List<ReturnBillHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                listResult = _salesReturnRepo.GetRetBillDetailByBillNo(BillNo, HospitalId);
            }
            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return listResult;
        }
        [HttpGet("GetHeaderSelectedReturnBill")]
        public List<ReturnBillHeader> GetHeaderSelectedReturnBill(long BillNo)
        {
            List<ReturnBillHeader> listRetHeader = new List<ReturnBillHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                listRetHeader = _salesReturnRepo.GetCashBillHeaderByBillNo(BillNo, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return listRetHeader;
        }
        #endregion
    }
}