using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace RisqueServer.Tickets {
    public class TicketList {
        Ticket[] list;
        public int count = 0;
        public int capacity;
        Ticket last = null;
        public TicketList(int capacity) {
            if (capacity < 2) this.capacity = 2;
            else { this.capacity = capacity; }
            list = new Ticket[this.capacity];
        }
        public void Add(Ticket tick) {
            if (count == (capacity - 1)) {
                Ticket[] tmpList = new Ticket[capacity * 2];
                for (int i = 0; i < capacity; i++) {
                    tmpList[i] = list[i];
                }
                capacity = capacity * 2;
                list = tmpList;
            }
            list[count] = tick;
            count = count + 1;
            //last = tick;
            //list = list;
        }
        public void RemoveLast() {
            if (count == 0) throw new Exception("No elements in list");
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
            count = count - 1;
        }
        public void RemoveFirst() {
            if (count == 0) throw new Exception("No elements in list");
            Ticket[] tmpList;
            if (count == capacity / 4) {
                tmpList = new Ticket[capacity];
            }
            else {
                tmpList = new Ticket[capacity];
            }
            for (int i = 1; i < count; i++) {
                tmpList[i] = list[i];
            }
            count = count - 1;
            list = tmpList;
        }
        public void RemoveAt(int index) {
            if (count == 0) throw new Exception("No elements in list");
            if (index >= count) throw new IndexOutOfRangeException();
            Ticket[] tmpList;
            if (count == capacity / 4) {
                tmpList = new Ticket[capacity / 4];
            }
            else {
                tmpList = new Ticket[capacity / 4];
            }
            int j = 0;
            for (int i = 0; i < count; i++) {
                if (i == index) {
                    continue;
                }
                try {
                    tmpList[j] = list[i];
                }
                catch (Exception e) {

                }
                j = j + 1;
            }
            list = tmpList;
        }

        public void Remove(int ticketId) {
            if (count == 0) throw new Exception("No elements in list");
            int index;
            if (Contains(ticketId, out index)) {
                //remove from list
                Ticket[] tmpList;
                if (count == capacity / 4) {
                    tmpList = new Ticket[capacity / 2];
                }
                else {
                    tmpList = new Ticket[capacity];
                }
                int j = 0, removed = 0;
                for (int i = 0; i < count; i++) {
                    //TODO
                    if (list[i].ticketID == ticketId) {
                        removed = removed + 1;
                        continue;
                    }
                    tmpList[j] = list[i];
                    j = j + 1;
                }
                list = tmpList;
                count = count - removed;
            }
            else {
                throw new ArgumentException("TicketID does not exist");
            }
        }
        public void Sort() {
            //Insertion sort
            if (count == 0 ||  count == 1) return;
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
            Console.WriteLine("Printing list");
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
