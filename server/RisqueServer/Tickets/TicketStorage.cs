using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Controls where tickets are stored and where to store tickets
    /// </summary>
    class TicketStorage {
        TicketDirectory ticketDirectory = null; 
        /// <summary>
        /// Default Constructor that chains the Main Constructor
        /// </summary>
        /// <seealso cref="TicketStorage(string path)"/>
        public TicketStorage() : this(System.Environment.CurrentDirectory + "\\Tickets") { }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="path">Place where the Tickets are currently stored or will be stored</param>
        /// <remarks>Path must contain a valid directory.json</remarks>
        public TicketStorage(string path) {
            if (Directory.Exists(path)) {
                //try and load directory.json
                string fileDirectory = path + @"\directory.json";
                if (!File.Exists(fileDirectory)) {
                    Console.WriteLine("Attempted to create TicketStorage with an invalid path");
                    throw new Exception("Attempted to create TicketStorage with an invalid path");
                }
                else {
                    using (StreamReader reader = new StreamReader(fileDirectory)) {
                        //this.ticketDirectory = JsonConvert.DeserializeObject<TicketDirectory>(reader.ReadToEnd());
                        this.ticketDirectory = TicketDirectory.Deserialize(reader.ReadToEnd());
                    }
                    //Validate directory
                    if (!isValidDirectory(this.ticketDirectory)) {
                        throw new Exception("Loaded an invalid Ticket Directory");
                    }
                }

            }
            else {
                //create path
                DirectoryInfo inf = Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\Tickets");

            }
        }
        private bool isValidDirectory(TicketDirectory ticketDirectory) {
            int count = 0;
            /*foreach (StoredDetails det in ticketDirectory.tickets) {
                count++;
                if (!File.Exists(det.configLocation) || !Directory.Exists(det.folderLocation)) {
                    return false;
                }
            }*/
            //http://stackoverflow.com/a/141098
            foreach (KeyValuePair<int, StoredDetails> entry in ticketDirectory.tickets) {
                // do something with entry.Value or entry.Key
            }

            if (count != ticketDirectory.ticketCount) return false;
            return true;
        }
        /// <summary>
        /// Stores a ticket and adds it to the scheduler
        /// </summary>
        /// <param name="ticket">The ticket to be stored</param>
        /// <returns>Whether the ticket was successfully stored</returns>
        public bool storeTicket(Ticket ticket) {
            //TODO Implement
            System.Diagnostics.Debug.WriteLine("TicketStorage.storeTicket() has not been implemented");
            return false;
        }
        /// <summary>
        /// Checks whether or not a ticket exists in the system
        /// </summary>
        /// <param name="id">The id of the ticket being searched</param>
        /// <returns>True if a Ticket exists, False if it doesn't</returns>
        public bool containsTicket(int id) {
            //TODO Implement
            return false;
        }
        /// <summary>
        /// Retrieve a stored id
        /// </summary>
        /// <param name="id">The id of the ticket being retrieved</param>
        /// <returns>The requested ticket or null</returns>
        public Ticket getTicket(int id) {
            //TODO Implement
            return null;
        }
    }
    /// <summary>
    /// The ticket field in Directory.json
    /// </summary>
    sealed class StoredDetails {
        public string folderLocation { get; set; }
        public string configLocation { get; set; }
        public int id { get; set; }
    }
    /// <summary>
    /// Directory.json
    /// </summary>
    class TicketDirectory {
        public TicketDirectory(int ticketCount) {
            this.ticketCount = ticketCount;
            this.tickets = new Dictionary<int, StoredDetails>(ticketCount);
        }
        public int ticketCount { get; set; }
        public Dictionary<int, StoredDetails> tickets { get; set; }
        //public StoredDetails[] tickets { get; set; }
        public static TicketDirectory Deserialize(string text) {
            JObject jobj;
            try {
                jobj = JObject.Parse(text);
            }
            catch (Exception e) {
                Debug.WriteLine("TicketDirectory failed to deserialize with Error: " + e.Message);
                return null;
            }

            TicketDirectory direct = new TicketDirectory(jobj.Property("ticketCount").Value.ToObject<int>());
            foreach (JProperty prop in jobj.Property("tickets").Value) {
                //Create StoredDetail and add to Dictionary
            }
        }
    }
}
