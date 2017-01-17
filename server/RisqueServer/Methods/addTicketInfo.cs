using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
        public JObject run(JObject args) {
            //TODO Implement
            //Utilize TicketStorage.storeTicket
            try {
                Ticket ticket = args.ToObject<Ticket>();
                ticket = ticket;
                return new JObject(new JProperty("success", true), 
                    new JProperty("errorMessage", null));
            }
            catch (Exception e) {
                string message = e.Message;
            }
            
            return null;
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
