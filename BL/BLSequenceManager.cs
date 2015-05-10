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
        public SequenceScene CreateScene(string sceneType, string sceneName, List<DataSource> urls, TimeSpan duration, bool cleanCache)
        {
            SequenceScene seqScene = null;
            if (sceneType == DataDefinition.SceneType.PowerPoint)
            {
                var converter = new PowerPointConverter();
                //TODO add output path
                var convertedUrl = new DataSource()
                {
                    Path = converter.GetVideoFromPpt(urls.FirstOrDefault() == null ? "" : urls.FirstOrDefault().Path, "output")
                };
                seqScene = new SequenceScene(new Scene(sceneName, sceneType, new List<DataSource>() { convertedUrl }) , duration, cleanCache );
            }
            else
            {
                seqScene = new SequenceScene(new Scene(sceneName, sceneType, urls), duration, cleanCache);
            }
             
            return seqScene;
        }

        public TerminalSequence CreateTerminalSequence(Terminal terminal, Sequence sequence, DisplaySetting setting)
        {
            var terminalSequence = new TerminalSequence(sequence, setting, terminal.Id.ToString());
            var times = settingsManager.CreateDisplayTimes(setting);
            terminalSequence.TimeIntervals = times;

            return terminalSequence;
        }
    }
}
