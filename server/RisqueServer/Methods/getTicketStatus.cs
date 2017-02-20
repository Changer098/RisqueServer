using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RisqueServer.Tickets;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    class getTicketStatus : IRPCMethod {
        TicketStorage storage;
        public getTicketStatus(TicketStorage storage) {
            this.storage = storage;
        }
        public JObject run(JObject args) {
            try {
                int ticketId = args.Value<int>("id");
                bool success;
                TicketStatus status = storage.getTicketStatus(ticketId, out success);
                if (success) {
                    return new JObject(new JProperty("completed", status.completed),
                        new JProperty("completionDate", status.completionDate),
                        new JProperty("user", status.user),
                        new JProperty("success", true),
                        new JProperty("failureReason", null));
                }
                else {
                    return new JObject(new JProperty("completed", null),
                        new JProperty("completionDate", null),
                        new JProperty("user", null),
                        new JProperty("success", false),
                        new JProperty("failureReason", "Ticket is not in the system"));
                }
            }
            catch (Exception e) {
                return new JObject(new JProperty("completed", false),
                    new JProperty("completionDate", String.Empty),
                    new JProperty("user", String.Empty),
                    new JProperty("success", false),
                    new JProperty("failureReason", e.Message));
            }
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
