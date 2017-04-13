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
            //Utilize TicketStorage.storeTicket
            try {
                Ticket ticket = args.ToObject<Ticket>();
                if (!ticket.isScheduled) {
                    return new JObject(new JProperty("success", false), new JProperty("failureReason",
                        "Ticket is not a Scheduled Ticket, cannot handle!"));
                }
                string failureReason;
                if (storage.storeTicket(ticket, out failureReason)) {
                    return new JObject(new JProperty("success", true), new JProperty("failureReason", failureReason));
                }
                else {
                    return new JObject(new JProperty("success", false), new JProperty("failureReason", failureReason));
                }
            }
            catch (Exception e) {
                return new JObject(new JProperty("success", false), new JProperty("failureReason", String.Format("Server Exception: {0}", e.Message)));
            }
            
            //return null;
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
