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
    [Route("Pharma/DcInvoice")]
    [ApiController]
    public class DcInvoiceAPIController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly IDCPurchaseRepo _dCPurchaseRepo;
        public DcInvoiceAPIController(IDBConnection iDBConnection, IErrorlog errorlog, INewInvoiceRepo newInvoiceRepo, ICashBillRepo cashBillRepo, IDCPurchaseRepo dCPurchaseRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _newInvoiceRepo = newInvoiceRepo;
            _cashBillRepo = cashBillRepo;
            _dCPurchaseRepo = dCPurchaseRepo;
        }

        [HttpGet("GetDCDrugDeatilsBySupplier")]
        public JsonResult GetDCDrugDeatilsBySupplier(int SupplierID)
        {
            List<DcDeatilsInfo> lstResult = new List<DcDeatilsInfo>();
            try
            {
                lstResult = _dCPurchaseRepo.GetDCDrugDeatilsBySupplier(SupplierID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult });
        }
        [HttpGet("GetDCDrugDeatilsByDcSeqIDList")]
        public JsonResult GetDCDrugDeatilsByDcSeqIDList(string SeqIDList)
        {
            List<DcDeatilsInfo> lstResult = new List<DcDeatilsInfo>();
            try
            {
                lstResult = _dCPurchaseRepo.GetDCDrugDeatilsByDcSeqIDList(SeqIDList);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult });
        }
        [HttpPost("SummaryCalculation")]
        public JsonResult SummaryCalculation([FromBody] DraftInvoiceSummary[] Summary)
        {
            List<DraftInvoiceSummary> lstSummary = new List<DraftInvoiceSummary>();
            
            try
            {
                  var sells = Summary
                 .GroupBy(a => a.TaxPrecentage)
                 .Select(a => new { Amount = a.Sum(b => b.Amount),
                  Tax=a.Sum(b=>b.Tax),NetAmount=a.Sum(b=>b.NetAmount),
                     TaxPrecentage=(a.Key)
                 })
                 .ToList();
                return Json(new { Summary = sells });
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Summary = lstSummary });
        }
        [HttpPost("InvoiceSave")]
        public JsonResult InvoiceSave([FromBody] InvoiceInfo invoiceInfo)
        {
            string StatusText = "";
            string storeName = invoiceInfo.WareHouse;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long SupplierSeqID = _newInvoiceRepo.IsSupplierInvoiceExist(invoiceInfo.SupplierInvoiceNumber,HospitalId);
            if (SupplierSeqID <= 0)
            {
                try
                {
                    if (invoiceInfo.DraftDeatils.Length > 0)
                    {
                        //for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                        //{
                        //    int DrugCode = invoiceInfo.DraftDeatils[count].DrugCode;
                        //    string Batch = invoiceInfo.DraftDeatils[count].Batch;
                        //    List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                        //    if (currentStockTimes.Count > 0)
                        //    {
                        //        byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                        //        currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                        //        if (currentStockTimes.Count > 0)
                        //        {
                        //        }
                        //        else
                        //        {
                        //            currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                        //        }
                        //        int Qty = invoiceInfo.DraftDeatils[count].Qty;
                        //        int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                        //        string Remarks = "DCInvoice";
                        //        string Reference = invoiceInfo.SupplierInvoiceNumber;
                        //        int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                        //        DateTime ExDate = DateTime.Now;
                        //        DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                        //        StockMovement stockMovement = new StockMovement
                        //        {
                        //            PH_RUN_PROCESSIDKEY = "Invoice",
                        //            PH_RUN_STORENAME = storeName,
                        //            PH_RUN_DRUGCODE = DrugCode,
                        //            PH_RUN_STOCK_TRANSACTVALUE = Qty,
                        //            PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                        //            PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                        //            PH_RUN_BATCHNO = Batch,
                        //            PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                        //            PH_RUN_DOC_HDRNO = 1,
                        //            PH_RUN_DOC_DTLNO = 1,
                        //            PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        //            PH_RUN_FINYEAR = "20-21",
                        //            PH_RUN_TRANSACT_ISACTIVE = true,
                        //            PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                        //            PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //        };
                        //        int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                        //    }
                        //    else
                        //    {
                        //        long CurrentStockID = _newInvoiceRepo.GetCurrentStockSeqID();
                        //        DateTime ExDate = DateTime.Now;
                        //        DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                        //        CurrentStockInfo currentStockInfo = new CurrentStockInfo
                        //        {
                        //            PH_CUR_SEQID = (int)CurrentStockID,
                        //            PH_CUR_DRUGCODE = invoiceInfo.DraftDeatils[count].DrugCode,
                        //            PH_CUR_STOCK_BATCHNO = invoiceInfo.DraftDeatils[count].Batch,
                        //            PH_CUR_OLDDRUGCODE = "",
                        //            PH_CUR_DRUGBRANDNAME = invoiceInfo.DraftDeatils[count].DrugName,
                        //            PH_CUR_OPSEQID = 0,
                        //            PH_CUR_STOCK = invoiceInfo.DraftDeatils[count].Qty,
                        //            PH_CUR_STOCKUOM = "",
                        //            PH_CUR_STOCK_INLOCK = 0,
                        //            PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        //            PH_CUR_STOCK_PURCHCOST = invoiceInfo.DraftDeatils[count].Rate,
                        //            PH_CUR_STOCK_BILLINGPRICE = invoiceInfo.DraftDeatils[count].MRP,
                        //            PH_CUR_STOCKISZERO = false,
                        //            PH_CUR_STOCK_STORENAME = storeName,
                        //            PH_CUR_LAST_PROCESSKEY = "Invoice",
                        //            PH_CUR_LAST_TRANSID = "",
                        //            PH_CUR_isROWACTIVE = true,
                        //            PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //            remarks = "DCInvoice",
                        //            HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                        //            Reference= invoiceInfo.SupplierInvoiceNumber
                        //        };
                        //        long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                        //        if (CurrentsockInsert > 0)
                        //        {

                        //            long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                        //            int Qty = invoiceInfo.DraftDeatils[count].Qty;
                        //            int CurrentStock = Qty;
                        //            StockMovement stockMovement = new StockMovement
                        //            {
                        //                PH_RUN_PROCESSIDKEY = "PHBYORD",
                        //                PH_RUN_STORENAME = storeName,
                        //                PH_RUN_DRUGCODE = DrugCode,
                        //                PH_RUN_STOCK_TRANSACTVALUE = invoiceInfo.DraftDeatils[count].Qty,
                        //                PH_RUN_STOCK_AFTERTRANSACT = invoiceInfo.DraftDeatils[count].Qty,
                        //                PH_RUN_STOCK_LEFTOUTINBATCH = invoiceInfo.DraftDeatils[count].Qty,
                        //                PH_RUN_BATCHNO = Batch,
                        //                PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                        //                PH_RUN_DOC_HDRNO = 1,
                        //                PH_RUN_DOC_DTLNO = 1,
                        //                PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        //                PH_RUN_FINYEAR = "20-21",
                        //                PH_RUN_TRANSACT_ISACTIVE = true,
                        //                PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                        //                PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //            };
                        //            int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                        //        }
                        //    }
                        //}
                        DateTime SupDate = DateTime.Now;
                        DateTime SupplierInvoiceDate = GetDataformat(invoiceInfo.SupplierInvoiceDate, SupDate);
                        InvoiceHeader invoiceHeader = new InvoiceHeader
                        {
                            PH_IN_DOCNO = invoiceInfo.SupplierInvoiceNumber,
                            PH_IN_SUPID = invoiceInfo.SupplierID,
                            PH_IN_POREFNO = "",
                            PH_IN_UniqueKey = "",
                            PH_IN_ENTRYDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_IN_ITEMCOUNT = invoiceInfo.DraftDeatils.Length,
                            PH_IN_ISTOINVOICEDONE = true,
                            PH_IN_INVOiCENO = 0,
                            PH_IN_SUPP_INVNO = invoiceInfo.SupplierInvoiceNumber,
                            PH_IN_INV_CONVERTDT = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_IN_TOTAL_AMOUNT = invoiceInfo.NetAmount,
                            PH_IN_CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            PH_IN_ModifiedBy = HttpContext.Session.GetString("Userseqid"),
                            PH_IN_ModifiedDTtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            PH_INReceivedBy = invoiceInfo.WareHouse,
                            PH_INWareHouse = invoiceInfo.WareHouse,
                            Amount = invoiceInfo.Amount,
                            Tax = invoiceInfo.Tax,
                            NetAmount = invoiceInfo.NetAmount,
                            HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                            DisType = invoiceInfo.DisType,
                            DiscountValue = invoiceInfo.DiscountValue,
                            DiscountAmt = invoiceInfo.Discount,
                            InvoiceType = invoiceInfo.InvoiceType,
                            InvoiceGSTType = invoiceInfo.InVoiceTaxType
                        };
                        long InvoiceHeaderSeqID = _newInvoiceRepo.CreateNewInvoiceHeader(invoiceHeader);
                        if (InvoiceHeaderSeqID > 0)
                        {
                            for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                InvoiceDeatils invoiceDeatils = new InvoiceDeatils
                                {
                                    PH_IN_SEQID = InvoiceHeaderSeqID,
                                    PH_INDTL_DOCNO = "",
                                    PH_INDTL_DRUGCODE = invoiceInfo.DraftDeatils[count].DrugCode,
                                    PH_INDTL_DRUGUOM = "",
                                    PH_INDTL_DRUGBATCHNO = invoiceInfo.DraftDeatils[count].Batch,
                                    PH_INDTL_DRUGEXPIRY = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                    PH_INDTL_RECVDQTY = invoiceInfo.DraftDeatils[count].Qty,
                                    PH_INDTL_BONUSQTY = invoiceInfo.DraftDeatils[count].FreeQty,
                                    PH_INDTL_RATEEACH = invoiceInfo.DraftDeatils[count].Rate,
                                    PH_INDTL_AMOUNT = invoiceInfo.DraftDeatils[count].NetAmount,
                                    PH_INDTL_MOVEDTO_CSTOCK = true,
                                    PH_INDTL_POBYREF = "",
                                    PH_INDTL_ISACTIVE = true,
                                    GST = invoiceInfo.DraftDeatils[count].TaxPrecentage,
                                    PurchaseCost = invoiceInfo.DraftDeatils[count].Rate,
                                    BillCost = invoiceInfo.DraftDeatils[count].MRP,
                                    PurAmount = invoiceInfo.DraftDeatils[count].Amount,
                                    PurTaxAmount = invoiceInfo.DraftDeatils[count].Tax,
                                    PurNetAmount = invoiceInfo.DraftDeatils[count].NetAmount
                                };
                                long InvoiceDtlSeqID = _newInvoiceRepo.NewInvoiceDeatils(invoiceDeatils);
                                long UpdateReult = _dCPurchaseRepo.UpdateDCDrugDtlBySeqID(invoiceInfo.DraftDeatils[count].DtlSeqID);
                            }
                        }
                        StatusText = "Save Success";
                    }
                }
                catch (Exception ex)
                {
                    string ErrorMsg = ex.ToString();
                    StatusText = "Logical Error";
                    long DeletSuppInvoice = _newInvoiceRepo.DeleteSupplierInvoice(invoiceInfo.SupplierInvoiceNumber);
                }
            }
            else
            {
                StatusText = "Supplier Invoice Number Exists";
            }
            return Json(StatusText);
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
        [HttpGet("GetDCDrugDeatilsBySupplierAndDrug")]
        public JsonResult GetDCDrugDeatilsBySupplierAndDrug(int SupplierID,string FreeSearch)
        {
            List<DcDeatilsInfo> lstResult = new List<DcDeatilsInfo>();
            try
            {
                lstResult = _dCPurchaseRepo.GetDCDrugDeatilsBySupplierAndDrug(SupplierID, FreeSearch);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult });
        }
    }
}