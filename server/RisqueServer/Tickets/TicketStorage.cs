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
    public class TicketStorage {
        TicketDirectory ticketDirectory = null;
        Dictionary<int, Tuple<Ticket, TicketStatus>> tickets = null;
        Scheduler scheduler = null;
        public static string folderRoot = null;
        Thread WorkerThread;
        int workerRefreshMinutes = 1;         //How often should IOWorker check for updates
        bool addedTicket = false, removedTicket = false;
        /// <summary>
        /// Default Constructor that chains the Main Constructor
        /// </summary>
        /// <seealso cref="TicketStorage(string path)"/>
        public TicketStorage() : this(getConstructorPath()) {
            Console.WriteLine("Called Default Constructor");
        }

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
                Debug.WriteLine("Directory exists");
                string fileDirectory; 
                if (isRightSlash) fileDirectory = path + @"directory.json";
                else fileDirectory = path + @"directory.json";
                if (!File.Exists(fileDirectory)) {
                    Console.WriteLine("Attempted to create TicketStorage with an invalid path");
                    throw new Exception("Attempted to create TicketStorage with an invalid path");
                }
                else {
                    TicketStorage.folderRoot = path;
                    using (StreamReader reader = new StreamReader(fileDirectory)) {
                        //this.ticketDirectory = JsonConvert.DeserializeObject<TicketDirectory>(reader.ReadToEnd());
                        string readToEnd = reader.ReadToEnd();
                        Console.WriteLine(readToEnd);
                        this.ticketDirectory = TicketDirectory.Deserialize(readToEnd);
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
                Console.WriteLine("Directory does not exist, path=" + path);
                TicketStorage.folderRoot = path;       
                if (Extensions.IsLinux) {
                    Console.WriteLine("Creating Directory at: {0}", System.Environment.CurrentDirectory + @"/Tickets");
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"/Tickets");
                }
                else {
                    Console.WriteLine("Creating Directory at: {0}", System.Environment.CurrentDirectory + @"\Tickets");
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\Tickets");
                }
                
                //create directory.json
                Debug.WriteLine("Creating TicketDirectory Object");
                TicketDirectory directory = new TicketDirectory(0);
                string jsonData = JsonConvert.SerializeObject(directory);
                string directoryPath = String.Empty;
                directoryPath = path + "directory.json";
                /*if (Extensions.IsLinux) {
                    Console.WriteLine("Is Linux");
                    directoryPath = path + @"/directory.json";
                }
                else {
                    Console.WriteLine("Is NOT Linux");
                    directoryPath = path + @"\directory.json";
                }*/
                Console.WriteLine("Directory path: " + directoryPath);
                using (StreamWriter writer = File.CreateText(directoryPath)) {
                    writer.Write(jsonData);
                }
                //create TicketDirectory!!
                ticketDirectory = new TicketDirectory(0);
                this.tickets = new Dictionary<int, Tuple<Ticket, TicketStatus>>(ticketDirectory.ticketCount);
            }
            Debug.WriteLine("Starting IOWorker thread");
            WorkerThread = new Thread(() => IOWorker());
            WorkerThread.Start();
        }
        //fix the constructor field in windows v linux
        public static string getConstructorPath() {
            if (Extensions.IsLinux) {
                Console.WriteLine("getConstructor: is linux");
                return System.Environment.CurrentDirectory + "/Tickets";
            }
            else {
                Console.WriteLine("getConstructor: is not linux");
                return System.Environment.CurrentDirectory + @"\Tickets";
            }
        }
        /// <summary>
        /// Registers the ticket scheduler
        /// </summary>
        /// <param name="scheduler">Scheduler to store</param>
        /// <returns>True if the scheduler is null and can be assigned, false otherwise</returns>
        public bool registerScheduler(Scheduler scheduler) {
            if (this.scheduler == null) {
                this.scheduler = scheduler;
                return true;
            }
            else {
                return false;
            }
        }
        public Tuple<Ticket, TicketStatus>[] getTicketDict(object caller) {
            //Dictionary<int, Tuple<Ticket, TicketStatus>> tickets
            Debug.WriteLine("Called getTicketDict");
            if (caller == scheduler) {
                Debug.WriteLine("Caller check passed");
                if (ticketDirectory == null) {
                    Debug.WriteLine("TicketDirectory is Null!");
                }
                Tuple<Ticket, TicketStatus>[] arr = new Tuple<Ticket, TicketStatus>[ticketDirectory.ticketCount];
                int i = 0;
                Debug.WriteLine("Created Ticket, TicketStatus tuple");
                foreach (KeyValuePair<int,Tuple<Ticket, TicketStatus>> item in tickets) {
                    Debug.WriteLine("i: " + i);
                    arr[i] = new Tuple<Ticket, TicketStatus>(item.Value.Item1, item.Value.Item2);
                    i = i + 1;
                }
                Debug.WriteLine("Exited loop");
                //i = i;
                return arr;
            }
            return null;
        }
        private bool isValidDirectory(TicketDirectory ticketDirectory) {
            int count = 0;  //How many tickets are actually in directory.json versus its ticketCount
            //Currently not verifying status.json or ticket.json
            //http://stackoverflow.com/a/141098
            this.tickets = new Dictionary<int, Tuple<Ticket, TicketStatus>>(ticketDirectory.ticketCount);
            foreach (KeyValuePair<int, StoredDetails> entry in ticketDirectory.tickets) {
                if (!File.Exists(getAbsoluteFileLocation(entry.Value.ticketLocation))) {
                    Console.WriteLine("{0} does not exist -  isValidDirectory ticketLocation", getAbsoluteFileLocation(entry.Value.ticketLocation));
                    return false;
                }
                if (!Directory.Exists(getAbsoluteFolderLocation(entry.Value.folderLocation))) {
                    Console.WriteLine("{0} does not exist -  isValidDirectory folderLocation", getAbsoluteFolderLocation(entry.Value.folderLocation));
                    return false;
                }
                if (!File.Exists(getAbsoluteFileLocation(entry.Value.statusLocation))) {
                    Console.WriteLine("{0} does not exist -  isValidDirectory statusLocation", getAbsoluteFileLocation(entry.Value.statusLocation));
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
        public string getAbsoluteFileLocation(string path) {
            string x = path.Replace("./", folderRoot);
            if (!Extensions.IsLinux) {
                // Don't use a forward slash, convert to backslash
                x = x.Replace('/', '\\');
            }
            return x;
        }
        public string getAbsoluteFolderLocation(string path) {
            string x;
            if (Extensions.IsLinux) {
                x =  getAbsoluteFileLocation(path) + '/';
                return x.Replace('\\', '/');
            }
            else {
                x = getAbsoluteFileLocation(path) + '\\';
                return x.Replace('/', '\\');
            }
        }
        /// <summary>
        /// Stores a ticket and adds it to the scheduler
        /// </summary>
        /// <param name="ticket">The ticket to be stored</param>
        /// <param name="failureReason">Reason why the method failed</param>
        /// <returns>Whether the ticket was successfully stored</returns>
        public bool storeTicket(Ticket ticket, out string failureReason) {
            //Set addedTicket = true
            //create folder with given ticketId as name
            if (Directory.Exists(folderRoot + ticket.ticketID)) {
                failureReason = "Ticket directory already exists, maybe the ticket exists?";
                return false;
            }
            else {
                try {
                    string fullPath = null;
                    if (Extensions.IsLinux) {
                        fullPath = Directory.CreateDirectory(folderRoot + ticket.ticketID).FullName + '/';
                    }
                    else {
                        fullPath = Directory.CreateDirectory(folderRoot + ticket.ticketID).FullName + '\\';
                    }
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
                    scheduler.storeTicket(ticket);
                    addedTicket = true;
                    failureReason = null;
                    WorkerThread.Interrupt();
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
                    
                    if (addedTicket || removedTicket)
                    {
                        updateDirectoryFile();
                        Console.WriteLine("Updated");
                    }
                    //Debug.WriteLine("Sleeping for {0} minutes", sleepTime.Minutes);
                    Thread.Sleep(sleepTime);
                }
                catch (ThreadInterruptedException e1) {
                    //Wake from sleep, add ticket
                    continue;
                }
                catch (ThreadAbortException e2)
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
                File.Copy(folderRoot + "directory.json", folderRoot + "directory.json.backup", true);
                File.Delete(folderRoot + "directory.json");
                //File.WriteAllText(folderRoot + "directory.json", JsonConvert.SerializeObject(ticketDirectory));
                File.WriteAllText(folderRoot + "directory.json", TicketDirectory.Serialize(this.ticketDirectory));
            }
            addedTicket = false;
            removedTicket = false;
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
        public StoredDetails getStoredDetails(int ticketId, out bool success) {
            if (ticketDirectory.tickets.ContainsKey(ticketId)) {
                success = true;
                return ticketDirectory.tickets[ticketId];
            }
            else {
                success = false;
                return null;
            }
        }
        public TicketStatus getTicketStatus(int ticketId, out bool success) {
            if (ticketDirectory.tickets.ContainsKey(ticketId)) {
                success = true;
                return tickets[ticketId].Item2;
            }
            else {
                success = false;
                return null;
            }
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

        public bool removeTicket(int ticketId) {
            if (ticketDirectory.tickets.ContainsKey(ticketId)) {
                ticketDirectory.tickets.Remove(ticketId);
                ticketDirectory.ticketCount = ticketDirectory.ticketCount - 1;
                tickets.Remove(ticketId);
                scheduler.scheduledTickets.Remove(ticketId);
                scheduler.scheduledTickets.count = scheduler.scheduledTickets.count - 1;
                removedTicket = true;
                WorkerThread.Interrupt();
                return true;
            }
            else {
                return false;
            }
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
            obj.Add(new JProperty("tickets", arr));
            return obj.ToString();
        }
    }
}
