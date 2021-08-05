using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using BizLayer.Utilities;
using System.Text.RegularExpressions;
using System.Data;
using BizLayer.Interface;
using System.Globalization;
using PharmacyBizLayer.Repo;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Emr_web.Controllers.PharmaApi
{
    [Produces("application/json")]
    [Route("Pharma/DcPurchase")]
    [ApiController]
    public class DCPurchaseAPIController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly IDCPurchaseRepo _dCPurchaseRepo;
        private readonly IPurchaseOrderRepo _purchaseOrderRepo;
        public DCPurchaseAPIController(IDBConnection iDBConnection, IErrorlog errorlog, INewInvoiceRepo newInvoiceRepo,ICashBillRepo cashBillRepo,IDCPurchaseRepo dCPurchaseRepo,IPurchaseOrderRepo purchaseOrderRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _newInvoiceRepo = newInvoiceRepo;
            _cashBillRepo = cashBillRepo;
            _dCPurchaseRepo = dCPurchaseRepo;
            _purchaseOrderRepo = purchaseOrderRepo;
        }
        [HttpPost("DcPurchaseSave")]
        public JsonResult DcPurchaseSave([FromBody] DcPurchaseInfo dcPurchaseInfo)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            string StatusText = "";
            string storeName = dcPurchaseInfo.WareHouse;
            long DcSeqID = _dCPurchaseRepo.IsSupplierDcPurchaseExist(dcPurchaseInfo.SupplierDCNumber,HospitalId);
            if (DcSeqID <= 0)
            {
                if (dcPurchaseInfo.DraftDeatils.Length > 0)
                {
                    try
                    {
                        for (int count = 0; count < dcPurchaseInfo.DraftDeatils.Length; count++)
                        {
                            int DrugCode = dcPurchaseInfo.DraftDeatils[count].DrugCode;
                            string Batch = dcPurchaseInfo.DraftDeatils[count].Batch;
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
                                int Qty = dcPurchaseInfo.DraftDeatils[count].Qty;
                                int FreeQty = dcPurchaseInfo.DraftDeatils[count].FreeQty;
                                int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty + FreeQty;
                                string Remarks = "Drug Inward";
                                string Reference = dcPurchaseInfo.SupplierDCNumber;
                                int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(dcPurchaseInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                StockMovement stockMovement = new StockMovement
                                {
                                    PH_RUN_PROCESSIDKEY = "Invoice",
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
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(dcPurchaseInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                CurrentStockInfo currentStockInfo = new CurrentStockInfo
                                {
                                    PH_CUR_SEQID = (int)CurrentStockID,
                                    PH_CUR_DRUGCODE = dcPurchaseInfo.DraftDeatils[count].DrugCode,
                                    PH_CUR_STOCK_BATCHNO = dcPurchaseInfo.DraftDeatils[count].Batch,
                                    PH_CUR_OLDDRUGCODE = "",
                                    PH_CUR_DRUGBRANDNAME = dcPurchaseInfo.DraftDeatils[count].DrugName,
                                    PH_CUR_OPSEQID = 0,
                                    PH_CUR_STOCK = dcPurchaseInfo.DraftDeatils[count].Qty + dcPurchaseInfo.DraftDeatils[count].FreeQty,
                                    PH_CUR_STOCKUOM = "",
                                    PH_CUR_STOCK_INLOCK = 0,
                                    PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    PH_CUR_STOCK_PURCHCOST = dcPurchaseInfo.DraftDeatils[count].Rate,
                                    PH_CUR_STOCK_BILLINGPRICE = dcPurchaseInfo.DraftDeatils[count].MRP,
                                    PH_CUR_STOCKISZERO = false,
                                    PH_CUR_STOCK_STORENAME = storeName,
                                    PH_CUR_LAST_PROCESSKEY = "Invoice",
                                    PH_CUR_LAST_TRANSID = "",
                                    PH_CUR_isROWACTIVE = true,
                                    PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    remarks = "Drug Inward",
                                    HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                                    Reference = dcPurchaseInfo.SupplierDCNumber
                                };
                                long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                                if (CurrentsockInsert > 0)
                                {

                                    long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                    int Qty = dcPurchaseInfo.DraftDeatils[count].Qty;
                                    int CurrentStock = Qty;
                                    StockMovement stockMovement = new StockMovement
                                    {
                                        PH_RUN_PROCESSIDKEY = "PHBYORD",
                                        PH_RUN_STORENAME = storeName,
                                        PH_RUN_DRUGCODE = DrugCode,
                                        PH_RUN_STOCK_TRANSACTVALUE = dcPurchaseInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_AFTERTRANSACT = dcPurchaseInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_LEFTOUTINBATCH = dcPurchaseInfo.DraftDeatils[count].Qty,
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
                        DateTime SupDate = DateTime.Now;
                        DateTime SupplierInvoiceDate = GetDataformat(dcPurchaseInfo.SupplierDCDate, SupDate);
                        DcHeaderInfo dcHeaderInfo = new DcHeaderInfo
                        {
                            PH_DC_DOCNO = dcPurchaseInfo.SupplierDCNumber,
                            PH_DC_SUPID = dcPurchaseInfo.SupplierID,
                            PH_DC_POREFNO = "",
                            PH_DC_UniqueKey = "",
                            PH_DC_ENTRYDATE = SupDate.ToString("yyyy-MM-dd"),
                            PH_DC_ITEMCOUNT = dcPurchaseInfo.DraftDeatils.Length,
                            PH_DC_ISTOINVOICEDONE = false,
                            PH_DC_INVOiCENO = 0,
                            PH_DC_SUPP_INVNO = "",
                            PH_DC_TOTAL_AMOUNT = dcPurchaseInfo.NetAmount,
                            PH_DC_CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            PH_DC_ModifiedBy = HttpContext.Session.GetString("Userseqid"),
                            PH_DC_ModifiedDTtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            PH_DC_InvoiceAmt = dcPurchaseInfo.Amount,
                            PH_DC_InvoiceTax = dcPurchaseInfo.Tax,
                            PH_DC_InvoiceTotalAmt = dcPurchaseInfo.NetAmount,
                            DcTaxType = dcPurchaseInfo.DcTaxType,
                            WareHouse=storeName,
                            HospitalID=HospitalId
                        };
                        long HeaderSeqID = _dCPurchaseRepo.CreateNewDcHeader(dcHeaderInfo);
                        if (HeaderSeqID > 0)
                        {
                            for (int count = 0; count < dcPurchaseInfo.DraftDeatils.Length; count++)
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(dcPurchaseInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                DcDeatilsInfo dcDeatilsInfo = new DcDeatilsInfo
                                {
                                    PH_DC_SEQID = (int)HeaderSeqID,
                                    PH_DCDTL_DOCNO = count + 1,
                                    PH_DCDTL_DRUGCODE = dcPurchaseInfo.DraftDeatils[count].DrugCode,
                                    PH_DCDTL_DRUGUOM = "",
                                    PH_DCDTL_DRUGBATCHNO = dcPurchaseInfo.DraftDeatils[count].Batch,
                                    PH_DCDTL_DRUGEXPIRY = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                    PH_DCDTL_RECVDQTY = dcPurchaseInfo.DraftDeatils[count].Qty,
                                    PH_DCDTL_BONUSQTY = dcPurchaseInfo.DraftDeatils[count].FreeQty,
                                    PH_DCDTL_RATEEACH = dcPurchaseInfo.DraftDeatils[count].Rate,
                                    PH_DCDTL_AMOUNT = dcPurchaseInfo.DraftDeatils[count].NetAmount,
                                    PH_DCDTL_MOVEDTO_CSTOCK = true,
                                    PH_DCDTL_POBYREF = "",
                                    PH_DCDTL_ISACTIVE = true,
                                    PH_DCDTL_COMPANY = "",
                                    MRP = dcPurchaseInfo.DraftDeatils[count].MRP,
                                    IsMovedInvoice = false,
                                    DiscountPrec= dcPurchaseInfo.DraftDeatils[count].Discount
                                };
                                long dtlSeqID = _dCPurchaseRepo.CreateNewDcDeatils(dcDeatilsInfo);
                                long PurDltSeqID = dcPurchaseInfo.DraftDeatils[count].DtlSeqID;
                                if (PurDltSeqID > 0)
                                    _purchaseOrderRepo.UpdatePurchaseDtlBySeqID(PurDltSeqID);
                                StatusText = "Save Success";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StatusText = "Logical Error";
                    }
                }
            }
            else
            {
                StatusText = "Supplier Dc Number Exists";
            }
            return Json(StatusText);
        }
        public string Validation(List<DrafView> drafViews)
        {
            string ErrorMsg = "";
            if (drafViews.Count > 0)
            {
                for(int count = 0; count < drafViews.Count; count++)
                {
                    string ExpiryDate = drafViews[count].ExpiryDate;
                    if (string.IsNullOrEmpty(ExpiryDate))
                    {
                        ErrorMsg += "Please Enter Expiry Date" + drafViews[count].DrugName;
                        break;
                    }
                    else
                    {
                        DateTime ExDate = DateTime.Now;
                        DateTime RunningExpiryDate = GetDataformat(drafViews[count].ExpiryDate, ExDate);
                        if (DateTime.Now > RunningExpiryDate)
                        {
                            ErrorMsg += "Please Enter Valid Expiry Date" + drafViews[count].DrugName;
                            break;
                        }
                    }
                    
                }
            }
            return ErrorMsg;
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
        [HttpGet("GetTop100DcPurchase")]
        public JsonResult GetTop100DcPurchase()
        {
            List<DcHeaderInfo> lstResult = new List<DcHeaderInfo>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _dCPurchaseRepo.GetTop100DcPurchase(HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetDcDrugDeatilsBySeqID")]
        public JsonResult GetDcDrugDeatilsBySeqID(long SeqID)
        {
            List<DcDeatilsInfo> lstResult = new List<DcDeatilsInfo>();
            try
            {
                lstResult = _dCPurchaseRepo.GetDcDrugDeatilsBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult});
        }

        [HttpGet("GetSupplierTop100DcPurchase")]
        public JsonResult GetSupplierTop100DcPurchase(long SupplierID)
        {
            List<DcHeaderInfo> lstResult = new List<DcHeaderInfo>();
            try
            {
                lstResult = _dCPurchaseRepo.GetSupplierTop100DcPurchase(SupplierID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        #region ABdullah
        [HttpGet("GetDcNumberDetailsBySearch")]
        public JsonResult GetDcNumberDetailsBySearch(string Search, long SupplierId)
        {
            List<DcHeaderInfo> lstResult = new List<DcHeaderInfo>();
            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _dCPurchaseRepo.GetDcNumberDetailsBySearch(HospitalId, Search, SupplierId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        #endregion
    }
}