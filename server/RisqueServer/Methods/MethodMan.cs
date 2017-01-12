using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RisqueServer.Tickets;

namespace RisqueServer.Methods {
    /// <summary>
    /// Method Manager, allows calling methods
    /// </summary>
    class MethodMan {
        private TicketStorage storage { get; }
        private Scheduler scheduler { get; }
        private Dictionary<string, IRPCMethod> methodDictionary;

        public static MethodMan getActiveManager { get; private set; }

        public MethodMan(TicketStorage storage, Scheduler scheduler) {
            this.scheduler = scheduler;
            this.storage = storage;
            methodDictionary = new Dictionary<string, IRPCMethod>(4);
            methodDictionary.Add("addTicketInfo", new addTicketInfo(storage));
            methodDictionary.Add("checkTicket", new checkTicket(scheduler));
            methodDictionary.Add("doesTicketExist", new doesTicketExist(storage));
            methodDictionary.Add("getTicketInfo", new getTicketInfo(storage));
            MethodMan.getActiveManager = this;
        }
        public bool isValidMethod(string methodName) {
            try {
                return methodDictionary.ContainsKey(methodName);
            }
            catch {
                return false;
            }
        }
    }
}
