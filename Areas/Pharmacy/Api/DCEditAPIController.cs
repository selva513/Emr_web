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

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/DCEdit")]
    [ApiController]
    public class DCEditAPIController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IDCEditRepo _dCEditRepo;
        private readonly IDCPurchaseRepo _dCPurchaseRepo;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        public DCEditAPIController(IDBConnection iDBConnection, IErrorlog errorlog,IDCEditRepo dCEditRepo,IDCPurchaseRepo dCPurchaseRepo,ICashBillRepo cashBillRepo, INewInvoiceRepo newInvoiceRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _dCEditRepo = dCEditRepo;
            _dCPurchaseRepo = dCPurchaseRepo;
            _cashBillRepo = cashBillRepo;
            _newInvoiceRepo = newInvoiceRepo;
        }
        [HttpGet("GetDcPurchaseByDcNumber")]
        public JsonResult GetDcPurchaseByDcNumber(string DcNumber)
        {
            List<DcHeaderInfo> lstResult = new List<DcHeaderInfo>();
            List<DcDeatilsInfo> lstDCDeatails = new List<DcDeatilsInfo>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _dCEditRepo.GetDcPurchaseBySupplierDCNum(HospitalId, DcNumber);
                if (lstResult.Count > 0)
                {
                    long SeqID = lstResult[0].PH_DC_SEQID;
                    lstDCDeatails = _dCPurchaseRepo.GetDcDrugDeatilsBySeqID(SeqID);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Header = lstResult, Deatils = lstDCDeatails });
        }
        [HttpPost("DcEditPurchaseSave")]
        public JsonResult DcEditPurchaseSave([FromBody] DcPurchaseInfo dcPurchaseInfo)
        {
            string StatusText = "";
            string storeName = dcPurchaseInfo.WareHouse;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            if (dcPurchaseInfo.DraftDeatils.Length > 0)
            {
                int CurrentStock = 0;
                for (int count=0;count< dcPurchaseInfo.DraftDeatils.Length; count++)
                {
                    string Status = dcPurchaseInfo.DraftDeatils[count].Status;
                    if (Status.Equals("DC"))
                    {
                        int DrugCode = dcPurchaseInfo.DraftDeatils[count].DrugCode;
                        string Batch = dcPurchaseInfo.DraftDeatils[count].OldBatch;
                        int CurrentQty = dcPurchaseInfo.DraftDeatils[count].Qty;
                        int OldQty = dcPurchaseInfo.DraftDeatils[count].OldStock;
                        int EditedQty = CurrentQty - OldQty;
                        string Remarks = "Drug Inward Correction";
                        
                        //int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks);
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
                            CurrentStock = currentStockTimes[0].PH_CUR_STOCK + EditedQty;
                            if(EditedQty<0)
                                Remarks = "Drug Inward Correction";
                            else
                                Remarks = "Drug Inward Edit";

                            DateTime ExDate = DateTime.Now;
                            DateTime RunningExpiryDate = GetDataformat(dcPurchaseInfo.DraftDeatils[count].ExpiryDate, ExDate);
                            string Exp = RunningExpiryDate.ToString("yyyy-MM-dd");
                            string NewBatch= dcPurchaseInfo.DraftDeatils[count].Batch;
                            decimal PurCost = dcPurchaseInfo.DraftDeatils[count].Rate;
                            decimal MRP = dcPurchaseInfo.DraftDeatils[count].MRP;
                            if (CurrentStock >= 0)
                            {
                                long StockEdit = _dCEditRepo.UpdateCurrentStocDCEdit(CurrentStock, storeName, DrugCode, Batch, Remarks, Exp, PurCost, MRP, NewBatch);
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
                                PH_CUR_STOCK = dcPurchaseInfo.DraftDeatils[count].Qty,
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
                                remarks = "Drug Inward Edit",
                                HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                                Reference = dcPurchaseInfo.SupplierDCNumber
                            };
                            long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                            if (CurrentsockInsert > 0)
                            {

                                long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                int Qty = dcPurchaseInfo.DraftDeatils[count].Qty;
                                CurrentStock = Qty;
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
                }
                if (CurrentStock >= 0)
                {
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
                        WareHouse = storeName,
                        HospitalID = HospitalId
                    };
                    long HeaderSeqID = _dCPurchaseRepo.UpdateDcHeaderByDCNumber(dcHeaderInfo);
                    if (HeaderSeqID > 0)
                    {
                        for (int count = 0; count < dcPurchaseInfo.DraftDeatils.Length; count++)
                        {
                            DateTime ExDate = DateTime.Now;
                            DateTime RunningExpiryDate = GetDataformat(dcPurchaseInfo.DraftDeatils[count].ExpiryDate, ExDate);
                            DcDeatilsInfo dcDeatilsInfo = new DcDeatilsInfo
                            {
                                PH_DCDTL_SEQID = dcPurchaseInfo.DraftDeatils[count].DtlSeqID,
                                PH_DC_SEQID = (int)HeaderSeqID,
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
                                DiscountPrec = dcPurchaseInfo.DraftDeatils[count].Discount
                            };
                            long dtlSeqID = _dCPurchaseRepo.UpdateDcDrugDeatilsBySeqID(dcDeatilsInfo);
                            StatusText = "Save Success";
                        }
                    }
                }
              
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
    }
}