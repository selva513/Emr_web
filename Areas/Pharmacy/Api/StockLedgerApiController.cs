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
using System.Globalization;
using System.Data;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Emr_web.Common;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockLedgerApiController : ControllerBase
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IStockLedgerRepo _stockLedgerRepo;
        private IConfiguration _configuration;

        public StockLedgerApiController(IDBConnection dBConnection, IErrorlog errorlog, IStockLedgerRepo stockLedgerRepo, IConfiguration configuration)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _stockLedgerRepo = stockLedgerRepo;
            _configuration = configuration;
        }
        [HttpGet("GetBatchno")]
        public DrugInfo[] GetBatchno(string Drugname, string StoreName, string UOM, string ItemShortCode)
        {
            List<DrugInfo> lstrole = new List<DrugInfo>();
            try
            {
                if (!string.IsNullOrWhiteSpace(Drugname))
                {
                    var EmptySearch = Drugname.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < EmptySearch.Length; i++)
                    {
                        if (i == 0)
                        {
                            Drugname = EmptySearch[0];
                        }
                        if (i == 1)
                        {
                            Drugname = EmptySearch[1];
                        }
                    }
                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstrole = _stockLedgerRepo.GetBatchno(Drugname, StoreName, UOM, ItemShortCode, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstrole.ToArray();
        }

        [HttpGet("GetLedgerData")]
        public string GetLedgerData(string StoreName, string BatchNumber, string FromDate, string ToDate)
        {
            DataSet dsResult = new DataSet();
            try
            {

                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DateTime FrDatetime = DateTime.Now;
                FromDate = CommonSetting.getDataformat(FromDate, FrDatetime).ToString("yyyy/MM/dd");

                DateTime ToDatetime = DateTime.Now;
                ToDate = CommonSetting.getDataformat(ToDate, ToDatetime).ToString("yyyy/MM/dd");

                DataTable dtDc = _stockLedgerRepo.getAllDcStockLedgerDetails(BatchNumber, FromDate, ToDate);
                DataTable dtRet = _stockLedgerRepo.getOpRetStockLedgerDetails(BatchNumber,FromDate, ToDate);
                dsResult = _stockLedgerRepo.getAllOPStockLedgerDetails(BatchNumber, FromDate, ToDate);
                DataTable dtResult = _stockLedgerRepo.OPStockLedgerSumByBatchNum(BatchNumber, FromDate, ToDate);
                DataTable dtRetSum = _stockLedgerRepo.getOpRetStockSum(BatchNumber, FromDate, ToDate);
                DataTable dtDcSum = _stockLedgerRepo.getAllDCSum(BatchNumber, FromDate, ToDate);
                DataTable dtTrnsfer = _stockLedgerRepo.getTransferDeatilsbyBatchNumber(BatchNumber, FromDate, ToDate);

                dsResult.Tables[0].TableName = "StockLedger";

                dsResult.Tables.Add(dtResult);
                dsResult.Tables[1].TableName = "StockLedgerSum";

                dsResult.Tables.Add(dtDc);
                dsResult.Tables[2].TableName = "DCStockLedger";

                dsResult.Tables.Add(dtRet);
                dsResult.Tables[3].TableName = "RetStockLedger";

                dsResult.Tables.Add(dtRetSum);
                dsResult.Tables[4].TableName = "RetStockSum";

                dsResult.Tables.Add(dtDcSum);
                dsResult.Tables[5].TableName = "DCStockSum";

                dsResult.Tables.Add(dtTrnsfer);
                dsResult.Tables[6].TableName = "TranStock";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return dsResult.GetXml();
        }
    }
}
