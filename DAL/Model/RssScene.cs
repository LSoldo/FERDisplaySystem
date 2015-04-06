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
            using (var r = new StreamReader(DataDefinition.SceneDefinition.Path))
            {
                ClearData();
                PageBuilder builder = new PageBuilder();
                string json = r.ReadToEnd();
                dynamic definition = JObject.Parse(json);
                this.HtmlContent = string.Join(Environment.NewLine, definition.rssscene.html);
                this.JavascriptFunctions = string.Join(Environment.NewLine,
                    definition.rssscene.javascriptFunctions);
                this.Css = builder.AddCss((definition.rssscene.css).ToObject<List<string>>());
                this.Js = builder.AddJs((definition.rssscene.js).ToObject<List<string>>());

                var reader = XmlReader.Create(this.Urls.FirstOrDefault());
                var feed = SyndicationFeed.Load(reader);
                reader.Close();

                if (feed != null)
                {
                    var rssContent = "";
                    var i = 0;
                    foreach (var item in feed.Items)
                    {
                        var subject = item.Title.Text;
                        rssContent += builder.AddDivWithId(
                            item.Summary.Text.Replace("\n", "").Replace("&nbsp", "&#160"),
                            DataDefinition.SequenceDefinition.RssSequenceDivId + i);
                        i++;
                    }
                    this.HtmlContent = string.Format(this.HtmlContent, rssContent);
                }
                else
                    throw new Exception("RSS feed not available");
            }
        }
    }
}
