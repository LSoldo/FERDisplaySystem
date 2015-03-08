using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL;
using DAL;
using DAL.Model;

namespace UnitTests
{
    [TestClass]
    public class DisplaySettingsTest
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_StartTimeBeforeNow_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>(){new Terminal(){Name="First terminal"}};

            manager.CreateDisplaySettingForDigitalSign(terminals, DateTime.Today.AddDays(-1), TimeSpan.FromDays(1), null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_TimeSpanLessThan3Minutes_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };

            manager.CreateDisplaySettingForDigitalSign(terminals, DateTime.Today, TimeSpan.FromMinutes(2), null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_NoTerminals_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>();

            manager.CreateDisplaySettingForDigitalSign(terminals, DateTime.Today, TimeSpan.FromMinutes(10), null, null, null);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_ConsecutiveNumberOfDaysIsZero_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            var time = DateTime.Now;

            var setting = manager.CreateDisplaySettingForDigitalSign(terminals, time, TimeSpan.FromHours(23), null, 0, null);
        }

        [TestMethod]
        public void CreateDisplaySetting_ConsecutiveNumberOfDays_Equal()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            var time = DateTime.Now;

            var setting = manager.CreateDisplaySettingForDigitalSign(terminals, time, TimeSpan.FromHours(25), null, 4, null);
            //Adding only 6 days because current day is also counted
            Assert.AreEqual(time.Add(TimeSpan.FromHours(25)).Add(TimeSpan.FromDays(6)), setting.ValidUntil);
        }
    }
}
