using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class SequenceHistory
    {
        public Terminal Terminal { get; set; }
        public TerminalSequence Sequence { get; set; }
    }
}
