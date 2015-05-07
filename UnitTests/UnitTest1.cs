using System;
using BL;
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
            var manager = new BLDisplaySettingsManager();
            DisplaySetting setting;
            var times = manager.CreateDisplayTimes(DateTime.Now, DateTime.Now.AddHours(2), TimeSpan.FromHours(2), 3,
                out setting);

            Assert.AreEqual(3, times.Count);
        }
    }
}
