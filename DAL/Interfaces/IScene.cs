﻿using System;
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
        string Name { get; set; }
        string Description { get; set; }
        List<string> Urls { get; set; }
        string HtmlContent { get; }
        List<string> Css { get; set; }
        List<string> Js { get; set; } 
        string JavascriptFunctions { get; }
        bool IsCacheable { get; }
        void Init(string name, string description, List<string> urls, TimeSpan duration, bool isCacheable);
        void Calculate();
    }
}