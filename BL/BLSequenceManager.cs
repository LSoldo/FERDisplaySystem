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
    public class BLSequenceManager
    {
        private BLDisplaySettingsManager settingsManager;

        public BLSequenceManager()
        {
            this.settingsManager = new BLDisplaySettingsManager();
        }
        public SequenceScene CreateScene(string sceneType, string sceneName, List<string> urls, TimeSpan duration, bool cleanCache)
        {
            ISceneFactory factory = new SceneFactory();
            IScene scene = factory.GetScene(sceneType);
            scene.Init();

            SequenceScene seqScene = null;
            if (sceneType == DataDefinition.SceneType.PowerPoint)
            {
                var converter = new PowerPointConverter();
                //TODO add output path
                var convertedUrl = converter.GetVideoFromPpt(urls.FirstOrDefault(), "output");
                seqScene = new SequenceScene(scene, new List<string>(){convertedUrl}, duration, cleanCache, sceneName);
            }
            else
            {
                seqScene = new SequenceScene(scene, urls, duration, cleanCache, sceneName);
            }
             
            return seqScene;
        }

        public ISequence CreateSequence(string sequenceType, string sequenceName, string sequenceDescription, List<SequenceScene> scenes)
        {
            ISequenceFactory factory = new SequenceFactory();
            ISequence sequence = factory.GetSequence(sequenceType);

            sequence.Init(sequenceName,sequenceDescription, scenes);
            return sequence;
        }

        public TerminalSequence CreateTerminalSequence(Terminal terminal, ISequence sequence, DisplaySetting setting)
        {
            var terminalSequence = new TerminalSequence(sequence, setting, terminal.Id.ToString());
            var times = settingsManager.CreateDisplayTimes(setting);
            terminalSequence.TimeIntervals = times;

            return terminalSequence;
        }
    }
}
