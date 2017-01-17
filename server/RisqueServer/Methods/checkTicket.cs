using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    //TODO - Implement after all other methods are implemented
    class checkTicket : IRPCMethod {
        Scheduler scheduler;
        public checkTicket(Scheduler scheduler) {

        }
        public JObject run(JObject args) {
            return null;
        }
        public bool usesKeepAlive() {
            return true;
        }
    }
}
