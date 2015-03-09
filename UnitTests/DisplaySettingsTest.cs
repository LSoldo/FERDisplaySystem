using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL;
using DAL;
using DAL.Model;
using System.Web.Script.Serialization;

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
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(terminals, DateTime.Today.AddDays(-1), TimeSpan.FromDays(1), null, null, ds);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_TimeSpanLessThan3Minutes_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(terminals, DateTime.Today, TimeSpan.FromMinutes(2), null, null, ds);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_NoTerminals_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>();
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(terminals, DateTime.Today, TimeSpan.FromMinutes(10), null, null, ds);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_ConsecutiveNumberOfDaysIsZero_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            var time = DateTime.Now;
            DigitalSign ds = new DigitalSign();

            var setting = manager.CreateDisplayTimes(terminals, time, TimeSpan.FromHours(23), null, 0, ds);
        }

        [TestMethod]
        public void CreateDisplayTimes_CountNumberOfRecords_Equal()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;           
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times = manager.CreateDisplayTimes(terminals, ds.StartTime, ds.DurationSpan, null, 7, sign);
            string output = new JavaScriptSerializer().Serialize(times);

            Assert.AreEqual(7, times.Count);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_GetOverlapping_True()
        {
            var manager = new DisplaySettingsManager();
            var terminals = new List<Terminal>() { new Terminal() { Name = "First terminal" } };
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminals, ds.StartTime, ds.DurationSpan, null, 1, sign);
            var times2 = manager.CreateDisplayTimes(terminals, ds.StartTime.AddHours(-1), ds.DurationSpan.Add(TimeSpan.FromHours(4)), null, 1, sign);

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }

        

    }
}
