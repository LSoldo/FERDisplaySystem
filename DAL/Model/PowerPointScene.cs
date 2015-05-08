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
    public class PowerPointScene : IScene
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
            this.IsCacheable = false;
            this.Type = DataDefinition.SceneType.PowerPoint;
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
                    this.HtmlContent = string.Join("", definition.powerpointscene.html);
                    this.JavascriptFunctions = (definition.powerpointscene.javascriptFunctions).ToObject<List<string>>();
                    this.Css = definition.powerpointscene.css.ToObject<List<string>>();
                    this.Js = definition.powerpointscene.js.ToObject<List<string>>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured: " + ex.StackTrace);
            }
        }

        public string GenerateHtmlContent(List<string> urls)
        {
            if(this.IsInitialized)
                return string.Format(this.HtmlContent, urls.FirstOrDefault());
            else
            {
                throw new Exception("Scene not initialized");
            }
        }
    }
}