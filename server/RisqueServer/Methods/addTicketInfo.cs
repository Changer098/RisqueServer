using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;

namespace RisqueServer.Methods {
    class addTicketInfo : IRPCMethod {
        TicketStorage storage;
        public addTicketInfo(TicketStorage storage) {
            this.storage = storage;
        }

        /// <summary>
        /// Store Ticket and add to scheduler
        /// </summary>
        /// <see cref="TicketStorage.storeTicket(Ticket)"/>
        public JProperty[] run(JProperty[] args) {
            //TODO Implement
            //Utilize TicketStorage.storeTicket
            return null;
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
