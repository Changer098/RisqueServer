using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    class removeTicket : IRPCMethod {
        Tickets.TicketStorage storage;
        public removeTicket(Tickets.TicketStorage storage) {
            this.storage = storage;
        }
        public JObject run(JObject args) {
            int ticketId;
            try {
                ticketId = args.Value<int>("id");
            }
            catch {
                return new JObject(new JProperty("exists", false),
                    new JProperty("success", false),
                    new JProperty("failureReason", "Method contained invalid arguments"));
            }
            if (storage.removeTicket(ticketId)) {
                return new JObject(new JProperty("success", true),
                    new JProperty("failureReason", null));
            }
            else {
                return new JObject(new JProperty("success", false),
                    new JProperty("failureReason", "Does not contain ticket"));
            }
        }

        public bool usesKeepAlive() {
            return false;
        }
    }
}
