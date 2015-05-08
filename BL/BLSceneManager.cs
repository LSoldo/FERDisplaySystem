using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Model;

namespace BL
{
    public class BLSceneManager
    {
        public SequenceScene CreateScene(string sceneType, string sceneName, List<string> urls, TimeSpan duration, bool cleanCache)
        {
            ISceneFactory factory = new SceneFactory();
            IScene scene = factory.GetScene(sceneType);

            scene.Init();
            SequenceScene seqScene = new SequenceScene(scene, urls, duration, cleanCache, sceneName);
            return seqScene;
        }

        public ISequence CreateSequence(string sequenceType, string sequenceName, string sequenceDescription, List<SequenceScene> scenes)
        {
            ISequenceFactory factory = new SequenceFactory();
            ISequence sequence = factory.GetSequence(sequenceType);

            sequence.Init(sequenceName,sequenceDescription, scenes);
            return sequence;
        }
    }
}
