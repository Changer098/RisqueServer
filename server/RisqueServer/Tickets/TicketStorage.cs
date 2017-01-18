using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Controls where tickets are stored and where to store tickets
    /// </summary>
    class TicketStorage {       
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
            }
            else {
                //create path
            }
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
    }
    /// <summary>
    /// Directory.json
    /// </summary>
    class TicketDirectory {
        public int ticketCount { get; set; }
        StoredDetails[] tickets { get; set; }
    }
}
