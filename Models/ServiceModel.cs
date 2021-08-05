using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceShortCode { get; set; }
        public int ServiceOrder { get; set; }
        public bool IsActive { get; set; }
        public string ServiceAlias { get; set; }
        public bool ServiceEditable { get; set; }
        public string HospitalID { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string TestShortCode { get; set; }
        public string TestAlias { get; set; }
        public bool TestIsActive { get; set; }
        public decimal TestRate { get; set; }
        public bool EnableQty { get; set; }
        public bool EnableRate { get; set; }
    }
}
