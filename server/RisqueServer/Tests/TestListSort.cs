using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RisqueServer.Tickets;
using NUnit.Framework;

namespace RisqueServer.Tests {
    [TestFixture]
    class TestListSort {
        [TestCase]
        public void basicTest() {
            TicketList list = new TicketList(4);
            list.Add(new Ticket(5050, new DateTime(2010, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2014, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2011, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2012, 1, 1)));
            list.printList();
            list.Sort();
            Assert.IsTrue(list.isSorted());
            Console.WriteLine("Sorted list");
            list.printList();
        }
        [TestCase]
        public void reverseList() {
            TicketList list = new TicketList(4);
            list.Add(new Ticket(5050, new DateTime(2013, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2012, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2011, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2010, 1, 1)));
            list.printList();
            list.Sort();
            Assert.IsTrue(list.isSorted());
            Console.WriteLine("Sorted list");
            list.printList();
        }
        [TestCase]
        public void alreadySorted() {
            TicketList list = new TicketList(4);
            list.Add(new Ticket(5050, new DateTime(2010, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2011, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2012, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2014, 1, 1)));
            list.printList();
            Assert.IsTrue(list.isSorted());
            Console.WriteLine("Sorted list");
            list.printList();
        }
        [TestCase]
        public void testRemove() {
            TicketList list = new TicketList(4);
            list.Add(new Ticket(5050, new DateTime(2010, 1, 1)));
            list.Add(new Ticket(8080, new DateTime(2011, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2012, 1, 1)));
            list.printList();
            list.Remove(8080);
            list.printList();
            int blah;
            Assert.IsFalse(list.Contains(8080, out blah));
        }
        [TestCase]
        public void RemoveDuplicates() {
            TicketList list = new TicketList(4);
            list.Add(new Ticket(5050, new DateTime(2010, 1, 1)));
            list.Add(new Ticket(8080, new DateTime(2011, 1, 1)));
            list.Add(new Ticket(5050, new DateTime(2012, 1, 1)));
            list.printList();
            list.Remove(5050);
            list.printList();
            int blah;
            Assert.IsFalse(list.Contains(5050, out blah));
        }
    }
}
