using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using Newtonsoft.Json.Linq;

namespace DAL.Interfaces
{
    public class TimeInterval
    {
        public int Id { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
    }
    public interface ISequenceGenerator
    {
        int Id { get; set; }
        List<DataSource> Css { get; }
        List<DataSource> Js { get; }
        string JavascriptFunctions { get; }
        string Type { get; }
        bool IsInitialized { get; set; }
        string GenerateHtml(List<SequenceScene> scenes, string groupId, string terminalSequenceId);
        void Init();
    }

}
