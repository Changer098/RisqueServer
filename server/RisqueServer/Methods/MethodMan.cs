using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RisqueServer.Tickets;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    /// <summary>
    /// Method Manager, allows calling methods
    /// </summary>
    class MethodMan {
        private TicketStorage storage;
        private Scheduler scheduler;
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
            methodDictionary.Add("getScheduledCount", new getScheduledCount(scheduler));
            methodDictionary.Add("getTicketStatus", new getTicketStatus(storage));
            methodDictionary.Add("removeTicket", new removeTicket(storage));
            MethodMan.getActiveManager = this;
        }
        /// <summary>
        /// Determines if a method exists or not
        /// </summary>
        /// <param name="methodName">The name of the method being searched</param>
        /// <returns>True if the method exists, false otherwise</returns>
        public bool isValidMethod(string methodName) {
            try {
                return methodDictionary.ContainsKey(methodName);
            }
            catch {
                return false;
            }
        }
        /// <summary>
        /// Checks if a method is Asynchronous or not
        /// </summary>
        /// <param name="methodName">Name of method being checked</param>
        /// <returns>True if a method is Asynchronous and false if it's not</returns>
        /// <exception cref="Exception">Throws an Exception if methodDictionary cannot retrive the method</exception>
        public bool isAsyncMethod(string methodName) {
            try {
                IRPCMethod method;
                bool getValue = methodDictionary.TryGetValue(methodName, out method);
                if (!getValue) {
                    throw new Exception("Method not found!");
                }
                return method.usesKeepAlive();
            }
            catch {
                throw new Exception("Unknown error occurred");
            }
        }
        //Assumes that isValidMethod has already been called
        /// <summary>
        /// Executes a given RPC method
        /// </summary>
        /// <param name="methodName">Name of method being called</param>
        /// <param name="result">JSON formatted result from method</param>
        /// <returns>True if a method executed, false if it didn't</returns>
        public bool callMethod(string methodName, out string result, JObject obj) {
            try {
                IRPCMethod method;
                bool getValue = methodDictionary.TryGetValue(methodName, out method);
                if (method.usesKeepAlive()) {
                    result = Communication.ComMessages.MethodErrorIsAsync;
                    return false;
                }
                else {
                    JObject output = method.run(obj);
                    if (output == null) {
                        result = Communication.ComMessages.MethodErrorGeneric;
                        return false;
                    }
                    else {
                        result = output.ToString();
                        return true;
                    }
                }
            }
            catch (Exception e) {
                result = Communication.ComMessages.ErrorNotValidMethod;
                return false;
            }
        }
        //Assumes that isValidMethod has already been called
        //NonAsync methods can be run asynchronously 
        public bool CallMethodAsync(string methodName, RisqueServer.Communication.WebSocketService service, JObject obj) {
            return false;
        }
    }
}
