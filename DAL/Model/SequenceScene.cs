using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Factories;
using DAL.Interfaces;

namespace DAL.Model
{
    public class SequenceScene
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Scene Scene { get; set; }
        [NotMapped] 
        private SceneSetup sceneSetup;
        [NotMapped]
        public SceneSetup Setup
        {
            get { return sceneSetup ?? (sceneSetup = MapToSceneSetup()); }
            set { sceneSetup = value; }
        }
        public TimeSpan Duration { get; set; }
        public bool ShouldCleanCache { get; set; }

        protected SequenceScene() { }

        /// <summary>
        /// Constructor which generates scene setup
        /// </summary>
        /// <param name="sceneType"></param>
        /// <param name="urls"></param>
        /// <param name="duration"></param>
        /// <param name="cleanCache"></param>
        /// <param name="name"></param>
        public SequenceScene(Scene scene, TimeSpan duration, bool cleanCache)
        {
            this.Scene = scene;
            this.Duration = duration;
            this.ShouldCleanCache = cleanCache;
            this.Setup = MapToSceneSetup();
        }

        public SceneSetup MapToSceneSetup()
        {
            return new SceneSetup(this.Scene.SceneType, this.Scene.DataSources, this.Duration);
        }
    }

    public class SceneSetup
    {
        public int Id { get; set; }
        public string HtmlContent { get; set; }
        public long Interval { get; set; }
        public List<string> JsFunctionsToCall { get; set; }
        public List<string> JsPathList { get; set; }
        public List<string> CssPathList { get; set; }

        public SceneSetup(string sceneType, List<DataSource> urls, TimeSpan duration)
        {
            var scene = new SceneGeneratorFactory().GetScene(sceneType);
            scene.Init();
                
            this.HtmlContent = scene.GenerateHtmlContent(urls);
            this.Interval = (long)duration.TotalMilliseconds;
            this.JsFunctionsToCall = scene.JavascriptFunctions.Select(s => s.Code).ToList();
            this.JsPathList = scene.Js.Select(s => s.Path).ToList();
            this.CssPathList = scene.Css.Select(s => s.Path).ToList();
        }
    }
}
