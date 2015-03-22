using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DatabaseAccessTest
    {
        [TestMethod]
        public void AddNewTerminal()
        {
            DalTerminal dalTerminal = new DalTerminal();
            int? id = dalTerminal.Add(new Terminal(){Name = "drugi terminal"});
            dalTerminal.Dispose();
            Assert.IsTrue(id != null && id > 0);
        }

        [TestMethod]
        public void DeleteTerminal()
        {
            int terminalCountBefore;
            int terminalCountAfter;
            DalTerminal dalTerminal = new DalTerminal();
            terminalCountBefore = dalTerminal.GetTotalNumberOfTerminals();

            Terminal t = new Terminal() {Name = "treci terminal"};
            dalTerminal.Add(t);
            dalTerminal.Delete(t);
            terminalCountAfter = dalTerminal.GetTotalNumberOfTerminals();
            dalTerminal.Dispose();
            Assert.AreEqual(terminalCountAfter, terminalCountBefore);
        }
    }
}
