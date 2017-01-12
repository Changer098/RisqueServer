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
        public JProperty[] run(JProperty[] args) {
            return null;
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
