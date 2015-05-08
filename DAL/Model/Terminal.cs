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
        public bool Active { get; set; }
        public virtual TerminalSequence CurrentSequence { get; set; }
        public virtual TerminalSequence ManualSequence { get; set; }
        public virtual TerminalSequence DefaultSequence { get; set; }
        public virtual List<TerminalSequence> AllSequences { get; set; }
        public virtual TimeInterval CurrentSequenceValidFromToInterval { get; set; }

        public Terminal()
        {
            this.AllSequences = new List<TerminalSequence>();
            this.Active = false;
        }

    }

}
