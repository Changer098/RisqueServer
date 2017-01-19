using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace RisqueServer.Tickets {
    /// <summary>
    /// Describes a Ticket for Deserializing
    /// </summary>
    class Ticket {
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
        public Action[] Actions { get; set; }
        public static Ticket getNull() {
            Ticket newTicket = new Ticket();
            newTicket.Actions = new Action[1];
            newTicket.Actions[0] = new Action();
            newTicket.dueBy = String.Empty;
            newTicket.ticketID = -1;
            return newTicket;
        }
    }
}
