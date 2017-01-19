using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RisqueServer.Tickets;
using NUnit.Framework;
using System.IO;

namespace RisqueServer.Tests {
    [TestFixture]
    public class TicketDirectoryTest {
        [TestCase]
        public void DeserializeBasicTest() {
            //I'm an idiot
            string fileLoc = @"C:\Users\everettr\Desktop\testDirectory.json";
            string text = new StreamReader(fileLoc).ReadToEnd();
            Console.WriteLine(text);
            Assert.DoesNotThrow(() => TicketDirectory.Deserialize(text));
        }
        [TestCase]
        public void DeseralizeAreEqual() {
            //same text as above
            string json = @"{
              'ticketCount': 2,
              'tickets': [
                {
                  'id': 25,
                  'folderLocation': 'blahblah',
                  'ticketLocation': 'blanky'
                },
                {
                  'id': 69,
                  'folderLocation': 'testy',
                  'ticketLocation': 'broken'
                }
              ]
            }";
            TicketDirectory directory = TicketDirectory.Deserialize(json);
            StoredDetails[] dets = { DumbConstructor(25, "blahblah", "blanky"),
                DumbConstructor(69, "testy", "broken")};
            TicketDirectory shouldBe = FakeConstructor(2, dets);
            Assert.IsTrue(directory.ticketCount == shouldBe.ticketCount);
            Console.WriteLine("TicketCounts are the same");
            Assert.IsTrue(directory.tickets.Count == shouldBe.tickets.Count);
            Console.WriteLine("Dictionary counts are the same");
            
            int[] ids = { 25, 69 };
            foreach (int id in ids) {
                Assert.IsTrue(directory.tickets[id].id == shouldBe.tickets[id].id);
                Console.WriteLine("Tickets[{0}] contain same ids", id);
                Assert.IsTrue(directory.tickets[id].ticketLocation.Equals(shouldBe.tickets[id].ticketLocation));
                Console.WriteLine("Tickets[{0}] contain same ticket Locations", id);
                Assert.IsTrue(directory.tickets[id].folderLocation.Equals(shouldBe.tickets[id].folderLocation));
                Console.WriteLine("Tickets[{0}] contain same folder Locations", id);
            }
            //Assert.AreSame(shouldBe, directory);
        }
        [TestCase]
        public void BuildTicketStorage() {
            TicketStorage storage;
            Assert.DoesNotThrow(() => {
                string path = @"C:\Users\everettr\Documents\Repositories\RisqueServer\server\RisqueServer\bin\Debug\Tickets";
                storage = new TicketStorage(path);
            });
        }

        private TicketDirectory FakeConstructor(int ticketCount, StoredDetails[] dets) {
            TicketDirectory direct = new TicketDirectory(ticketCount);
            foreach (StoredDetails det in dets) {
                direct.tickets.Add(det.id, det);
            }
            return direct;
        }
        private StoredDetails DumbConstructor(int id, string folderLocation, string ticketLocation) {
            StoredDetails deter = new StoredDetails();
            deter.id = id;
            deter.folderLocation = folderLocation;
            deter.ticketLocation = ticketLocation;
            return deter;
        }
    }
}
