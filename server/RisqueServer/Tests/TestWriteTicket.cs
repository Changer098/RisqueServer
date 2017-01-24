using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RisqueServer.Tickets;
using System.Threading.Tasks;
using NUnit.Framework;

//all tests passed 1/24/2017
namespace RisqueServer.Tests {
    [TestFixture]
    class TestWriteTicket {
        [TestCase]
        public void writeBlankTicket() {
            string filePath = @"C:\Users\everettr\Desktop\ticket.json";
            Ticket t = new Ticket();
            t.Actions = new Tickets.Action[1];
            t.Actions[0] = new Tickets.Action();
            t.Actions[0].portInfo = new PortInfo();
            t.Actions[0].settings = new Settings();
            t.ticketID = 5;
            //set function to static for this test to work.
            TicketStorage.writeTicketFile(t, filePath);
            FileAssert.Exists(filePath);
        }
        [TestCase]
        public void overwriteBlankTicket() {
            string filePath = @"C:\Users\everettr\Desktop\ticket.json";
            Ticket t = new Ticket();
            t.Actions = new Tickets.Action[2];
            t.Actions[0] = new Tickets.Action();
            t.Actions[0].portInfo = new PortInfo();
            t.Actions[0].settings = new Settings();
            t.Actions[1] = new Tickets.Action();
            t.Actions[1].portInfo = new PortInfo();
            t.Actions[1].settings = new Settings();
            t.ticketID = 25;
            //set function to static for this test to work.
            TicketStorage.writeTicketFile(t, filePath);
            FileAssert.Exists(filePath);
        }
    }
}
