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
        public void Remove() {
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
        public void Sort() {
            //Insertion sort
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
