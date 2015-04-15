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
    public interface ISequence
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; }
        List<string> Css { get; }
        List<string> Js { get; }
        string JavascriptFunctions { get; }
        string Type { get; }
        List<SequenceScene> Scenes { get; set; }
        string GenerateHtml(string groupId);
        void Add(SequenceScene scene);
        void Init(string name, string description, List<SequenceScene> scenes);
    }

}
