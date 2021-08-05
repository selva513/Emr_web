using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class QuestionView
    {
        public long QuestionsSeqid { get; set; }
        public string QuestionName { get; set; }
        public bool IsActive { get; set; }
    }
}
