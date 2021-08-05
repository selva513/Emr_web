using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class HISOrderView
    {
        public int LIS_OrderID { get; set; }
        public string LIS_OrderDT { get; set; }
        public string LIS_OrderPatID { get; set; }
        public int LIS_VisitID { get; set; }
        public int LIS_OrderType { get; set; }
        public string LIS_OrderDocCode { get; set; }
        public bool LIS_isOrderActive { get; set; }
        public string LIS_CreateDt { get; set; }
        public int LIS_USERCODE { get; set; }
        public int LIS_LastMODifyUsercode { get; set; }
        public bool LIS_PayFlag { get; set; }
        public decimal Lis_ord_Grandtotal { get; set; }
        public decimal Lis_ord_Granddiscount { get; set; }
        public string Lis_ord_Grandtotalfrom { get; set; }
        public string Lis_ord_Granddistype { get; set; }
        public decimal Lis_ord_nettotal { get; set; }
        public int LIS_InvoiceNo { get; set; }
        public string LIS_Finyear { get; set; }
        public string LIS_Paymentmode { get; set; }
        public bool LIS_isCombinedpayment { get; set; }
        public decimal LIS_CASHRECIEVEDAMT { get; set; }
        public decimal LIS_CCARDRECIEVEDAMT { get; set; }
        public decimal LIS_DCARDRECIEVEDAMT { get; set; }
        public decimal LIS_PENDINGTOPAY { get; set; }
        public decimal LIS_ADVANCEADJUSTED { get; set; }
        public int LIS_ADVANCEVOUCHERNO { get; set; }
        public string LIS_ADVANCEVOUCHERDT { get; set; }
        public string LIS_CCARDNO { get; set; }
        public string LIS_DBCARDNO { get; set; }
        public string LIS_CAUTHNO { get; set; }
        public string LIS_DAUTHNO { get; set; }
        public string LIS_CRISSUEBANK { get; set; }
        public string LIS_DBISSUEBANK { get; set; }
        public int LIS_CRISSUEBANKID { get; set; }
        public int LIS_DBISSUEBANKID { get; set; }
        public string LIS_SPLCOMMENT { get; set; }
        public string LIS_CHNAME { get; set; }
        public string LIS_CHTYPE { get; set; }
        public string LIS_DHNAME { get; set; }
        public string LIS_DHTYPE { get; set; }
        public string LIS_LocationID { get; set; }
        public decimal LIS_Currentpendingamount { get; set; }
        public decimal LIS_IsSentforAuthorise { get; set; }
        public int LIS_AuthoriseUserCode { get; set; }
        public string LIS_Due_Disc_Spl_Cmt { get; set; }
        public int LIS_PatAdmissionID { get; set; }
        public decimal HIS_OP_RefundAmt { get; set; }
        public decimal HIS_OP_Current_Refund { get; set; }
        public string HIS_Bill_Type { get; set; }
        public bool HIS_IS_THROUGHBANK { get; set; }
        public decimal HIS_THROUGHBANKAMT { get; set; }
        public string HIS_TRANSATIONID { get; set; }
        public bool HIS_ISCompanyClaim { get; set; }
        public string HIS_BatchID { get; set; }
        public string HIS_PAYMENTRECEIVEDATE { get; set; }
        public decimal HIS_PAYMENTRECEIVEAMOUNT { get; set; }
        public decimal HIS_WITHHELDAMOUNT { get; set; }
        public int HIS_GROUPID { get; set; }
        public string HIS_PANELCODE { get; set; }
        public string HIS_AGREEDATE { get; set; }
        public string HIS_AGREETIME { get; set; }
        public string HIS_SERFROMDATE { get; set; }
        public string HIS_SERTODATE { get; set; }
        public decimal HIS_CHEQUE_AMT { get; set; }
        public string HIS_CHEQUE_NO { get; set; }
        public string HIS_BANK_NAME { get; set; }
        public string HIS_CHEQUE_DATE { get; set; }
        public int HIS_CARD_DIGITS { get; set; }
        public int HIS_CARD_DIGITS1 { get; set; }
        public int HIS_RadioTotal_Count { get; set; }
        public int HIS_RadioPending_Count { get; set; }
        public int HIS_RadioCollected_Count { get; set; }
        public int HIS_RadioReportProgress_Count { get; set; }
        public int HIS_RadioReported_Count { get; set; }
        public string HIS_CORP_PONO { get; set; }
    }
}
