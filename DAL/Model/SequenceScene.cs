using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL.Model
{
    public class SequenceScene
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IScene Scene { get; set; }
        public List<string> Urls { get; set; } 
        public SceneSetup Setup { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ShouldCleanCache { get; set; }

        public SequenceScene(IScene scene, List<string> urls, TimeSpan duration, bool cleanCache, string name)
        {
            this.Scene = scene;
            this.Urls = urls;
            this.Duration = duration;
            this.ShouldCleanCache = cleanCache;
            this.Name = name;
            this.Setup = MapToSceneSetup();
        }

        public SceneSetup MapToSceneSetup()
        {
            return new SceneSetup(this.Scene, this.Urls, this.Duration);
        }
    }

    public class SceneSetup
    {
        public string HtmlContent { get; set; }
        public long Interval { get; set; }
        public List<string> JsFunctionsToCall { get; set; }
        public List<string> JsPathList { get; set; }
        public List<string> CssPathList { get; set; }

        public SceneSetup(IScene scene, List<string> urls, TimeSpan duration)
        {
            this.HtmlContent = scene.GenerateHtmlContent(urls);
            this.Interval = (long)duration.TotalMilliseconds;
            this.JsFunctionsToCall = scene.JavascriptFunctions;
            this.JsPathList = scene.Js;
            this.CssPathList = scene.Css;
        }
    }
}
