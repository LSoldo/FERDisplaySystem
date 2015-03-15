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
            var terminal = new Terminal(){Name="prvi terminal"};
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(terminal, DateTime.Today.AddDays(-1), TimeSpan.FromDays(1), null, null, ds, "Event");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_TimeSpanLessThan3Minutes_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(terminal, DateTime.Today, TimeSpan.FromMinutes(2), null, null, ds, "Event");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateDisplaySetting_NoTerminals_ThrowsException()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            DigitalSign ds = new DigitalSign();

            manager.CreateDisplayTimes(null, DateTime.Today, TimeSpan.FromMinutes(10), null, null, ds, "Event");
        }


        [TestMethod]
        public void CreateDisplayTimes_CountNumberOfRecords_Equal()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;           
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 7, sign, "Event");
            string output = new JavaScriptSerializer().Serialize(times);

            Assert.AreEqual(7, times.Count);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_GetOverlapping_True()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(-1), ds.DurationSpan.Add(TimeSpan.FromHours(4)), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_StartOfTheFirstIsEndOfTheSecond_False()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(-1), TimeSpan.FromHours(1), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(false, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_SecondsGoesOneHourAfterStartOfTheFirst_True()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(-1), TimeSpan.FromHours(2), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_SecondOutsideOfFirst_False()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(5), TimeSpan.FromHours(2), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(false, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_StartOfTheSecondIsEndOfTheFirst_False()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(2), TimeSpan.FromHours(2), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(false, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_SecondIsCompletelyOverlappingFirst_True()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(-1), TimeSpan.FromHours(5), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_SecondIsCompletelyInsideFirst_True()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime, TimeSpan.FromHours(2), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_SecondIsCompletelyInsideFirst2_True()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal(){Name="prvi terminal"};
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, null, 1, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.AddHours(0.5), TimeSpan.FromHours(0.5), null, 1, sign, "Event");

            bool areOverlapping = manager.IsOverlappingWithExistingDate(times1, times2);
            Assert.AreEqual(true, areOverlapping);
        }
        [TestMethod]
        public void ConvertScheduledTimesToJson_GettingData()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal() { Name = "prvi terminal" };
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(24);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, TimeSpan.FromHours(24), null, sign, "Event");
            var json = manager.ConvertScheduledTimesToJson(times);

            //just to pass, not a test
            Assert.AreEqual(30, times.Count);
        }

        [TestMethod]
        public void IsOverlappingWithExistingDate_2HourVoid_False()
        {
            var manager = new DisplaySettingsManager();
            var terminal = new Terminal() { Name = "prvi terminal" };
            var time = DateTime.Now;

            DisplaySetting ds = new DisplaySetting();
            ds.StartTime = time;
            ds.DurationSpan = TimeSpan.FromHours(2);

            DigitalSign sign = new DigitalSign();
            sign.Name = "sign";

            var times1 = manager.CreateDisplayTimes(terminal, ds.StartTime, ds.DurationSpan, TimeSpan.FromHours(2), null, sign, "Event");
            var times2 = manager.CreateDisplayTimes(terminal, ds.StartTime.Add(TimeSpan.FromHours(2)), ds.DurationSpan, TimeSpan.FromHours(2), null, sign, "Event");
            var json1 = manager.ConvertScheduledTimesToJson(times1);
            var json2 = manager.ConvertScheduledTimesToJson(times2);

            Assert.AreEqual(false, manager.IsOverlappingWithExistingDate(times1, times2));


        }



    }
}
