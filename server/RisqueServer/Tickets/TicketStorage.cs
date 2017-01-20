using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Controls where tickets are stored and where to store tickets
    /// </summary>
    class TicketStorage {
        TicketDirectory ticketDirectory = null;
        Dictionary<int, Tuple<Ticket, TicketStatus>> tickets = null;
        string folderRoot = null;
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
            bool isRightSlash = false;
            if (path.Contains('\\')) {
                isRightSlash = false;
                if (path.Last<char>() != '\\') {
                    path = path + '\\';
                }
            }
            else if (path.Contains('/')) {
                isRightSlash = true;
                if (path.Last<char>() != '/') {
                    path = path + '/';
                }
            }

            if (Directory.Exists(path)) {
                //try and load directory.json
                string fileDirectory; 
                if (isRightSlash) fileDirectory = path + @"directory.json";
                else fileDirectory = path + @"directory.json";
                if (!File.Exists(fileDirectory)) {
                    Console.WriteLine("Attempted to create TicketStorage with an invalid path");
                    throw new Exception("Attempted to create TicketStorage with an invalid path");
                }
                else {
                    this.folderRoot = path;
                    using (StreamReader reader = new StreamReader(fileDirectory)) {
                        //this.ticketDirectory = JsonConvert.DeserializeObject<TicketDirectory>(reader.ReadToEnd());
                        this.ticketDirectory = TicketDirectory.Deserialize(reader.ReadToEnd());
                    }
                    //Validate directory
                    if (!isValidDirectory(this.ticketDirectory)) {
                        throw new Exception("Loaded an invalid Ticket Directory");
                    }
                    //Load Tickets from stored json format into a dictionary
                }

            }
            else {
                //create path
                this.folderRoot = path;
                DirectoryInfo inf = Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\Tickets");
                //create directory.json
                TicketDirectory directory = new TicketDirectory(0);
                string jsonData = JsonConvert.SerializeObject(directory);
                string directoryPath = path + @"\directory.json";
                using (StreamWriter writer = File.CreateText(directoryPath)) {
                    writer.Write(jsonData);
                }
            }
        }
        private bool isValidDirectory(TicketDirectory ticketDirectory) {
            int count = 0;  //How many tickets are actually in directory.json versus its ticketCount
            //Currently not verifying status.json or ticket.json
            //http://stackoverflow.com/a/141098
            this.tickets = new Dictionary<int, Tuple<Ticket, TicketStatus>>(ticketDirectory.ticketCount);
            foreach (KeyValuePair<int, StoredDetails> entry in ticketDirectory.tickets) {
                if (!File.Exists(getAbsoluteFileLocation(entry.Value.ticketLocation))) {
                    Console.WriteLine("{0} does not exist", getAbsoluteFileLocation(entry.Value.ticketLocation));
                    return false;
                }
                if (!Directory.Exists(getAbsoluteFolderLocation(entry.Value.folderLocation))) {
                    Console.WriteLine("{0} does not exist", getAbsoluteFolderLocation(entry.Value.folderLocation));
                    return false;
                }
                if (!File.Exists(getAbsoluteFileLocation(entry.Value.statusLocation))) {
                    Console.WriteLine("{0} does not exist", getAbsoluteFileLocation(entry.Value.statusLocation));
                    return false;
                }
                if (!LoadTicket(getAbsoluteFileLocation(entry.Value.ticketLocation), getAbsoluteFileLocation(entry.Value.statusLocation))) {
                    return false;
                }
                count++;
            }

            if (count != ticketDirectory.ticketCount) {
                Debug.WriteLine("Ticket Count: {0} does not match count supplied by directory.json: {1}", count, ticketDirectory.ticketCount);
                return false;
            }
            return true;
        }
        private bool LoadTicket(string ticketPath, string statusPath) {
            try {
                using (StreamReader ticketReader = new StreamReader(ticketPath)) {
                    using (StreamReader statusReader = new StreamReader(statusPath)) {
                        string ticketJson = ticketReader.ReadToEnd();
                        if (ticketJson == String.Empty) return false;
                        string statusJson = statusReader.ReadToEnd();
                        if (statusJson == String.Empty) return false;
                        Ticket newTicket = JsonConvert.DeserializeObject<Ticket>(ticketJson);
                        TicketStatus newStatus = JsonConvert.DeserializeObject<TicketStatus>(statusJson);
                        this.tickets.Add(newTicket.ticketID, new Tuple<Ticket, TicketStatus>(newTicket, newStatus));
                        return true;
                    }
                }
            }
            catch (Exception e) {
                return false;
            }
        }
        private string getAbsoluteFileLocation(string path) {
            return path.Replace("./", folderRoot);
        }
        private string getAbsoluteFolderLocation(string path) {
            return getAbsoluteFileLocation(path) + '\\';
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
            try {
                Ticket tick = this.tickets[id].Item1;
                return true;
            }
            catch {
                return false;
            }
        }
        /// <summary>
        /// Retrieve a stored id
        /// </summary>
        /// <param name="id">The id of the ticket being retrieved</param>
        /// <param name="success">Whether or not a Ticket was found</param>
        /// <returns>The requested ticket or null</returns>
        public Ticket getTicket(int id, out bool success) {
            Ticket tick;
            try {
                tick = this.tickets[id].Item1;
                success = true;
                return tick;
            }
            catch {
                success = false;
                return null;
            }
        }

        //TODO Implement
        //Make async
        private void updateDirectoryFile() {
            //updates the directory file with the new values
            
        }
        //TODO Implement
        //Make async
        private void updateStatusFile(int ticketId) {
            //updates the given status file for a ticket
        }
        //TODO Implement
        //Make async
        private void writeTicketFile(Ticket ticket, string path) {
            //writes ticket to path
        }
    }
    /// <summary>
    /// The ticket field in Directory.json
    /// </summary>
    public sealed class StoredDetails {
        public string folderLocation { get; set; }
        public string ticketLocation { get; set; }
        public string statusLocation { get; set; }
        public int id { get; set; }
    }
    /// <summary>
    /// Directory.json
    /// </summary>
    public class TicketDirectory {
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
                throw new Exception("Could not deserialize TicketDirectory");
            }

            TicketDirectory direct = new TicketDirectory(jobj.Property("ticketCount").Value.ToObject<int>());
            foreach (JToken token in jobj.Property("tickets").Values()) {
                //Create StoredDetail and add to Dictionary
                StoredDetails stored = token.ToObject<StoredDetails>();
                direct.tickets.Add(stored.id, stored);
            }
            return direct;
        }
    }
}
