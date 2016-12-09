using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RisqueServer.Tickets;
using NUnit.Framework;

namespace RisqueServer.Tests {
    //All Tests passing 12/8/2016
    [TestFixture]
    class TestPortSpeedParsing {
        [TestCase]
        public void parseTestPass() {
            string testString = "10/100/1000T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(10, 100, 1000, 0), 'T', "SW");
            Assert.IsTrue(doEqual(shouldBe, portSpeed.ParseString(testString)));
        }
        [TestCase]
        public void parseTestPassDoubleSpeed() {
            string testString = "10/100T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto,new Tuple<int, int, int, int>(10,100,0,0),'T',"SW");
            Assert.IsTrue(doEqual(shouldBe, portSpeed.ParseString(testString)));
        }
        [TestCase]
        public void parseTestPassSingleSpeed() {
            string testString = "100T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(0, 100, 0, 0), 'T', "SW");
            Assert.IsTrue(doEqual(shouldBe, portSpeed.ParseString(testString)));
        }
        [TestCase]
        public void parseTestFailMegabitChange() {
            string testString = "10/400/1000T-SW-A";
            Assert.Throws<Exception>(() => portSpeed.ParseString(testString));
        }
        [TestCase]
        public void parseTestPassGigabitChange() {
            string testString = "10/100/1000/10000T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(10, 100, 1000, 10000), 'T', "SW");
            Assert.IsTrue(doEqual(shouldBe, portSpeed.ParseString(testString)));
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
        [TestCase]
        public void parseTestShouldPassIsfailing() {
            string testString = "10/100T-SW-A";
            portSpeed shouldBe = new portSpeed(Duplex.Auto, new Tuple<int, int, int, int>(10, 100, 0, 0), 'T', "SW");
            Assert.IsTrue(doEqual(shouldBe,portSpeed.ParseString(testString)));
        }
        //For some reason, NUnit's AreEqual doesn't work properly with portSpeed Objects
        public bool doEqual(portSpeed old, portSpeed newer) {
            if (old.duplex != newer.duplex) {
                Console.WriteLine("Duplex wrong");
                return false;
            }
            if (old.midMod != newer.midMod) {
                Console.WriteLine("midMod wrong");
                return false;
            }
            //check tuples
            if (old.speed.Item1 != newer.speed.Item1) {
                Console.WriteLine("item1 wrong");
                return false;
            }
            if (old.speed.Item2 != newer.speed.Item2) {
                Console.WriteLine("item2 wrong");
                return false;
            }
            if (old.speed.Item3 != newer.speed.Item3) {
                Console.WriteLine("item3 wrong");
                return false;
            }
            if (old.speed.Item4 != newer.speed.Item4) {
                Console.WriteLine("item4 wrong");
                return false;
            }
            if (old.speedMod != newer.speedMod) {
                Console.WriteLine("speedMod wrong");
                Console.WriteLine("new speedMod: {0}, old speedMod: {1}", newer.speedMod, old.speedMod);
                return false;
            }
            return true;
        }
    }
}
