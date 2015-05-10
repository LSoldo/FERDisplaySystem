using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Model;

namespace DAL.Factories
{
    public class SceneGeneratorFactory : ISceneGeneratorFactory
    {
        public ISceneGenerator GetScene(string type)
        {
            if(type == DataDefinition.SceneType.Video)
                return new Html5VideoSceneGenerator();
            else if(type == DataDefinition.SceneType.Slideshow)
                return new ImageSlideshowSceneGenerator();
            else if(type == DataDefinition.SceneType.Rss)
                return new RssSceneGenerator();
            else if (type == DataDefinition.SceneType.Clock)
                return new ClockSceneGenerator();
            else
                throw new ArgumentException("SceneFactory: Scene type argument not valid: {0}", type);
        }
    }
}
