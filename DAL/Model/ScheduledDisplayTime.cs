using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL.Model
{
    public class ScheduledDisplayTime
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual Terminal Terminal { get; set; }
        [JsonIgnore]
        public virtual DigitalSign DigitalSign { get; set; }
        [JsonIgnore]
        public virtual DisplaySetting DisplaySetting { get; set; }
        public virtual List<TimeInterval> TimeIntervals { get; set; }
        [JsonIgnore]
        //for shutting down indefinite runs
        public bool Active { get; set; }
    }

    public class TimeInterval
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
