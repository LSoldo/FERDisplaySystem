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
    }
}
