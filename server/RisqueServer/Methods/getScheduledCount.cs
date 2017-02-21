using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RisqueServer.Tickets;

namespace RisqueServer.Methods {
    class getScheduledCount : IRPCMethod {
        Scheduler scheduler;
        public getScheduledCount(Scheduler scheduler) {
            this.scheduler = scheduler;
        }
        public JObject run(JObject args) {
            try {
                TicketList list = scheduler.scheduledTickets;
                int[] ticketIds = new int[list.count];
                for (int i = 0; i < list.count; i++) {
                    ticketIds[i] = (list[i] as Ticket).ticketID;
                }
                return new JObject(new JProperty("count", 0),
                    new JProperty("ticketIds", ticketIds),
                    new JProperty("success", false),
                    new JProperty("failureReason", String.Empty));
            }
            catch (Exception e) {
                int[] blankList = new int[0];
                return new JObject(new JProperty("count", 0),
                    new JProperty("ticketIds", blankList),
                    new JProperty("success", false),
                    new JProperty("failureReason", e.Message));
            }
        }
        public bool usesKeepAlive() {
            return false;
        }
    }
}
