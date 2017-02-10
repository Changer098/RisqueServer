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
            return new JObject(new JProperty("completed", false),
                new JProperty("completionDate", String.Empty),
                new JProperty("user", String.Empty),
                new JProperty("success", false),
                new JProperty("failureReason", String.Empty));
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
