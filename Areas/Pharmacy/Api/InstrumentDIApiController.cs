using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentDIApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IInstrumentDIRepo _instrumentDIRepo;
        private IConfiguration _configuration;

        public InstrumentDIApiController(IDBConnection dBConnection, IErrorlog errorlog, IInstrumentDIRepo instrumentDIRepo, IConfiguration configuration)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _instrumentDIRepo = instrumentDIRepo;
            _configuration = configuration;
        }
        [HttpGet("GetInstrumentFromMaster")]
        public JsonResult GetInstrumentFromMaster(string SearchTearm)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalInstruments> lstResult = new List<SurgicalInstruments>();
            try
            {
                lstResult = _instrumentDIRepo.GetInstrumentFromMaster(SearchTearm, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpPost("InstrumentDraftSave")]
        public JsonResult InstrumentDraftSave([FromBody] InstrumentDraftInfo draftInfo)
        {
            DateTime SupDate = DateTime.Now;
            DateTime SupplierInvoiceDate = GetDataformat(draftInfo.SupplierInvoiceDate, SupDate);
            List<InstrumentDraftInfoHeader> lstHeader = new List<InstrumentDraftInfoHeader>();
            List<InstrumentDraftInfoDetails> lstDeatils = new List<InstrumentDraftInfoDetails>();
            List<DraftInvoiceSummary> lstSummary = new List<DraftInvoiceSummary>();
            List<InvoiceTotalSum> lstTotal = new List<InvoiceTotalSum>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                
                long HeaderSeqID = _instrumentDIRepo.IsExitsSupplierInvoiceNum(draftInfo.SupplierInvoiceNumber,HospitalId);
                if (HeaderSeqID > 0)
                {
                    long draftSeqID = _instrumentDIRepo.IsExitsDrugByInvoice(HeaderSeqID, draftInfo.InstrumentCode);
                    if (draftSeqID > 0)
                    {
                        long DelSeqID = _instrumentDIRepo.DeleteDraftDtlByDrugCode(draftInfo.InstrumentCode);
                        InstrumentDraftInfoDetails draftDeatils = new InstrumentDraftInfoDetails
                        {
                            DraftSeqID = HeaderSeqID,
                            Instrumentame = draftInfo.InstrumentName,
                            InstrumentCode = draftInfo.InstrumentCode,
                            ModelName = draftInfo.ModelName,
                            ModelNo = draftInfo.ModelNo,
                            CompanyName = draftInfo.CompanyName,
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _instrumentDIRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                    else
                    {
                        InstrumentDraftInfoDetails draftDeatils = new InstrumentDraftInfoDetails
                        {
                            DraftSeqID = HeaderSeqID,
                            Instrumentame = draftInfo.InstrumentName,
                            InstrumentCode = draftInfo.InstrumentCode,
                            ModelName = draftInfo.ModelName,
                            ModelNo = draftInfo.ModelNo,
                            CompanyName = draftInfo.CompanyName,
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _instrumentDIRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                }
                else
                {
                    InstrumentDraftInfoHeader draftInfoHeader = new InstrumentDraftInfoHeader
                    {
                        SupplierInvoiceNumber = draftInfo.SupplierInvoiceNumber,
                        SupplierID = draftInfo.SupplierID,
                        SupplierInvoiceDate = SupplierInvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        InVoiceTaxType = draftInfo.InVoiceTaxType,
                        InvoiceType = draftInfo.InvoiceType,
                        Amount = 0,
                        Tax = 0,
                        NetAmount = 0,
                        TaxPrecentage = 0,
                        Discount = 0,
                        WareHouse = draftInfo.WareHouse,
                        CreatedDatime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"))
                    };
                    HeaderSeqID = _instrumentDIRepo.CreateNewDraftHeader(draftInfoHeader);
                    if (HeaderSeqID > 0)
                    {
                        InstrumentDraftInfoDetails draftDeatils = new InstrumentDraftInfoDetails
                        {
                            DraftSeqID = HeaderSeqID,
                            Instrumentame = draftInfo.InstrumentName,
                            InstrumentCode = draftInfo.InstrumentCode,
                            ModelName = draftInfo.ModelName,
                            ModelNo = draftInfo.ModelNo,
                            CompanyName = draftInfo.CompanyName,
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _instrumentDIRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                }
                lstHeader = _instrumentDIRepo.GetDraftBillSeqID(HeaderSeqID, HospitalId);
                lstDeatils = _instrumentDIRepo.GetDraftDeatailsBySeqID(HeaderSeqID);
                lstSummary = _instrumentDIRepo.GetInvoiceBillGroupByTax(HeaderSeqID);
                lstTotal = _instrumentDIRepo.GetInvoiceTotalSumBySeID(HeaderSeqID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Deatils = lstDeatils, Summary = lstSummary, Total = lstTotal });
        }
        [HttpPost("InstrumentInvoiceSave")]
        public JsonResult InstrumentInvoiceSave([FromBody] InstrumentInvoiceInfo invoiceInfo)
        {
            string StatusText = "";
            string storeName = invoiceInfo.WareHouse;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            //long SupplierSeqID = _instrumentDIRepo.IsExitsSupplierInvoiceNum(invoiceInfo.SupplierInvoiceNumber,HospitalId);
            long InvoiceSeqID = _instrumentDIRepo.IsExistInvoiceNumInInvoiceHeader(invoiceInfo.SupplierInvoiceNumber, HospitalId); //Habib To Check Invoice No Exists In Invoice Header.
            if (InvoiceSeqID <= 0)
            {
                try
                {
                    if (invoiceInfo.DraftDeatils.Length > 0)
                    {
                        for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                        {
                            int DrugCode = invoiceInfo.DraftDeatils[count].InstrumentCode;
                            string Batch = invoiceInfo.DraftDeatils[count].ModelName;
                            string Modelno= invoiceInfo.DraftDeatils[count].ModelNo;
                            string Company = invoiceInfo.DraftDeatils[count].CompanyName;
                            List<CurrentStockTimeStamp> currentStockTimes = _instrumentDIRepo.GetTimeStampByModel(Batch, DrugCode, storeName);
                            if (currentStockTimes.Count > 0)
                            {
                                byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                                currentStockTimes = _instrumentDIRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                                if (currentStockTimes.Count > 0)
                                {
                                }
                                else
                                {
                                    currentStockTimes = _instrumentDIRepo.GetTimeStampByModel(Batch, DrugCode, storeName);
                                }
                                int Qty = invoiceInfo.DraftDeatils[count].Qty;
                                int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                                string Remarks = "DirectInvoice";
                                int StockUpdate = _instrumentDIRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks);
                                //DateTime ExDate = DateTime.Now;
                                //DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                InstrumentStockMovements stockMovement = new InstrumentStockMovements
                                {
                                    PH_RUN_PROCESSIDKEY = "Invoice",
                                    PH_RUN_STORENAME = storeName,
                                    PH_RUN_INSTRUMENTCODE = DrugCode,
                                    PH_RUN_STOCK_TRANSACTVALUE = Qty,
                                    PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                                    PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                                    PH_RUN_MODELNAME = Batch,
                                    PH_RUN_MODELNO = Modelno,
                                    PH_RUN_COMPANYNAME = Company,
                                    PH_RUN_DOC_HDRNO = 1,
                                    PH_RUN_DOC_DTLNO = 1,
                                    PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                                    PH_RUN_FINYEAR = "20-21",
                                    PH_RUN_TRANSACT_ISACTIVE = true,
                                    PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                                    PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                };
                                int StockMovementUpdate = _instrumentDIRepo.InsertStockMovement(stockMovement);
                            }
                            else
                            {
                                //DateTime ExDate = DateTime.Now;
                                //DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                InstrumentCurrentStockInfo currentStockInfo = new InstrumentCurrentStockInfo
                                {
                                    ItemID = invoiceInfo.DraftDeatils[count].InstrumentCode,
                                    CurrentStockQty = invoiceInfo.DraftDeatils[count].Qty,
                                    PurchasePrice = invoiceInfo.DraftDeatils[count].Rate,
                                    SellingPrice = invoiceInfo.DraftDeatils[count].MRP,
                                    WarehouseName = storeName,
                                    Remarks = "DirectInvoice",
                                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                                    CreatedUser = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                                    CreatedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    IsActive = true
                                };
                                long CurrentsockInsert = _instrumentDIRepo.CreateNewStock(currentStockInfo);
                                long CurrentStockID = _instrumentDIRepo.GetCurrentStockSeqID();
                                if (CurrentsockInsert > 0)
                                {

                                    //long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                    int Qty = invoiceInfo.DraftDeatils[count].Qty;
                                    int CurrentStock = Qty;
                                    
                                    InstrumentStockMovements stockMovement = new InstrumentStockMovements
                                    {
                                        PH_RUN_PROCESSIDKEY = "Invoice",
                                        PH_RUN_STORENAME = storeName,
                                        PH_RUN_INSTRUMENTCODE = DrugCode,
                                        PH_RUN_STOCK_TRANSACTVALUE = invoiceInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_AFTERTRANSACT = invoiceInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_LEFTOUTINBATCH = invoiceInfo.DraftDeatils[count].Qty,
                                        PH_RUN_MODELNAME = Batch,
                                        PH_RUN_MODELNO = Modelno,
                                        PH_RUN_COMPANYNAME = Company,
                                        PH_RUN_DOC_HDRNO = 1,
                                        PH_RUN_DOC_DTLNO = 1,
                                        PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                                        PH_RUN_FINYEAR = "20-21",
                                        PH_RUN_TRANSACT_ISACTIVE = true,
                                        PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                                        PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    };
                                    int StockMovementUpdate = _instrumentDIRepo.InsertStockMovement(stockMovement);
                                }
                            }
                        }
                        DateTime SupDate = DateTime.Now;
                        DateTime SupplierInvoiceDate = GetDataformat(invoiceInfo.SupplierInvoiceDate, SupDate);
                        InstrumentInvoiceHeader invoiceHeader = new InstrumentInvoiceHeader
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
                        long InvoiceHeaderSeqID = _instrumentDIRepo.CreateNewInvoiceHeader(invoiceHeader);
                        if (InvoiceHeaderSeqID > 0)
                        {
                            for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                            {
                                //DateTime ExDate = DateTime.Now;
                                //DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                InstrumentInvoiceDetail invoiceDeatils = new InstrumentInvoiceDetail
                                {
                                    PH_IN_SEQID = InvoiceHeaderSeqID,
                                    PH_INDTL_DOCNO = "",
                                    PH_INDTL_INSTRUMENTCODE = invoiceInfo.DraftDeatils[count].InstrumentCode,
                                    PH_INDTL_DRUGUOM = "",
                                    PH_INDTL_MODELNAME = invoiceInfo.DraftDeatils[count].ModelName,
                                    PH_INDTL_MODELNO = invoiceInfo.DraftDeatils[count].ModelNo,
                                    PH_INDTL_COMPANYNAME = invoiceInfo.DraftDeatils[count].CompanyName,
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
                                long InvoiceDtlSeqID = _instrumentDIRepo.NewInvoiceDeatils(invoiceDeatils);
                            }
                        }
                        StatusText = "Save Success";
                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                    StatusText = "Logical Error";
                    long DeletSuppInvoice = _instrumentDIRepo.DeleteSupplierInvoice(invoiceInfo.SupplierInvoiceNumber);
                }
            }
            else
            {
                StatusText = "Supplier Invoice Number Exists";
            }
            return Json(StatusText);
        }
        [HttpGet("GetTop50DraftBill")]
        public JsonResult GetTop50DraftBill()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<InstrumentDraftInfoHeader> lstResult = new List<InstrumentDraftInfoHeader>();
            try
            {
                lstResult = _instrumentDIRepo.GetTop50DraftBill(HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet("GetSelectedDraftBySeqID")]
        public JsonResult GetSelectedDraftBySeqID(long SeqID)
        {
            List<InstrumentDraftInfoHeader> lstHeader = new List<InstrumentDraftInfoHeader>();
            List<InstrumentDraftInfoDetails> lstDeatils = new List<InstrumentDraftInfoDetails>();
            List<DraftInvoiceSummary> lstSummary = new List<DraftInvoiceSummary>();
            List<InvoiceTotalSum> lstTotal = new List<InvoiceTotalSum>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHeader = _instrumentDIRepo.GetDraftBillSeqID(SeqID,HospitalId);
                lstDeatils = _instrumentDIRepo.GetDraftDeatailsBySeqID(SeqID);
                lstSummary = _instrumentDIRepo.GetInvoiceBillGroupByTax(SeqID);
                lstTotal = _instrumentDIRepo.GetInvoiceTotalSumBySeID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Header = lstHeader, Deatils = lstDeatils, Summary = lstSummary, Total = lstTotal });
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
