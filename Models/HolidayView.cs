using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class HolidayView
    {
        public int HolidayId { get; set; }
        public string HolidayName { get; set; }
        public string Content { get; set; }
        public DateTime HolidayDate { get; set; }
    }
}
