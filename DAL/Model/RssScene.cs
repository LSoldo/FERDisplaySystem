using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DAL.Interfaces;
using DAL.Utils;
using Newtonsoft.Json.Linq;

namespace DAL.Model
{
    public class RssScene : IScene
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

        public void Init(string name,
            string description,
            List<string> urls,
            bool isCacheable
            )
        {
            this.Name = name;
            this.Description = description;
            this.Urls = urls;
            this.IsCacheable = isCacheable;
            this.Type = DataDefinition.SceneType.Rss;
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
                    this.HtmlContent = string.Join("", definition.rssscene.html);
                    this.JavascriptFunctions = string.Join(Environment.NewLine,
                        definition.rssscene.javascriptFunctions);
                    this.Css = (definition.rssscene.css).ToObject<List<string>>();
                    this.Js = (definition.rssscene.js).ToObject<List<string>>();

                    var reader = XmlReader.Create(this.Urls.FirstOrDefault() ?? "");
                    var feed = SyndicationFeed.Load(reader);
                    reader.Close();

                    if (feed != null)
                    {
                        var rssContent = "";
                        var i = 0;
                        foreach (var item in feed.Items)
                        {
                            var subject = builder.AddH1(item.Title.Text);
                            var content = (subject + item.Summary.Text).Replace("\n", "").Replace("&nbsp", "&#160");

                            rssContent += builder.AddDivWithId(content,
                                DataDefinition.SequenceDefinition.RssSequenceDivId + i);
                            i++;
                        }
                        this.HtmlContent = string.Format(this.HtmlContent, rssContent);
                    }
                    else
                        throw new Exception("RSS feed not available");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured: " + ex.Message);
            }
        }
    }
}
