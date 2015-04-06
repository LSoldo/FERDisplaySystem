using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Model;

namespace DAL.Factories
{
    public class SceneFactory : ISceneFactory
    {
        public IScene GetScene(string type)
        {
            if(type == DataDefinition.SceneType.Video)
                return new Html5VideoScene();
            else if(type == DataDefinition.SceneType.Slideshow)
                return new ImageSlideshowScene();
            else if(type == DataDefinition.SceneType.Rss)
                return new RssScene();
            else if (type == DataDefinition.SceneType.Clock)
                return new ClockScene();
            else
                throw new ArgumentException("SceneFactory: Scene type argument not valid: {0}", type);
        }
    }
}
