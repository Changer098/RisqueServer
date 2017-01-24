using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

//ALL tests passing 1/20/2017
namespace RisqueServer.Tests {
    [TestFixture]
    class TestRisqueDateDecoding {
        [TestCase]
        public void tryParse() {
            string date = @"1/31/2017 9:00 AM (s)";
            Assert.DoesNotThrow(() => parseString(date));
        }
        [TestCase]
        public void basicDate() {
            string date = @"1/31/2017 9:00 AM (s)";
            DateTime shouldBe = new DateTime(2017, 1, 31, 9, 0, 0);
            DateTime actual = parseString(date);
            actual = actual;
            Assert.IsTrue(shouldBe == actual,"BasicDate() are not same");
        }
        [TestCase]
        public void moreDates() {
            string dueBy = @"1/25/2017 5:00 PM (s) ";
            string submitted = @"1/19/2017 4:09 PM";
            DateTime dueShouldBe = new DateTime(2017, 1, 25, 17, 0, 0);
            DateTime subShouldBe = new DateTime(2017, 1, 19, 16, 9, 0);
            Assert.IsTrue(dueShouldBe == parseString(dueBy), "moreDates() failed dueBy");
            Assert.IsTrue(subShouldBe == parseString(submitted), "moreDates() failed submitted");
        }

        //Now located in Extensions.ParseRisqueTime()
        public DateTime parseString(string text) {
            text = text.Trim();
            string toParse;
            if (text.Last<char>() == ')') {
                //get rid of the weird "(s)" formatting at the end
                toParse = text.Substring(0, text.Length - 3);
            }
            else {
                toParse = text;
            }
            return DateTime.Parse(toParse);
        }
    }
}
