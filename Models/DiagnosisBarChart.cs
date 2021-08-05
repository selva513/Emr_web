using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class DiagnosisBarChart
    {
        public string[] labels { get; set; }
        public decimal[] Total { get; set; }
        public List<DiagnosisBarDatasets> datasets { get; set; }
    }
    public class DiagnosisBarDatasets
    {
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public bool pointRadius { get; set; }
        public string pointColor { get; set; }
        public string pointStrokeColor { get; set; }
        public string pointHighlightFill { get; set; }
        public string pointHighlightStroke { get; set; }
        public decimal[] data { get; set; }
    }
}
