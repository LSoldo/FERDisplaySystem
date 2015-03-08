using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class ScheduledDisplayTime
    {
        public int Id { get; set; }
        public Terminal Terminal { get; set; }
        public DigitalSign DigitalSign { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IndefiniteRun { get; set; }
        //for shutting down indefinite runs
        public bool Active { get; set; }
    }
}
