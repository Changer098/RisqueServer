using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    class Ticket {
        int id;
        Action[] actions;
        public Ticket(int id, Action[] actions) {
            this.id = id;
            this.actions = actions;
        }
    }
}
