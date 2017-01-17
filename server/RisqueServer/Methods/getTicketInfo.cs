using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;

namespace RisqueServer.Methods {
    class getTicketInfo : IRPCMethod {
        TicketStorage storage;
        public getTicketInfo(TicketStorage storage) {
            this.storage = storage;
        }
        public JObject run(JObject args) {
            return null;
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
