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
using DAL.Model;

namespace DAL.Interfaces
{
    public interface ISceneGenerator
    {
        int Id { get; set; }
        string Type { get; }
        string HtmlContent { get; }
        List<DataSource> Css { get; set; }
        List<DataSource> Js { get; set; }
        List<JsCodeWrapper> JavascriptFunctions { get; }
        bool IsCacheable { get; }
        bool IsInitialized { get; }
        void Init();
        void Calculate();
        string GenerateHtmlContent(List<DataSource> urls);
    }
}
