using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Describes a Ticket for Deserializing
    /// </summary>
    public class Ticket : IComparable<Ticket> {
        //Constructor for Testing
        public Ticket(int id, DateTime dt) {
            this.ticketID = id;
            this.date = dt;
        }
        public Ticket() { }
        /*int id;
        Action[] actions;
        DateTime dueBy;
        string ticketLoc;           //stored info of the ticket - Each scheduled ticket gets its own directory
        public Ticket(int id, DateTime dueBy, Action[] actions) {
            this.id = id;
            this.dueBy = dueBy;
            this.actions = actions;
        }
        public static Ticket parseTicket(string json) {
            //can parse Action
            return null;
        }*/
        public int ticketID { get; set; }
        public string dueBy { get; set; }
        public bool isScheduled { get; set; }
        [JsonIgnore]
        public DateTime date;
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) {
            date = dueBy.fromRisqueTime();
        }
        public Action[] Actions { get; set; }
        public static Ticket getNull() {
            Ticket newTicket = new Ticket();
            newTicket.Actions = new Action[1];
            newTicket.Actions[0] = new Action();
            newTicket.dueBy = String.Empty;
            newTicket.ticketID = -1;
            return newTicket;
        }
        public int CompareTo(Ticket a) {
            if (a == null) {
                throw new Exception("Supplied ticket is null");
            }
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
        public static bool operator >(Ticket a, Ticket b) {
            if (a == null || b == null) {
                throw new Exception("A parameter is null");
            }
            int compare = a.CompareTo(b);
            if (compare == 1) {
                return true;
            }
            else {
                return false;
            }
        }
        public static bool operator <(Ticket a, Ticket b) {
            if (a == null || b == null) {
                throw new Exception("A parameter is null");
            }
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
