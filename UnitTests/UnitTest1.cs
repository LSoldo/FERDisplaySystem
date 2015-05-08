using System;
using System.Collections.Generic;
using BL;
using DAL;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DisplaySettingsManagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DALTerminal dalTerminal = new DALTerminal();

            var manager = new BLDisplaySettingsManager();
            var sceneFactory = new SceneFactory();
            var sequenceFactory = new SequenceFactory();

            var clock = sceneFactory.GetScene(DataDefinition.SceneType.Clock);
            clock.Init();

            var sequenceScene = new List<SequenceScene>
            {
                new SequenceScene(clock, new List<DataSource>() {new DataSource(){Path=""}}, TimeSpan.FromMilliseconds(20000), false, "sat")
            };

            var sequence = sequenceFactory.GetSequence(DataDefinition.SequenceType.MaxImage);
            sequence.Init("Sekvenca", "opis", sequenceScene);
            sequence.Id = 1;

            DisplaySetting displaySetting;
            var times = manager.CreateDisplayTimes(DateTime.Now, DateTime.Now.AddSeconds(30), TimeSpan.FromMinutes(10),
                5,
                out displaySetting);

            var terminal = new Terminal {Name = "Aula 1", Type = DataDefinition.TerminalType.LED};

            var terminalSequence = new TerminalSequence(sequence, displaySetting, terminal.Id.ToString())
            {
                TimeIntervals = times,
                Name = "Video",
                CurrentType = DataDefinition.CurrentSequence.ScheduledSequence
            };


            terminal.DefaultSequence = terminalSequence;
            terminalSequence.CurrentType = DataDefinition.CurrentSequence.DefaultSequence;
            terminal.CurrentSequenceValidFromToInterval = new TimeInterval()
            {
                TimeFrom = DateTime.Now,
                TimeTo = DateTime.Now.AddSeconds(30)
            };
            terminal.AllSequences.Add(terminalSequence);

            int terminalId = dalTerminal.AddTerminal(terminal);
            dalTerminal.Dispose();
            Assert.IsTrue(terminalId >=0);
        }
    }
}
