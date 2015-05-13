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

        public void AddScene_Success()
        {
            DALScene dalScene = new DALScene();

            Scene scene = new Scene("Nova scena", DataDefinition.SceneType.Video,
                new List<DataSource>()
                {
                    new DataSource() {Path = "http://www.google.hr"},
                    new DataSource() {Path = "http://www.youtube.com"}
                });

            int id = dalScene.AddScene(scene);
            dalScene.Dispose();
            Assert.IsTrue(id > 0);
        }

        [TestMethod]

        public void EditScene_Success()
        {
            DALScene dalScene = new DALScene();

            var list = new List<DataSource>()
            {
                new DataSource() {Path = "http://www.yahoo.hr"},
                new DataSource() {Path = "http://www.net.com"}
            };

            var scene = new Scene("moja scena", DataDefinition.SceneType.Slideshow, list) {Description = "Desc", Id = 1};

            dalScene.UpdateScene(scene);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeactivateScene_Success()
        {
            DALScene dalScene = new DALScene();

            dalScene.SetSceneActiveProperty(1, false);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetScene_Success()
        {
            DALScene dalScene = new DALScene();

            var s1 = dalScene.GetSceneByActiveProperty(false);
            var s2 = dalScene.GetSceneByType(DataDefinition.SceneType.Slideshow);
            var s3 = dalScene.GetSceneByType(DataDefinition.SceneType.Clock);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]

        public void AddSequenceScene_Success()
        {
            DALScene dalScene = new DALScene();

            var scene = dalScene.GetSceneById(1);

            var seqScene = new SequenceScene(scene, TimeSpan.FromHours(2), true);
            int id = dalScene.AddSequenceScene(seqScene);
            dalScene.Dispose();
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void UpdateSequenceScene_Success()
        {
            DALScene dalScene = new DALScene();

            var seqScene = dalScene.GetSequenceSceneById(1);
            var scene = dalScene.GetSceneById(2);
            seqScene.Scene = scene;
            seqScene.Name = "My name";
            dalScene.UpdateSequenceScene(seqScene);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeleteSequenceScene_Success()
        {
            DALScene dalScene = new DALScene();

            dalScene.RemoveSequenceScene(1);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetSequenceSceneBySceneType_Success()
        {
            var dalScene = new DALScene();

            var scene = dalScene.GetSequenceSceneBySceneType(DataDefinition.SceneType.Slideshow);
            dalScene.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void AddSequence_Success()
        {
            var dalSequence = new DALSequence();
            var dalScene = new DALScene();

            var scene = dalScene.GetSceneById(1);
            var seqScene = new SequenceScene(scene, TimeSpan.FromHours(2), true);
            var seqScene2 = new SequenceScene(scene, TimeSpan.FromMinutes(10), true);
            var seqList = new List<SequenceScene>() {seqScene, seqScene2};

            var sequence = new Sequence("Sekvenca za pamćenje", DataDefinition.SequenceType.MaxImage, seqList);
            int id = dalSequence.AddSequence(sequence);
            dalScene.Dispose();
            dalSequence.Dispose();
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void RemoveSequence_Success()
        {
            var dalSequence = new DALSequence();
            var seq = dalSequence.GetSequenceById(3);
            dalSequence.RemoveSequence(seq);
            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UpdateSequence_Success()
        {
            var dalSequence = new DALSequence();
            var seq = dalSequence.GetSequenceById(1);
            seq.Name = "Moja sekvenca";
            dalSequence.UpdateSequence(seq);
            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void RemoveSequence2_Success()
        {
            var dalSequence = new DALSequence();
            var seq = dalSequence.GetSequenceById(2);
            dalSequence.RemoveSequence(seq);
            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void AddTerminalSequence_Success()
        {
            var dalSequence = new DALSequence();
            var seq = dalSequence.GetSequenceById(1);

            var termSeq = new TerminalSequence(seq,
                new DisplaySetting()
                {
                    ConsecutiveTimesToShow = 5,
                    DurationSpan = TimeSpan.FromHours(2),
                    InsertionTs = DateTime.Now,
                    StartTime = DateTime.Now,
                    ValidUntil = DateTime.Now
                }, "4");
            int id = dalSequence.AddTerminalSequence(termSeq);
            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void RemoveTerminalSequence_Success()
        {
            var dalSequence = new DALSequence();
            dalSequence.RemoveTerminalSequence(1);

            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UpdateTerminalSequence_Success()
        {
            var dalSequence = new DALSequence();
            var termSeq = dalSequence.GetTerminalSequenceById(2);
            termSeq.Active = false;
            termSeq.CurrentType = DataDefinition.CurrentSequence.ManualSequence;

            dalSequence.UpdateTerminalSequence(termSeq);

            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void AddTerminal_Success()
        {
            var dalSequence = new DALSequence();
            var dalTerminal = new DALTerminal();
            //var termSeq = dalSequence.GetTerminalSequenceById(1);
            var term = new Terminal {Name = "Moj terminal", Active = true, Type = DataDefinition.TerminalType.CRT};
            var seq = dalSequence.GetSequenceById(1);
            var termSeq = new TerminalSequence(seq, new DisplaySetting()
            {
                ConsecutiveTimesToShow = 5,
                DurationSpan = TimeSpan.FromHours(2),
                InsertionTs = DateTime.Now,
                StartTime = DateTime.Now,
                ValidUntil = DateTime.Now
            }, "4");
            //term.ManualSequence = termSeq;
            term.TerminalSequencePool.Add(termSeq);
            dalTerminal.AddTerminal(term);

            dalSequence.Dispose();
            dalTerminal.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetTerminal_Success()
        {
            var dalTerminal = new DALTerminal();
            var terminal = dalTerminal.GetTerminalById(4);
            dalTerminal.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeleteTerminal_Success()
        {
            var dalTerminal = new DALTerminal();
            dalTerminal.RemoveTerminal(4);
            dalTerminal.Dispose();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UpdateTerminal_Success()
        {
            
            var dalSequence = new DALSequence();
            var dalTerminal = new DALTerminal(dalSequence.GetContext());
            var seq = dalSequence.GetSequenceById(2);
            var termSeq = new TerminalSequence(seq, new DisplaySetting()
            {
                ConsecutiveTimesToShow = 5,
                DurationSpan = TimeSpan.FromHours(2),
                InsertionTs = DateTime.Now,
                StartTime = DateTime.Now,
                ValidUntil = DateTime.Now
            }, "4");
            dalTerminal.UpdateOrAddTerminalSequence(3, termSeq, DataDefinition.CurrentSequence.ScheduledSequence);
            dalTerminal.Dispose();
            dalSequence.Dispose();
            Assert.IsTrue(true);
        }

    }
}
