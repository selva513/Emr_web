using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class HISOrderListView
    {
        public int LIS_OrderlistID { get; set; }
        public int LIS_OrderId { get; set; }
        public int LIS_TestID { get; set; }
        public int Lis_Deptid { get; set; }
        public bool LIS_isOrderListActive { get; set; }
        public int LIS_InvoiceNo { get; set; }
        public string LIS_ordercreateDt { get; set; }
        public int LIS_UserCode { get; set; }
        public int LIS_LastModifyUsercode { get; set; }
        public bool LIS_ISStat { get; set; }
        public int Lis_Ordqty { get; set; }
        public decimal Lis_ordprice { get; set; }
        public decimal Lis_orddiscount { get; set; }
        public decimal Lis_ordtotal { get; set; }
        public bool LIS_SAMPLECANCELFLAG { get; set; }
        public string LIS_LocationID { get; set; }
        public string HIS_PerformingDoctorID { get; set; }
        public string HIS_RefferalDoctorID { get; set; }
        public decimal HIS_RefferalDoctorAmt { get; set; }
        public decimal HIS_PerformingDoctorAmt { get; set; }
        public string HIS_PackgeName { get; set; }
        public bool HIS_Radiology_Pending { get; set; }
        public bool HIS_Radiology_Collected { get; set; }
        public bool HIS_Radiology_Authenticate { get; set; }
        public bool HIS_Radiology_ReportinProgress { get; set; }
        public bool HIS_Radiology_Printed { get; set; }
        public int HIS_Template_ID { get; set; }
        public string HIS_Template_Name { get; set; }
        public string HIS_TEMPLATE_SUMMARY { get; set; }
    }
}
