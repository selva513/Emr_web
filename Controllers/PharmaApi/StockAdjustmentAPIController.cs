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
using Emr_web.Common;
using Microsoft.Extensions.Logging;

namespace Emr_web.Controllers.PharmaApi
{
    [Produces("application/json")]
    [Route("Pharma/StockAdjust")]
    public class StockAdjustmentAPIController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IMedicineCostingRepo _medicineCostingRepo;

        public StockAdjustmentAPIController(IDBConnection iDBConnection, IErrorlog errorlog,IMedicineCostingRepo medicineCostingRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _medicineCostingRepo = medicineCostingRepo;
        }
        [HttpGet("GetCurrentStockByDrugCodeByCosting")]
        public JsonResult GetCurrentStockByDrugCodeByCosting(int DrugCode)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugInfo> lstResult = new List<DrugInfo>();
            try
            {
                lstResult = _medicineCostingRepo.GetCurrentStockByDrugCodeByCosting(DrugCode, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("SaveStockAdjustment")]
        public JsonResult SaveStockAdjustment([FromBody] StockAdjustmentInfo stockAdjustmentInfo)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            string StatusText = "";
            try
            {
                int DrugCode = stockAdjustmentInfo.DrugCode;
                string Batch = stockAdjustmentInfo.BeforeBatch;
                string Store = stockAdjustmentInfo.StoreName;
                long IsExistDrug = _medicineCostingRepo.IsStockAdjustmentExists(DrugCode, Batch, Store);
                bool IsApproved = stockAdjustmentInfo.IsApproved;
                if (IsExistDrug > 0)
                {
                    if (!IsApproved)
                    {
                        StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader
                        {
                            SeqID=IsExistDrug,
                            ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsApproved = stockAdjustmentInfo.IsApproved,
                            Status = "Not Approved"
                        };
                        long HeaderSeqID = _medicineCostingRepo.UpdateStockAdjustmentHeaderBySeqID(stockAdjustmentHeader);
                        if (HeaderSeqID > 0)
                        {
                            long DeleteSeqID = _medicineCostingRepo.DeleteStockadjustmentBySeqID(IsExistDrug);
                            if(DeleteSeqID>0)
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime BeforeExpiryDate = GetDataformat(stockAdjustmentInfo.BeforeExpiry, ExDate);
                                DateTime AfterExpiryDate = GetDataformat(stockAdjustmentInfo.AfterExpiry, ExDate);
                                StockAdjustmentDetails stockAdjustmentDetails = new StockAdjustmentDetails
                                {
                                    HeaderSeqID = stockAdjustmentHeader.SeqID,
                                    DrugCode = stockAdjustmentInfo.DrugCode,
                                    BeforeBatch = stockAdjustmentInfo.BeforeBatch,
                                    BeforeExpiry = BeforeExpiryDate.ToString("yyyy-MM-dd"),
                                    BeforeCost = stockAdjustmentInfo.BeforeCost,
                                    BeforeStock = stockAdjustmentInfo.BeforeStock,
                                    BeforeMRP = stockAdjustmentInfo.BeforeMRP,
                                    AfterBatch = stockAdjustmentInfo.AfterBatch,
                                    AfterExpiry = AfterExpiryDate.ToString("yyyy-MM-dd"),
                                    AfterStock = stockAdjustmentInfo.AfterStock,
                                    AfterCost = stockAdjustmentInfo.AfterCost,
                                    AfterMRP = stockAdjustmentInfo.AfterMRP,
                                    StoreName = stockAdjustmentInfo.StoreName
                                };
                                long DtlResult = _medicineCostingRepo.CreateNewStockAdjustmentDtl(stockAdjustmentDetails);
                                if (DtlResult > 0)
                                    StatusText = "Save Success";
                            }
                        }
                    }
                    else
                    {
                        StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader
                        {
                            SeqID = IsExistDrug,
                            ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsApproved = stockAdjustmentInfo.IsApproved,
                            Status = "Approved"
                        };
                        long HeaderSeqID = _medicineCostingRepo.UpdateStockAdjustmentHeaderBySeqID(stockAdjustmentHeader);
                        if (HeaderSeqID > 0)
                        {
                            long DeleteSeqID = _medicineCostingRepo.DeleteStockadjustmentBySeqID(IsExistDrug);
                            if (DeleteSeqID > 0)
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime BeforeExpiryDate = GetDataformat(stockAdjustmentInfo.BeforeExpiry, ExDate);
                                DateTime AfterExpiryDate = GetDataformat(stockAdjustmentInfo.AfterExpiry, ExDate);
                                StockAdjustmentDetails stockAdjustmentDetails = new StockAdjustmentDetails
                                {
                                    HeaderSeqID = stockAdjustmentHeader.SeqID,
                                    DrugCode = stockAdjustmentInfo.DrugCode,
                                    BeforeBatch = stockAdjustmentInfo.BeforeBatch,
                                    BeforeExpiry = BeforeExpiryDate.ToString("yyyy-MM-dd"),
                                    BeforeCost = stockAdjustmentInfo.BeforeCost,
                                    BeforeStock = stockAdjustmentInfo.BeforeStock,
                                    BeforeMRP = stockAdjustmentInfo.BeforeMRP,
                                    AfterBatch = stockAdjustmentInfo.AfterBatch,
                                    AfterExpiry = AfterExpiryDate.ToString("yyyy-MM-dd"),
                                    AfterStock = stockAdjustmentInfo.AfterStock,
                                    AfterCost = stockAdjustmentInfo.AfterCost,
                                    AfterMRP = stockAdjustmentInfo.AfterMRP,
                                    StoreName = stockAdjustmentInfo.StoreName
                                };
                                long DtlResult = _medicineCostingRepo.CreateNewStockAdjustmentDtl(stockAdjustmentDetails);
                                if (DtlResult > 0)
                                {
                                    DrugCode = stockAdjustmentInfo.DrugCode;
                                    string CurBatch = stockAdjustmentDetails.BeforeBatch;
                                    string ChBatch = stockAdjustmentDetails.AfterBatch;
                                    string Expiry = stockAdjustmentDetails.AfterExpiry;
                                    decimal Cost = stockAdjustmentDetails.AfterCost;
                                    decimal MRP = stockAdjustmentDetails.AfterMRP;
                                    int Qty = stockAdjustmentDetails.AfterStock;
                                    Store = stockAdjustmentDetails.StoreName;
                                    string Remarks = "Stockadjustment";
                                    long Result = _medicineCostingRepo.UpdateCurrentStockCosting(DrugCode, CurBatch, ChBatch, Expiry, Qty, Cost, MRP, Store, Remarks);
                                    if (Result > 0)
                                        StatusText = "Save Success";
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!IsApproved)
                    {
                        StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader
                        {
                            RefNumber = CommonSetting.GetPurchaseOrderNumber(),
                            CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsApproved = stockAdjustmentInfo.IsApproved,
                            Status = "Not Approved"
                        };
                        long HeaderSeqID = _medicineCostingRepo.CreateNewStockAdjustment(stockAdjustmentHeader);
                        if (HeaderSeqID > 0)
                        {
                            DateTime ExDate = DateTime.Now;
                            DateTime BeforeExpiryDate = GetDataformat(stockAdjustmentInfo.BeforeExpiry, ExDate);
                            DateTime AfterExpiryDate = GetDataformat(stockAdjustmentInfo.AfterExpiry, ExDate);
                            StockAdjustmentDetails stockAdjustmentDetails = new StockAdjustmentDetails
                            {
                                HeaderSeqID = stockAdjustmentHeader.SeqID,
                                DrugCode = stockAdjustmentInfo.DrugCode,
                                BeforeBatch = stockAdjustmentInfo.BeforeBatch,
                                BeforeExpiry = BeforeExpiryDate.ToString("yyyy-MM-dd"),
                                BeforeCost = stockAdjustmentInfo.BeforeCost,
                                BeforeStock = stockAdjustmentInfo.BeforeStock,
                                BeforeMRP = stockAdjustmentInfo.BeforeMRP,
                                AfterBatch = stockAdjustmentInfo.AfterBatch,
                                AfterExpiry = AfterExpiryDate.ToString("yyyy-MM-dd"),
                                AfterStock = stockAdjustmentInfo.AfterStock,
                                AfterCost = stockAdjustmentInfo.AfterCost,
                                AfterMRP = stockAdjustmentInfo.AfterMRP,
                                StoreName = stockAdjustmentInfo.StoreName
                            };
                            long DtlResult = _medicineCostingRepo.CreateNewStockAdjustmentDtl(stockAdjustmentDetails);
                            if (DtlResult > 0)
                                StatusText = "Save Success";
                        }
                    }
                    else
                    {
                        StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader
                        {
                            RefNumber = CommonSetting.GetPurchaseOrderNumber(),
                            CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsApproved = stockAdjustmentInfo.IsApproved,
                            Status = "Approved"
                        };
                        long HeaderSeqID = _medicineCostingRepo.CreateNewStockAdjustment(stockAdjustmentHeader);
                        if (HeaderSeqID > 0)
                        {
                            DateTime ExDate = DateTime.Now;
                            DateTime BeforeExpiryDate = GetDataformat(stockAdjustmentInfo.BeforeExpiry, ExDate);
                            DateTime AfterExpiryDate = GetDataformat(stockAdjustmentInfo.AfterExpiry, ExDate);
                            StockAdjustmentDetails stockAdjustmentDetails = new StockAdjustmentDetails
                            {
                                HeaderSeqID = stockAdjustmentHeader.SeqID,
                                DrugCode = stockAdjustmentInfo.DrugCode,
                                BeforeBatch = stockAdjustmentInfo.BeforeBatch,
                                BeforeExpiry = BeforeExpiryDate.ToString("yyyy-MM-dd"),
                                BeforeCost = stockAdjustmentInfo.BeforeCost,
                                BeforeStock = stockAdjustmentInfo.BeforeStock,
                                BeforeMRP = stockAdjustmentInfo.BeforeMRP,
                                AfterBatch = stockAdjustmentInfo.AfterBatch,
                                AfterExpiry = AfterExpiryDate.ToString("yyyy-MM-dd"),
                                AfterStock = stockAdjustmentInfo.AfterStock,
                                AfterCost = stockAdjustmentInfo.AfterCost,
                                AfterMRP = stockAdjustmentInfo.AfterMRP,
                                StoreName = stockAdjustmentInfo.StoreName
                            };
                            long DtlResult = _medicineCostingRepo.CreateNewStockAdjustmentDtl(stockAdjustmentDetails);
                            if (DtlResult > 0)
                            {
                                 DrugCode = stockAdjustmentInfo.DrugCode;
                                string CurBatch = stockAdjustmentDetails.BeforeBatch;
                                string ChBatch = stockAdjustmentDetails.AfterBatch;
                                string Expiry = stockAdjustmentDetails.AfterExpiry;
                                decimal Cost = stockAdjustmentDetails.AfterCost;
                                decimal MRP = stockAdjustmentDetails.AfterMRP;
                                int Qty = stockAdjustmentDetails.AfterStock;
                                Store = stockAdjustmentDetails.StoreName;
                                string Remarks = "Stockadjustment";
                                long Result = _medicineCostingRepo.UpdateCurrentStockCosting(DrugCode, CurBatch, ChBatch, Expiry, Qty, Cost, MRP, Store, Remarks);
                                if (Result > 0)
                                    StatusText = "Save Success";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
                StatusText = "Logical Error";
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

        [HttpGet("GetStockAdjustment")]
        public JsonResult GetStockAdjustment()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<StockAdjustmentInfo> lstResult = new List<StockAdjustmentInfo>();
            try
            {
                lstResult = _medicineCostingRepo.GetStockAdjustment(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
    }
}