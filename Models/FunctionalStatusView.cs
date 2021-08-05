using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class FunctionalStatusView
    {
        public string PickListName { get; set; }
        public FunPickListDetails[] FunStatsuDtl { get; set; }
    }
}
