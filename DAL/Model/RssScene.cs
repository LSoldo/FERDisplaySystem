﻿using System;
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
        public string HtmlContent { get; private set; }
        public List<string> JavascriptFunctions { get; set; }
        public List<string> Css { get; set; }
        public List<string> Js { get; set; }
        public bool IsCacheable { get; set; }
        public bool IsInitialized { get; private set; }

        public void Init()
        {
            this.IsCacheable = true;
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
                   
                    var json = r.ReadToEnd();
                    dynamic definition = JObject.Parse(json);
                    this.HtmlContent = string.Join("", definition.rssscene.html);
                    this.JavascriptFunctions = (definition.rssscene.javascriptFunctions).ToObject<List<string>>();
                    this.Css = (definition.rssscene.css).ToObject<List<string>>();
                    this.Js = (definition.rssscene.js).ToObject<List<string>>();                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured: " + ex.Message);
            }
        }

        public string GenerateHtmlContent(List<string> urls)
        {
            if (this.IsInitialized)
            {
                var builder = new PageBuilder();
                var reader = XmlReader.Create(urls.FirstOrDefault() ?? "");
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
                    return string.Format(this.HtmlContent, rssContent);
                }
                else
                    throw new Exception("RSS feed not available");
            }
            else
            {
                throw new Exception("Scene is not initialized");
            }
        }
    }
}
