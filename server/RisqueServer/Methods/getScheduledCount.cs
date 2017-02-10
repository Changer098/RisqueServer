using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    class getScheduledCount : IRPCMethod {
        Scheduler scheduler;
        getScheduledCount(Scheduler scheduler) {
            this.scheduler = scheduler;
        }
        public JObject run(JObject args) {
            int[] arr = { 1, 2, 3 };
            return new JObject(new JProperty("count", 0),
                new JProperty("ticketIds", arr),
                new JProperty("success", false),
                new JProperty("failureReason", String.Empty));
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
