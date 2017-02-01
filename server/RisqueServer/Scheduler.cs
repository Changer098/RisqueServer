﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using RisqueServer.Tickets;

namespace RisqueServer {
    class Scheduler {
        //Compiler complains but loadTickets() initalizes it
        TicketList scheduledTickets = null;
        DateTime StartTime;     //5:30PM
        DateTime EndTime;       //11:55PM
        TicketStorage storage;
        //Queue<T> immediateQueue
        Thread mainSchedule;
        private readonly object _lockObject = new object();
        public Scheduler(TicketStorage storage) {
            StartTime = new DateTime(1, 1, 1, 12, 40, 0);
            EndTime = new DateTime(1, 1, 1, 12, 45, 0);
            mainSchedule = new Thread(loop);
            this.storage = storage;
            this.storage.registerScheduler(this);
            loadTickets();
            mainSchedule.Start();
        }
        //Check if the queue is sorted
        private bool isSorted() {
            for (int i = 0; i < scheduledTickets.count - 1; i++) {
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
                if (this.scheduledTickets.count != 0) {
                    if (scheduledTickets[0] == null) {
                        Debug.WriteLine("First ticket is null!");
                        sleepTime = new TimeSpan(0, 1, 0);
                    }
                    else {
                        if ((scheduledTickets[0] as Ticket).date <= DateTime.Now) {
                            if (inOperatingHours()) {
                                //do work
                                Ticket toExecute = (scheduledTickets[0] as Ticket);
                                Console.WriteLine("Executing Ticket: {0}", toExecute.ticketID);
                                storage.completeTicket(toExecute.ticketID);
                                this.scheduledTickets.Remove(toExecute.ticketID);
                                continue;
                            }
                            else {
                                Debug.WriteLine("not in Operating Hours");
                                //calculate how long until operating hours
                                int hours, minutes;
                                hours = DateTime.Now.Hour - StartTime.Hour;
                                minutes = DateTime.Now.Minute - StartTime.Minute;
                                TimeSpan actual = new TimeSpan(Math.Abs(hours), Math.Abs(minutes), 0);
                                sleepTime = actual;
                            }
                        }
                        else {
                            sleepTime = (scheduledTickets[0] as Ticket).date - DateTime.Now;
                            //Console.WriteLine("Sleeping for {0} minutes", sleepTime.Minutes);
                        }
                    }
                }
                else {
                    //Console.WriteLine("scheduledTickets Count: {0}", this.scheduledTickets.Count);
                    sleepTime = new TimeSpan(1, 0, 0);
                }
                Console.WriteLine("Schedule sleep for {0} minutes", sleepTime.TotalMinutes);
                lock (_lockObject) {
                    Monitor.Wait(_lockObject, sleepTime);
                }
                //Sleep until there is
            }
        }
        private bool inOperatingHours() {
            DateTime Now = DateTime.Now;
            if (Now.Hour > StartTime.Hour) {
                //check endTime
                if (Now.Hour < EndTime.Hour) {
                    return true;
                }
                else if (Now.Hour == EndTime.Hour && Now.Minute < EndTime.Minute) {
                    return true;
                }
                return false;
            }
            else if (Now.Hour == StartTime.Hour && Now.Minute >= StartTime.Minute) {
                //check endTime
                if (Now.Hour < EndTime.Hour) {
                    return true;
                }
                else if (Now.Hour == EndTime.Hour && Now.Minute < EndTime.Minute) {
                    return true;
                }
                return false;
            }
            return false;
        }
        private void loadTickets() {
            Tuple<Ticket, TicketStatus>[] tickets = storage.getTicketDict(this);
            scheduledTickets = new TicketList(tickets.Count() * 2);
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
            if (scheduledTickets.count == 0) {
                //nothing in the list
                scheduledTickets.Add(tick);
                Console.WriteLine("Added ticket");
                //mainSchedule.Interrupt();
                lock (_lockObject) {
                    //Wake up
                    Monitor.Pulse(_lockObject);
                }
            }
            else if (scheduledTickets.count != 0) {
                scheduledTickets = scheduledTickets;
                if (tick.dueBy.fromRisqueTime() < (scheduledTickets[scheduledTickets.count - 1] as Ticket).dueBy.fromRisqueTime()) {
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
