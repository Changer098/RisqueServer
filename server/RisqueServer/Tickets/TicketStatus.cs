using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    public class TicketStatus {
        //Default Constructor
        public TicketStatus() {
            this.user = "Default";
            this.completed = false;
            this.completionDate = null;
        }
        //Supplied user name
        public TicketStatus(string user) : base() {
            this.user = user;
            //TODO check if valid user
        }
        //Allow for custom SSH credientials
        //Values: "Default" (System stored credientials), Actual user name
        //CASE SENSITIVE
        public string user { get; set; }
        //has the ticket been completed?
        public bool completed { get; set; }
        //NULL if !completed
        //Uses Risque Date Encoding
        public string completionDate { get; set; }
    }
}
