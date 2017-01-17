using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;
using RisqueServer.Communication;

namespace RisqueServer.Methods {
    class getTicketInfo : IRPCMethod {
        TicketStorage storage;
        public getTicketInfo(TicketStorage storage) {
            this.storage = storage;
        }
        public JObject run(JObject args) {
            try {
                int ticketId = args.Value<int>("id");
                if (!storage.containsTicket(ticketId)) {
                    //Does not contain the ticket, send error
                    return new JObject(null,
                        new JProperty("success", false),
                        new JProperty("failureReason", ComMessages.MethodErrorInvalidArguments));
                }
                else {
                    //return retrieved ticket
                    Ticket ticket = storage.getTicket(ticketId);
                    return new JObject(ticket,
                        new JProperty("success", true),
                        new JProperty("failureReason", null));
                }
            }
            catch {
                return new JObject(null,
                        new JProperty("success", false),
                        new JProperty("failureReason", ComMessages.MethodErrorInvalidArguments));
            }
            
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
