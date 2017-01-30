using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    public class TicketList {
        Ticket[] list;
        int count = 0;
        int capacity;
        Ticket last = null;
        public TicketList(int capacity) {
            list = new Ticket[capacity];
            this.capacity = capacity;
        }
        public void Add(Ticket tick) {
            if (count == capacity) {
                Ticket[] tmpList = new Ticket[capacity * 2];
                for (int i = 0; i < capacity; i++) {
                    tmpList[i] = list[i];
                }
                capacity = capacity * 2;
                list = tmpList;
                list[count] = tick;
                count += 1;
            }
            list[count] = tick;
            count += 1;
            last = tick;
        }
        public void RemoveLast() {
            //shrink at a quarter full
            if (count == capacity / 4) {
                //shrink
                Ticket[] tmpList = new Ticket[capacity / 2];
                for (int j = 0; j < count; j++) {
                    tmpList[j] = list[j];
                }
                capacity = capacity / 4;
            }
            list[count - 1] = null;
            
        }
        public void Remove(int ticketId) {
            int index;
            if (Contains(ticketId, out index)) {
                /*if (count == capacity / 4) {
                    //shrink
                    Ticket[] tmpList = new Ticket[capacity / 2];
                    for (int j = 0; j < count; j++) {
                        tmpList[j] = list[j];
                    }
                    capacity = capacity / 4;
                }
                list[ticketId] = null;*/
                //remove from list
                if (count == capacity / 4) {
                    Ticket[] tmpList = new Ticket[capacity / 2];
                    for (int i = 0; i < count; i++) {
                         //TODO
                    }
                }
                else {
                    Ticket[] tmpList = new Ticket[capacity];
                    for (int i = 0; i < count; i++) {
                        //TODOS
                    }
                }
                
            }
            else {
                throw new ArgumentException("TicketID does not exist");
            }
        }
        public void Sort() {
            //Insertion sort
            for (int i = 1; i < list.Length; i++) {
                int j = i - 1;
                Ticket temp = list[i];

                while(j >= 0 && temp < list[j]) {
                    list[j + 1] = list[j];
                    j--;
                }

                list[j + 1] = temp;
            }
        }
        public bool isSorted() {
            for (int i = 0; i < count - 1; i++) {
                if (list[i] > list[i+1]) {
                    return false;
                } 
            }
            return true;
        }
        public Ticket getLast() {
            return last;
        }
        public Ticket get(int ticketId) {
            int index;
            if (Contains(ticketId, out index)) {
                return list[index];
            }
            else {
                return null;
            }
        }
        public bool Contains(int tickedId, out int index) {
            for (int i = 0; i < count; i++) {
                if (list[i].ticketID == tickedId) {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        public void printList() {
            for (int i = 0; i < count; i++) {
                Console.WriteLine("ID: {0}, Date: {1}", list[i].ticketID, list[i].date);
            }
        }

        public object this[int i] {
            get {
                if (i >= count || i < 0) {
                    throw new IndexOutOfRangeException();
                }
                return list[i];
            }
            set {
                throw new InvalidOperationException("Can't set from [] operator");
            }
        }
    }
}
