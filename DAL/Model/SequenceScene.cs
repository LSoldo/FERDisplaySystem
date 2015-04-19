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
        public IScene Scene { get; set; }
        public SceneSetup Setup { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ShouldCleanCache { get; set; }

        public SequenceScene(IScene scene, TimeSpan duration, bool cleanCache)
        {
            this.Scene = scene;
            this.Duration = duration;
            this.ShouldCleanCache = cleanCache;

            this.Setup = MapToSceneSetup();
        }

        public SceneSetup MapToSceneSetup()
        {
            return new SceneSetup(this.Scene, this.Duration);
        }
    }

    public class SceneSetup
    {
        public string HtmlContent { get; set; }
        public long Interval { get; set; }
        public List<string> JsFunctionsToCall { get; set; }
        public List<string> JsPathList { get; set; }
        public List<string> CssPathList { get; set; }

        public SceneSetup(IScene scene, TimeSpan duration)
        {
            this.HtmlContent = scene.HtmlContent;
            this.Interval = (long)duration.TotalMilliseconds;
            this.JsFunctionsToCall = scene.JavascriptFunctions;
            this.JsPathList = scene.Js;
            this.CssPathList = scene.Css;
        }
    }
}
