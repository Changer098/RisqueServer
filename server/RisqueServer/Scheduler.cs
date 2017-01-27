using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using RisqueServer.Tickets;

namespace RisqueServer {
    class Scheduler {
        //Compiler complains but loadTickets() initalizes it
        ArrayList scheduledTickets = null;
        TicketStorage storage;
        //Queue<T> immediateQueue
        Thread mainSchedule;
        private readonly object _lockObject = new object();
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
                if ((Ticket)scheduledTickets[i] > (Ticket)scheduledTickets[i + 1]) {
                    return false;
                }
            }
            return true;
        }
        private void loop() {
            //do stuff here
            while (true) {
                TimeSpan sleepTime = new TimeSpan(0);
                bool addTicket = false;
                //Is there anything to do?
                if (this.scheduledTickets.Count != 0) {
                    if ((scheduledTickets[0] as Ticket).date <= DateTime.Now) {
                        //do work
                        Console.WriteLine("Executing Ticket: {0}", (scheduledTickets[0] as Ticket).ticketID);
                        storage.completeTicket((scheduledTickets[0] as Ticket).ticketID);
                        this.scheduledTickets.RemoveAt(0);
                        //scheduledTickets[0] = scheduledTickets[0];
                            
                    }
                    else {
                        sleepTime = (scheduledTickets[0] as Ticket).date - DateTime.Now;
                        //Console.WriteLine("Sleeping for {0} minutes", sleepTime.Minutes);
                    }
                }
                else {
                    //Console.WriteLine("scheduledTickets Count: {0}", this.scheduledTickets.Count);
                    sleepTime = new TimeSpan(0, 1, 0);
                }
                //Console.WriteLine("Schedule sleep for {0} minutes", sleepTime.TotalMinutes);
                lock (_lockObject) {
                    Monitor.Wait(_lockObject, sleepTime);
                }
                //Sleep until there is
            }
        }
        private void loadTickets() {
            Tuple<Ticket, TicketStatus>[] tickets = storage.getTicketDict(this);
            scheduledTickets = new ArrayList(tickets.Count() * 2);
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
            if (scheduledTickets.Count == 0) {
                //nothing in the list
                scheduledTickets.Add(tick);
                Console.WriteLine("Added ticket");
                //mainSchedule.Interrupt();
                lock (_lockObject) {
                    //Wake up
                    Monitor.Pulse(_lockObject);
                }
            }
            else if (scheduledTickets.Count != 0) {
                if (tick.dueBy.fromRisqueTime() < (scheduledTickets[scheduledTickets.Count - 1] as Ticket).dueBy.fromRisqueTime()) {
                    scheduledTickets.Add(tick);
                    Console.WriteLine("Added Ticket");
                    new Task(() => {
                        scheduledTickets.Sort();
                        lock (_lockObject) {
                            //Wake up
                            Monitor.Pulse(_lockObject);
                        }
                        Console.WriteLine("Sorted tickets");
                    }).Start();
                }
                else {
                    Console.WriteLine("Added ticket");
                    scheduledTickets.Add(tick);
                }
            }
            else {
                Console.WriteLine("Dunno");
            }
            
        }
    }
}
