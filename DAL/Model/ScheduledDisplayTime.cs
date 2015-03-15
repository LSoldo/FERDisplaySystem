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
        public Terminal Terminal { get; set; }
        [JsonIgnore]
        public DigitalSign DigitalSign { get; set; }
        [JsonIgnore]
        public DisplaySetting DisplaySetting { get; set; }
        [JsonProperty(PropertyName = "start")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "end")]
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public TimeSpan? IndefiniteRunEvery { get; set; }
        [JsonIgnore]
        //for shutting down indefinite runs
        public bool Active { get; set; }
    }
}
