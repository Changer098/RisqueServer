using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;
using RisqueServer.Communication;
using Newtonsoft.Json;

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
                    return new JObject(new JProperty("success", false),
                        new JProperty("failureReason", ComMessages.MethodErrorInvalidArguments));
                }
                else {
                    //return retrieved ticket
                    bool success;
                    Ticket ticket = storage.getTicket(ticketId, out success);
                    if (success) {
                        JObject obj = JObject.FromObject(ticket);
                        obj.Add("success", true);
                        obj.Add("failureReason", null);
                        return obj;
                    }
                    else {
                        return new JObject(new JProperty("success", false),
                            new JProperty("failureReason", "Ticket is not in the system"));
                    }
                    
                }
            }
            catch (Exception e) {
                return new JObject(new JProperty("success", false),
                        new JProperty("failureReason", ComMessages.MethodErrorInvalidArguments));
            }
            
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
