using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace DAL.Interfaces
{
    public interface IScene
    {
        int Id { get; set; }
        string Type { get; }       
        string HtmlContent { get; }
        List<string> Css { get; set; }
        List<string> Js { get; set; } 
        List<string> JavascriptFunctions { get; }
        bool IsCacheable { get; }
        bool IsInitialized { get; }
        void Init();
        void Calculate();
        string GenerateHtmlContent(List<string> urls);
    }
}
