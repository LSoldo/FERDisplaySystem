using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Utils;
using Newtonsoft.Json.Linq;

namespace DAL.Model
{
    public class Html5VideoScene : IScene
    {
        public int Id { get; set; }
        public string Type { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Urls { get; set; }
        public string HtmlContent { get; private set; }
        public string JavascriptFunctions { get; set; }
        public List<string> Css { get; set; }
        public List<string> Js { get; set; }
        public bool IsCacheable { get; set; }
        public bool IsInitialized { get; private set; }
        public void Init(string name, string description, List<string> urls, bool isCacheable)
        {
            this.Name = name;
            this.Description = description;
            this.Urls = urls;
            this.IsCacheable = isCacheable;
            this.Type = DataDefinition.SceneType.Video;
            this.IsInitialized = true;

            Calculate();
        }
        private void ClearData()
        {
            this.Css = null;
            this.Js = null;
            this.JavascriptFunctions = null;
            this.HtmlContent = "";
        }
        public void Calculate()
        {
            try
            {
                using (var r = new StreamReader(DataDefinition.SceneDefinition.Path))
                {
                    ClearData();
                    var builder = new PageBuilder();
                    var json = r.ReadToEnd();
                    dynamic definition = JObject.Parse(json);
                    this.HtmlContent = string.Format(string.Join("", definition.html5videoscene.html), this.Urls.FirstOrDefault());
                    this.JavascriptFunctions = string.Join(Environment.NewLine,
                        definition.html5videoscene.javascriptFunctions);
                    this.Css = definition.html5videoscene.css.ToObject<List<string>>();
                    this.Js = definition.html5videoscene.js.ToObject<List<string>>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured: " + ex.Message);
            }
            
        }
    }
}
