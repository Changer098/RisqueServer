using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;

namespace RisqueServer.Methods {
    class doesTicketExist : IRPCMethod {

        TicketStorage storage;
        public doesTicketExist(TicketStorage storage) {
            this.storage = storage;
        }
        public JObject run(JObject args) {
            //"returns" : {
            //"exists" : boolean,
            //Will be NULL
            //"success" : boolean,
            //"failureReason" : String
            int ticketId;
            try {
                ticketId = args.Value<int>("id");
            }
            catch {
                return new JObject(new JProperty("exists", false), new JProperty("success", false), new JProperty("failureReason", Communication.ComMessages.MethodErrorInvalidArguments));
            }
            if (storage.containsTicket(ticketId)) {
                return new JObject(new JProperty("exists", true), new JProperty("success", true), new JProperty("failureReason", null));
            }
            else {
                return new JObject(new JProperty("exists", false), new JProperty("success", true), new JProperty("failureReason", null));
            }
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
