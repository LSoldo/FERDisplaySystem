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
        public List<string> URLs { get; set; }
        public TimeSpan Duration { get; set; }
        public string HtmlContent { get; private set; }
        public string JavascriptFunctions { get; set; }
        public List<string> Css { get; set; }
        public List<string> Js { get; set; }
        public bool IsCacheable { get; set; }
        public bool ShouldCleanCache { get; set; }

        public void Init(string name,
            string description,
            List<string> urls,
            TimeSpan duration,
            bool isCacheable,
            bool shouldCleanCache)
        {
            this.Name = name;
            this.Description = description;
            this.URLs = urls;
            this.Duration = duration;
            this.IsCacheable = isCacheable;
            this.ShouldCleanCache = shouldCleanCache;
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
            using (StreamReader r = new StreamReader(DataDefinition.SceneDefinition.Path))
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

                XmlReader reader = XmlReader.Create(this.URLs.FirstOrDefault());
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                string rssContent = "";
                int i = 0;
                foreach (SyndicationItem item in feed.Items)
                {
                    String subject = item.Title.Text;
                    rssContent += builder.AddDivWithId(item.Summary.Text.Replace("\n", "").Replace("&nbsp", "&#160"),
                        DataDefinition.SequenceDefinition.RssSequenceDivId + i);
                    i++;
                }
                this.HtmlContent = string.Format(this.HtmlContent, rssContent);
            }
        }
    }
}
