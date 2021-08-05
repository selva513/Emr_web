using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class DrugMaster
    {
        public long SeqID { get; set; }
        public string DrugName { get; set; }
        public string Category { get; set; }
        public string Uom { get; set; }
        public decimal Gst { get; set; }
        public string ScheduleType { get; set; }
        public string HSnCode { get; set; }
        public string Company { get; set; }
        public string Type { get; set; }
        public IFormFile FilePatientDocment { get; set; }
    }
}
