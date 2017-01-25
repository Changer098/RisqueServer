using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Controls where tickets are stored and where to store tickets
    /// </summary>
    class TicketStorage {
        TicketDirectory ticketDirectory = null;
        Dictionary<int, Tuple<Ticket, TicketStatus>> tickets = null;
        string folderRoot = null;
        Thread WorkerThread;
        int workerRefreshMinutes = 1;         //How often should IOWorker check for updates
        bool addedTicket;
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
            WorkerThread = new Thread(() => IOWorker());
            WorkerThread.Start();
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
        /// <param name="failureReason">Reason why the method failed</param>
        /// <returns>Whether the ticket was successfully stored</returns>
        public bool storeTicket(Ticket ticket, out string failureReason) {
            //TODO Implement
            //Set addedTicket = true
            //create folder with given ticketId as name
            if (Directory.Exists(folderRoot + ticket.ticketID)) {
                failureReason = "Ticket directory already exists, maybe the ticket exists?";
                return false;
            }
            else {
                try {
                    string fullPath = Directory.CreateDirectory(folderRoot + ticket.ticketID).FullName + '\\';
                    //fullPath = fullPath;
                    File.WriteAllText(fullPath + "ticket.json", JsonConvert.SerializeObject(ticket));
                    ticketDirectory.ticketCount = ticketDirectory.ticketCount + 1;
                    string relativeFolder = "./" + ticket.ticketID + '/';
                    //relativeFolder = relativeFolder;
                    StoredDetails detail = new StoredDetails("./" + ticket.ticketID,
                        relativeFolder + "ticket.json",
                        relativeFolder + "status.json",
                        ticket.ticketID);
                    //detail = detail;
                    TicketStatus status = new TicketStatus();
                    ticketDirectory.tickets.Add(ticket.ticketID, detail);
                    tickets.Add(ticket.ticketID, new Tuple<Ticket, TicketStatus>(ticket, status));
                    updateStatusFile(ticket.ticketID, false);
                    Console.WriteLine("SCHEDULER STILL HAS TO BE IMPLEMENTED");
                    addedTicket = true;
                    failureReason = null;
                    return true;
                }
                catch (Exception e) {
                    failureReason = e.Message;
                    return false;
                }
            }
            //System.Diagnostics.Debug.WriteLine("TicketStorage.storeTicket() has not been implemented");
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
		/// <summary>
        /// Background Worker for maintaining updates to files
        /// </summary>
        protected void IOWorker()
        {
            TimeSpan sleepTime = new TimeSpan(0, workerRefreshMinutes, 0);
            while(true)
            {
                try {
                    
                    if (addedTicket)
                    {
                        updateDirectoryFile();
                        Console.WriteLine("Updated");
                    }
                    Debug.WriteLine("Sleeping for {0} minutes", sleepTime.Minutes);
                    Thread.Sleep(sleepTime);
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine("TicketStorage thread was interrupted, killing");
                    break;
                }
            }
            //Thread is closing, do something
            //TODO Implement BackgroundWorker closing
        }

        //Make async
        private void updateDirectoryFile() {
            //updates the directory file with the new values
            string filePath = folderRoot + "directory.json";
            if (!File.Exists(filePath)) {
                //FBackups are when things go badly, not normal backups
                Debug.WriteLine("Directory file doesn't exist, backing up our current settings.");
                if (File.Exists(folderRoot + "directory.Fbackup.json")) {
                    File.Copy(folderRoot + "directory.Fbackup.json", folderRoot + "directory.Fbackup.json.old");
                    File.Delete(folderRoot + "directory.Fbackup.json");
                }
                //File.WriteAllText(folderRoot + "directory.Fbackup.json", JsonConvert.SerializeObject(ticketDirectory));
                File.WriteAllText(folderRoot + "directory.Fbackup.json", TicketDirectory.Serialize(this.ticketDirectory));
                throw new Exception("Can't update directory file, doesn't exist");
                //Maybe backup the current directory to file?
            }
            else {
                File.Copy(folderRoot + "directory.json", folderRoot + "directory.json.backup");
                File.Delete(folderRoot + "directory.json");
                //File.WriteAllText(folderRoot + "directory.json", JsonConvert.SerializeObject(ticketDirectory));
                File.WriteAllText(folderRoot + "directory.json", TicketDirectory.Serialize(this.ticketDirectory));
            }
        }
        //Run async
        private void updateStatusFile(int ticketId, bool completed) {
            //updates the given status file for a ticket
            //assumes the ticket has already been verified and stored in tickets[]
            if (tickets.ContainsKey(ticketId)) {
                TicketStatus status = tickets[ticketId].Item2;
                if (completed) {
                    status.completed = true;
                    status.completionDate = DateTime.Now.toRisqueTime();
                }
                if (ticketDirectory.tickets.ContainsKey(ticketId)) {
                    string statusLocation = ticketDirectory.tickets[ticketId].statusLocation;
                    statusLocation = getAbsoluteFileLocation(statusLocation);
                    File.WriteAllText(statusLocation, JsonConvert.SerializeObject(status));
                }
                else {
                    throw new Exception("Ticket has been stored, but not in the ticketDirectory");
                }
            }
            else {
                throw new Exception("Ticket hasn't been stored yet, can't update Status file for it");
            }
        }

        //Callable method for completing tickets
        public void completeTicket(int tickedId) {
            Console.WriteLine("Completed: " + tickedId);
            updateStatusFile(tickedId, true);
        }

        //Run async
        private void writeTicketFile(Ticket ticket, string path) {
            //writes ticket to path
            if (File.Exists(path)) {
                //either updating or the file is occupied for some reason, move and copy it
                string newPath = path + ".old";
                //string DataToCopy = File.ReadAllText(path);
                //File.WriteAllText(newPath, DataToCopy);
                File.Copy(path, newPath, true);
                File.Delete(path);
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(ticket));
        }
    }
    /// <summary>
    /// The ticket field in Directory.json
    /// </summary>
    public sealed class StoredDetails {
        public StoredDetails() { }
        public StoredDetails(string folderLocation, string ticketLocation, string statusLocation, int id) {
            this.folderLocation = folderLocation;
            this.ticketLocation = ticketLocation;
            this.statusLocation = statusLocation;
            this.id = id;
        }
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
                //broken
                StoredDetails stored = token.ToObject<StoredDetails>();
                direct.tickets.Add(stored.id, stored);
            }
            return direct;
        }
        public static string Serialize(TicketDirectory tick) {
            JObject obj = new JObject(new JProperty("ticketCount", tick.ticketCount));
            JArray arr = new JArray();
            foreach (KeyValuePair<int, StoredDetails> entry in tick.tickets) {
                try {
                    JObject entryObj = new JObject(new JProperty("id", entry.Value.id),
                        new JProperty("folderLocation", entry.Value.folderLocation),
                        new JProperty("statusLocation", entry.Value.statusLocation),
                        new JProperty("ticketLocation", entry.Value.ticketLocation));
                    arr.Add(entryObj);
                }
                catch (Exception e) {
                    string message = e.Message;
                }
            }
            obj.Add(arr);
            return obj.ToString();
        }
    }
}
