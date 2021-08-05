using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class HISPackageView
    {
        public long HIS_PackageID { get; set; }
        public string HIS_PACKAGENAME { get; set; }
        public string HIS_PACKAGEALIASNAME { get; set; }
        public bool HIS_PACKAGEACTIVE { get; set; }
        public string HIS_PACKAGETYPE { get; set; }
        public decimal HIS_PACKAGETOTALAMT { get; set; }
        public decimal HIS_PACKAGEDISC_AMOUNT { get; set; }
        public decimal HIS_PACKAGEDISC_PERC { get; set; }
        public int HIS_TestID { get; set; }
        public decimal HIS_TestAmt { get; set; }
    }
}
