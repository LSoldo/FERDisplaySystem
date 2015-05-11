using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("ManualSequenceId")]
        public virtual TerminalSequence ManualSequence { get; set; }
        public int? ManualSequenceId { get; set; }
        [ForeignKey("DefaultSequenceId")]
        public virtual TerminalSequence DefaultSequence { get; set; }
        public int? DefaultSequenceId { get; set; }
        public virtual List<TerminalSequence> TerminalSequencePool { get; set; }

        public Terminal()
        {
            this.TerminalSequencePool = new List<TerminalSequence>();
            this.Active = false;
        }

    }

}
