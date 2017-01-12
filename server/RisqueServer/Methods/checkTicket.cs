using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    class checkTicket : IRPCMethod {
        Scheduler scheduler;
        public checkTicket(Scheduler scheduler) {

        }
        public JProperty[] run(JProperty[] args) {
            return null;
        }
        public bool usesKeepAlive() {
            return true;
        }
    }
}
