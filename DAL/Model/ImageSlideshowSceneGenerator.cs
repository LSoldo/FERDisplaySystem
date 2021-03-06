﻿using System;
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
    public class ImageSlideshowSceneGenerator : ISceneGenerator
    {
        public int Id { get; set; }
        public string Type { get; private set; }
        public string HtmlContent { get; private set; }
        public virtual List<JsCodeWrapper> JavascriptFunctions { get; set; }
        public virtual List<DataSource> Css { get; set; }
        public virtual List<DataSource> Js { get; set; }
        public bool IsCacheable { get; set; }
        public bool IsInitialized { get; private set; }
        public void Init()
        {
            this.IsCacheable = true;
            this.Type = DataDefinition.SceneType.Slideshow;
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
                    this.HtmlContent = string.Join("", definition.imageslideshow.html);

                    this.JavascriptFunctions =
                        TypeConverter.ConvertToJsCodeWrapper((definition.imageslideshow.javascriptFunctions).ToObject<List<string>>());
                    this.Css = TypeConverter.ConvertToDataSource((definition.imageslideshow.css).ToObject<List<string>>());
                    this.Js = TypeConverter.ConvertToDataSource((definition.imageslideshow.js).ToObject<List<string>>());
                }
            }
            catch (Exception ex)
            {  
                throw new Exception("Exception occured: " + ex.Message);
            }          
        }

        public string GenerateHtmlContent(List<DataSource> urls)
        {
            if (!this.IsInitialized)
                Init();

            var builder = new PageBuilder();
            return string.Format(
                this.HtmlContent,
                string.Join("", builder.AddImg(urls.Select(s => s.Path).ToList())));

        }
    }
}
