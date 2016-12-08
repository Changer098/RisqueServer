using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RisqueServer.Tickets;
using NUnit.Framework;

namespace RisqueServer.Tests {
    //All Tests passing 12/8/2016
    [TestFixture]
    class TestActionParsing {
        [TestCase]
        public void parseTestPass() {
            string testString = "10/100/1000T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(10, 100, 1000, 0), 'T', "SW");
            portSpeed parsed;
            try {
                parsed = portSpeed.ParseString(testString);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            Assert.Equals(shouldBe, parsed);
        }
        [TestCase]
        public void parseTestPassDoubleSpeed() {
            string testString = "10/100T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto,new Tuple<int, int, int, int>(10,100,0,0),'A',"SW");
            portSpeed parsed;
            try {
                parsed = portSpeed.ParseString(testString);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            Assert.Equals(shouldBe, parsed);
        }
        [TestCase]
        public void parseTestPassSingleSpeed() {
            string testString = "100T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(0, 100, 0, 0), 'A', "SW");
            portSpeed parsed;
            try {
                parsed = portSpeed.ParseString(testString);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            Assert.Equals(shouldBe, parsed);
        }
        [TestCase]
        public void parseTestFailMegabitChange() {
            string testString = "10/400/1000T-SW-A";
            Assert.Throws<Exception>(() => portSpeed.ParseString(testString));
        }
        [TestCase]
        public void parseTestPassGigabitChange() {
            string testString = "10/100/1000/10000T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(10, 100, 1000, 10000), 'A', "SW");
            portSpeed parsed;
            try {
                parsed = portSpeed.ParseString(testString);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            Assert.Equals(shouldBe, parsed);
        }
       /* [TestCase]
        public void TestNUnit() {
            //This should fail
            System.Diagnostics.Debug.WriteLine("This should fail");
            Assert.Equals(true, false);
        }
        [TestCase]*/
        public void parseTestFail() {
            string testString = "10T-";
            Assert.Throws<Exception>(() => portSpeed.ParseString(testString));
        }
        [TestCase]
        public void parseTestFailTricky() {
            string testString = "10/500/1000T-SW-A";
            Assert.Throws<Exception>(() => portSpeed.ParseString(testString));
        }
        [TestCase]
        public void parseTestFailOutOfOrder() {
            string testString = "1000/100/10T-SW-A";
            Assert.Throws<Exception>(() => portSpeed.ParseString(testString));
        }
    }
}
