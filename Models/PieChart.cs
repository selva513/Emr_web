using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class PieChart
    {
        public string[] labels { get; set; }
        public List<PieDatasets> datasets { get; set; }
    }
    public class PieDatasets
    {
        public decimal[] data { get; set; }
        public string[] backgroundColor { get; set; }
    }
}
