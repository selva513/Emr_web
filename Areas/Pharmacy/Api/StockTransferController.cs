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


namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/StockTransfer")]
    [ApiController]
    public class StockTransferController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IStockTransferRepo _stockTransferRepo;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        private readonly ICashBillRepo _cashBillRepo;
        
        public StockTransferController(IDBConnection dBConnection, IErrorlog errorlog, IStockTransferRepo stockTransferRepo, INewInvoiceRepo newInvoiceRepo, ICashBillRepo cashBillRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _stockTransferRepo = stockTransferRepo;
            _newInvoiceRepo = newInvoiceRepo;
            _cashBillRepo = cashBillRepo;
        }
        [HttpGet("GetDrugDetailsBySearch")]
        public List<StockTransfer> GetDrugDetailsBySearch(string search, string storeName)
        {
            List<StockTransfer> lstresult = new List<StockTransfer>();
            try
            {
                lstresult = _stockTransferRepo.GetDrugDetailsBySearch(search, storeName);
            }
            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return lstresult;
        }
        [HttpGet("GetDrugInfoByDrugName")]
        public List<StockTransfer> GetDrugInfoByDrugName(string drugName, string storeName)
        {
            List<StockTransfer> lstresult = new List<StockTransfer>( );
            string[] arr = drugName.Split('^');
            drugName = arr[0].ToString();
            string stockUom = arr[2].ToString();
            string shortCode = arr[3].ToString();
            //List<StockTransfer> drugList = null;
            try
            {
                lstresult = _stockTransferRepo.GetDrugInfoByDrugName(drugName, storeName,stockUom, shortCode);
            }
            catch(Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstresult;
        }
        [HttpPost("SaveStockTransfer")]
        public JsonResult SaveStockTransfer([FromBody] StockTransferInfo transferInfo)
        {
            string StatusText = "";
            string storeName = transferInfo.FromStoreName;
            List<StockTransferHeader> lstStockTransHeader = new List<StockTransferHeader>();
            List<StockTransferDeatils> lstStockTransDtl = new List<StockTransferDeatils>();
            try
            {
                if (transferInfo.TransferDeatils.Length > 0)
                {
                    
                    StockTransferHeader stockTransferHeader = new StockTransferHeader
                    {
                        PH_TRANSFERID = (int)_stockTransferRepo.GetTransferHeaderSeqID(),
                        PH_TRAN_FROMSTORENAME = transferInfo.FromStoreName,
                        PH_TRAN_TOSTORENAME = transferInfo.ToStoreName,
                        PH_TRAN_PROCESSIDKEY = "Transfer",
                        PH_TRAN_PROCESSCOMPLETED= "INTRANSIT",
                        PH_TRAN_TRANSFERDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        PH_TRAN_USERNAME = HttpContext.Session.GetString("Userseqid"),
                        PH_TRAN_ISACTIVE = true
                    };
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));

                    long TransHeadSeqID = _stockTransferRepo.CreateNewStockTransfer(stockTransferHeader, HospitalId);
                    if (TransHeadSeqID > 0)
                    {
                        for (int count = 0; count < transferInfo.TransferDeatils.Length; count++)
                        {
                            DateTime ExDate = DateTime.Now;
                            DateTime RunningExpiryDate = GetDataformat(transferInfo.TransferDeatils[count].Expiry, ExDate);
                            StockTransferDeatils stockTransferDeatils = new StockTransferDeatils
                            {
                                PH_SEQUENCEID = (int)_stockTransferRepo.GetTransferDtlSeqID(),
                                PH_TRANSFERID = stockTransferHeader.PH_TRANSFERID,
                                PH_TRAN_BRANDNAME = transferInfo.TransferDeatils[count].BrandName,
                                PH_TRAN_BATCH = transferInfo.TransferDeatils[count].Batch,
                                PH_TRAN_EXPIRYDATE = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                PH_TRAN_STOCKTOTRANSFER = transferInfo.TransferDeatils[count].StockTransferTo,
                                PH_TRAN_STOCKTRANSFER = transferInfo.TransferDeatils[count].StockTransfer,
                                PH_TRAN_STOCKBALANCE = transferInfo.TransferDeatils[count].StockBalance,
                                PH_TRAN_ISACTIVE = true,
                                PH_TRAN_DRUGCODE = transferInfo.TransferDeatils[count].DrugCode,
                                MRP= transferInfo.TransferDeatils[count].MRP,
                                Cost= transferInfo.TransferDeatils[count].Cost
                            };
                            long TransDtlResult = _stockTransferRepo.CreateNewStockTransferDeatils(stockTransferDeatils);
                            StatusText = "Save Success";
                        }

                        lstStockTransHeader = _stockTransferRepo.GetStockTransferHeaderBySeqID(stockTransferHeader.PH_TRANSFERID , HospitalId);
                        lstStockTransDtl = _stockTransferRepo.GetStockTransferDetailsBySeqID(stockTransferHeader.PH_TRANSFERID,HospitalId);
                    }
                    for (int count = 0; count < transferInfo.TransferDeatils.Length; count++)
                    {
                        int DrugCode = transferInfo.TransferDeatils[count].DrugCode;
                        string Batch = transferInfo.TransferDeatils[count].Batch;
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
                            int Qty = transferInfo.TransferDeatils[count].StockTransfer;
                            int CurrentStock = currentStockTimes[0].PH_CUR_STOCK - Qty;
                            string Remarks = "StockTransfer";
                            string Reference = "From "+ transferInfo.FromStoreName+" To "+transferInfo.ToStoreName;
                            int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                            DateTime ExDate = DateTime.Now;
                            DateTime RunningExpiryDate = GetDataformat(transferInfo.TransferDeatils[count].Expiry, ExDate);
                            StockMovement stockMovement = new StockMovement
                            {
                                PH_RUN_PROCESSIDKEY = "Transfer",
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
                    }
                }
            }
            catch
            {
                StatusText = "Logical Error";
            }
            return Json(new { PrintHeader = lstStockTransHeader, PrintDeatils = lstStockTransDtl });
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
        [HttpGet("GetStockTransferHeader")]
        public JsonResult GetStockTransferHeader()
        {
            List<StockTransferHeader> lstResult = new List<StockTransferHeader>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _stockTransferRepo.GetStockTransferHeader(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetStockTransferDetailsBySeqID")]
        public JsonResult GetStockTransferDetailsBySeqID(int SeqID)
        {
            List<StockTransferDeatils> lstResult = new List<StockTransferDeatils>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _stockTransferRepo.GetStockTransferDetailsBySeqID(SeqID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult});
        }
        [HttpGet("StockReceive")]
        public JsonResult StockReceive(int SeqID,string StoreName)
        {
            string StatusText = "";
            List<StockTransferHeader> lstStockTransHeader = new List<StockTransferHeader>();
            List<StockTransferDeatils> lstStockTransDtl = new List<StockTransferDeatils>();
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                lstStockTransHeader = _stockTransferRepo.GetStockTransferHeaderBySeqID(SeqID, HospitalId);
                string From = lstStockTransHeader[0].PH_TRAN_FROMSTORENAME;
                string To = lstStockTransHeader[0].PH_TRAN_TOSTORENAME;
                long UpdateResult = _stockTransferRepo.UpdateStockTransferHeader(SeqID);
                if (UpdateResult > 0)
                {
                    
                    List<StockTransferDeatils> stockTransferDeatils = _stockTransferRepo.GetStockTransferDetailsBySeqID(SeqID ,HospitalId);
                    if (stockTransferDeatils.Count > 0)
                    {
                        for(int count = 0; count < stockTransferDeatils.Count; count++)
                        {
                            int DrugCode = stockTransferDeatils[count].PH_TRAN_DRUGCODE;
                            string Batch = stockTransferDeatils[count].PH_TRAN_BATCH;
                            List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, StoreName);
                            if (currentStockTimes.Count > 0)
                            {
                                byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                                currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, StoreName, LastTimeStamp);
                                if (currentStockTimes.Count > 0)
                                {
                                }
                                else
                                {
                                    currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, StoreName);
                                }
                                int Qty = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER;
                                int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                                string Remarks = "StockReceive";
                                string Reference = "From " + From + " To " + To;
                                int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, StoreName, Batch, DrugCode, Remarks, Reference);
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(stockTransferDeatils[count].PH_TRAN_EXPIRYDATE, ExDate);
                                StockMovement stockMovement = new StockMovement
                                {
                                    PH_RUN_PROCESSIDKEY = "Invoice",
                                    PH_RUN_STORENAME = StoreName,
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
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(stockTransferDeatils[count].PH_TRAN_EXPIRYDATE, ExDate);
                                CurrentStockInfo currentStockInfo = new CurrentStockInfo
                                {
                                    PH_CUR_SEQID = (int)CurrentStockID,
                                    PH_CUR_DRUGCODE = stockTransferDeatils[count].PH_TRAN_DRUGCODE,
                                    PH_CUR_STOCK_BATCHNO = stockTransferDeatils[count].PH_TRAN_BATCH,
                                    PH_CUR_OLDDRUGCODE = "",
                                    PH_CUR_DRUGBRANDNAME = stockTransferDeatils[count].PH_TRAN_BRANDNAME,
                                    PH_CUR_OPSEQID = 0,
                                    PH_CUR_STOCK = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER,
                                    PH_CUR_STOCKUOM = "",
                                    PH_CUR_STOCK_INLOCK = 0,
                                    PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    PH_CUR_STOCK_PURCHCOST = stockTransferDeatils[count].Cost,
                                    PH_CUR_STOCK_BILLINGPRICE = stockTransferDeatils[count].MRP,
                                    PH_CUR_STOCKISZERO = false,
                                    PH_CUR_STOCK_STORENAME = StoreName,
                                    PH_CUR_LAST_PROCESSKEY = "Stock",
                                    PH_CUR_LAST_TRANSID = "",
                                    PH_CUR_isROWACTIVE = true,
                                    PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    remarks = "StockReceive",
                                    Reference = "From " + From + " To " + To,
                                    HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"))
                                };
                                long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                                if (CurrentsockInsert > 0)
                                {

                                    long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                    int Qty = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER;
                                    int CurrentStock = Qty;
                                    StockMovement stockMovement = new StockMovement
                                    {
                                        PH_RUN_PROCESSIDKEY = "Transfer",
                                        PH_RUN_STORENAME = StoreName,
                                        PH_RUN_DRUGCODE = DrugCode,
                                        PH_RUN_STOCK_TRANSACTVALUE = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER,
                                        PH_RUN_STOCK_AFTERTRANSACT = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER,
                                        PH_RUN_STOCK_LEFTOUTINBATCH = stockTransferDeatils[count].PH_TRAN_STOCKTRANSFER,
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
                        
                        lstStockTransHeader = _stockTransferRepo.GetStockTransferHeaderBySeqID(SeqID,HospitalId);
                        lstStockTransDtl = _stockTransferRepo.GetStockTransferDetailsBySeqID(SeqID, HospitalId);
                    }
                }
            }
            catch(Exception ex)
            {
                StatusText = "Logical Error";
            }
            return Json(new { PrintHeader = lstStockTransHeader, PrintDeatils = lstStockTransDtl });
        }
        [HttpGet("GetStockTransferReport")]
        public JsonResult GetStockTransferReport(string FromStore, string ToStore,string StDate,string EnDate)
        {
            string StatusText = "";
            DateTime ExDate = DateTime.Now;
            DateTime SDate = GetDataformat(StDate, ExDate);
            DateTime EDate = GetDataformat(EnDate, ExDate);
            List<StockTransferDeatils> lstStockTransDtl = new List<StockTransferDeatils>();
            try
            {
                StDate = SDate.ToString("yyyy-MM-dd 00:00:00");
                EnDate = EDate.ToString("yyyy-MM-dd 23:59:59");
                lstStockTransDtl = _stockTransferRepo.GetStockTransferReport(StDate, EnDate, FromStore, ToStore);
            }
            catch (Exception ex)
            {
                StatusText = "Logical Error";
            }
            return Json(lstStockTransDtl);
        }
        [HttpGet("GetStockTransferReportPrint")]
        public JsonResult GetStockTransferReportPrint(string FromStore, string ToStore, string StDate, string EnDate)
        {
            string StatusText = "";
            DateTime ExDate = DateTime.Now;
            DateTime SDate = GetDataformat(StDate, ExDate);
            DateTime EDate = GetDataformat(EnDate, ExDate);
            List<StockTransferDeatils> lstStockTransDtl = new List<StockTransferDeatils>();
            try
            {
                StDate = SDate.ToString("yyyy-MM-dd 00:00:00");
                EnDate = EDate.ToString("yyyy-MM-dd 23:59:59");
                lstStockTransDtl = _stockTransferRepo.GetStockTransferReport(StDate, EnDate, FromStore, ToStore);
            }
            catch (Exception ex)
            {
                StatusText = "Logical Error";
            }
            return Json(lstStockTransDtl);
        }
    }
}
