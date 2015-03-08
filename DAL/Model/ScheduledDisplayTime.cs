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
        public List<Terminal> Terminals { get; set; }
        public DigitalSign DigitalSign { get; set; }
        public DisplaySetting DisplaySetting { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? IndefiniteRunEveryNDays { get; set; }
        //for shutting down indefinite runs
        public bool Active { get; set; }
    }
}
