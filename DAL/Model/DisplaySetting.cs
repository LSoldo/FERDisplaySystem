using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class DisplaySetting
    {
        //public string DisplayPeriod { get; set; }
        public int Id { get; set; }
        public DateTime InsertionTs { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan DurationSpan { get; set; }
        public DateTime ValidUntil { get; set; }
        public TimeSpan? ShowEvery { get; set; }
        public int? ConsecutiveTimesToShow { get; set; }
    }
}
