using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class ReportView
    {
        public long ReportSeqID { get; set; }
        public string Report_Name { get; set; }
        public bool IsActive { get; set; }
        public bool AllReport_IsActive { get; set; }
    }
}
