using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

//ALL TESTS PASSING 1/26/2017
namespace RisqueServer.Tests {
    [TestFixture]
    class TestDateSort {
        [TestCase]
        public void basicSort() {
            List<FakeTicket> tickets = new List<FakeTicket>(5);
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 3)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 2)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 4)));
            tickets.Sort();
            printList(tickets);
            Assert.IsTrue(isSorted(tickets), "List is not sorted");
        }
        [TestCase]
        public void allSame() {
            List<FakeTicket> tickets = new List<FakeTicket>(5);
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Sort();
            printList(tickets);
            Assert.IsTrue(isSorted(tickets), "List is not sorted");
        }
        [TestCase]
        public void reverseOrder() {
            List<FakeTicket> tickets = new List<FakeTicket>(5);
            tickets.Add(new FakeTicket(new DateTime(2015, 4, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 3, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 2, 1)));
            tickets.Add(new FakeTicket(new DateTime(2015, 1, 1)));
            tickets.Sort();
            printList(tickets);
            Assert.IsTrue(isSorted(tickets), "List is not sorted");
        }

        public bool isSorted(List<FakeTicket> tickets) {
            for (int i = 0; i < tickets.Count - 1; i++) {
                if (tickets[i] > tickets[i+1]) {
                    return false;
                }
            }
            return true;
        }
        public void printList(List<FakeTicket> tickets) {
            for (int i = 0; i < tickets.Count; i++) {
                Console.WriteLine("{0}: {1}", i, tickets[i].date.ToString());
            }
        }
    }
    class FakeTicket : IComparable<FakeTicket> {
        public DateTime date;
        public FakeTicket(DateTime date) {
            this.date = date;
        }
        public int CompareTo(FakeTicket a) {
            if (this.date > a.date) {
                return 1;
            }
            else if (this.date < a.date) {
                return -1;
            }
            else {
                return 0;
            }
        }
        public static bool operator> (FakeTicket a, FakeTicket b) {
            int compare = a.CompareTo(b);
            if (compare == 1) {
                return true;
            }
            else {
                return false;
            }
        }
        public static bool operator< (FakeTicket a, FakeTicket b) {
            int compare = a.CompareTo(b);
            if (compare == -1) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
