using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace RisqueServer.Tickets {
    class Ticket {
        int id;
        Action[] actions;
        DateTime dueBy;
        string ticketLoc;           //stored info of the ticket - Each scheduled ticket gets its own directory
        public Ticket(int id, DateTime dueBy, Action[] actions) {
            this.id = id;
            this.dueBy = dueBy;
            this.actions = actions;
        }
        public static Ticket parseTicket(string json) {
            return null;
        }
    }
}
