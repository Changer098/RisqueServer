using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer {
    //Contains extension classes
    public static class Extensions {
        //Source: http://stackoverflow.com/a/2567623
        public static int CountLines(this string s) {
            int n = 0;
            foreach (var c in s) {
                if (c == '\n') n++;
            }
            return n + 1;
        }

        //Modification of CountLines
        //Return true if a string contains at least two lines
        public static bool AtleastTwoLines(this string s) {
            int n = 0;
            foreach (var c in s) {
                if (c == '\n') n++;
                if (n >= 2) return true;
            }
            return false;
        }
    }
}
