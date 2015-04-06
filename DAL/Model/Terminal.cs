using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL.Model
{
    public class Terminal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public TerminalSequence CurrentSequence { get; set; }
        public List<TerminalSequence> AllSequences { get; set; }
        public TerminalSequence ManualSequence { get; set; }
        public TerminalSequence DefaultSequence { get; set; }
        public DateTime CurrentSequenceFrom { get; set; }
        public DateTime CurrentSequenceTo { get; set; }
 
    }
}
