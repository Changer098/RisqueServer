using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    public class TicketStatus {
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
