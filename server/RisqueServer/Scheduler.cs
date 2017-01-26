using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RisqueServer.Tickets;

namespace RisqueServer {
    class Scheduler {
        //Compiler complains but loadTickets() initalizes it
        List<Ticket> scheduledTickets = null;
        TicketStorage storage;
        //Queue<T> immediateQueue
        Thread mainSchedule;
        public Scheduler(TicketStorage storage) {
            mainSchedule = new Thread(loop);
            this.storage = storage;
            this.storage.registerScheduler(this);
            loadTickets();
            mainSchedule.Start();
        }
        //Check if the queue is sorted
        private bool isSorted() {
            for (int i = 0; i < scheduledTickets.Count - 1; i++) {
                if (scheduledTickets[i] > scheduledTickets[i + 1]) {
                    return false;
                }
            }
            return true;
        }
        private void loop() {
            //do stuff here
            while (true) {
                TimeSpan sleepTime = new TimeSpan(0);
                try {
                    //Is there anything to do?
                    if (scheduledTickets.Count != 0) {
                        if (scheduledTickets[0].date <= DateTime.Now) {
                            //do work
                            Console.WriteLine("Executing Ticket: {0}", scheduledTickets[0].ticketID);
                            scheduledTickets.RemoveAt(0);
                            scheduledTickets[0] = scheduledTickets[0];
                            
                        }
                    }
                    else {
                        sleepTime = new TimeSpan(0, 5, 0);
                    }
                    Thread.Sleep(sleepTime);
                    //Sleep until there is
                }
                catch (ThreadInterruptedException e) {
                    //Wake up and do stuff
                    continue;
                }
                catch (ThreadAbortException e) {
                    //Kill thread
                }
            }
        }
        private void loadTickets() {
            Tuple<Ticket, TicketStatus>[] tickets = storage.getTicketDict(this);
            scheduledTickets = new List<Ticket>(tickets.Count());
            foreach (Tuple<Ticket, TicketStatus> ticket in tickets) {
                if (!ticket.Item2.completed) {
                    scheduledTickets.Add(ticket.Item1);
                }
            }
            scheduledTickets.Sort();
        }
        /// <summary>
        /// Adds a ticket to the list of tickets
        /// </summary>
        /// <param name="tick">Ticket to add</param>
        public void storeTicket(Ticket tick) {
            if (scheduledTickets[0] == null) {
                //nothing in the list
                scheduledTickets.Add(tick);
                mainSchedule.Interrupt();
            }
            else if (tick.dueBy.fromRisqueTime() < scheduledTickets.Last().dueBy.fromRisqueTime()) {
                scheduledTickets.Add(tick);
                new Task(() => {
                    scheduledTickets.Sort();
                    mainSchedule.Interrupt();
                }).Start();
            }
            else {
                scheduledTickets.Add(tick);
            }
        }
    }
}
