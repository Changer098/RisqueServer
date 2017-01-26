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
        public static string toRisqueTime(this DateTime dt) {
            //Midnight is 12am, Midday is 12pm
            StringBuilder builder = new StringBuilder();
            bool isPM = (dt.Hour - 12 > 0);
            string meridiem;
            if (isPM) {
                meridiem = "PM";
            }
            else {
                meridiem = "AM";
            }
            builder.AppendFormat("{0}/{1}/{2} {3}:{4} {5}", dt.Month, dt.Day, dt.Year, dt.Minute, dt.Hour, meridiem);
            return builder.ToString();
        }
        public static DateTime fromRisqueTime(this string s) {
            s = s.Trim();
            string toParse;
            if (s.Last<char>() == ')') {
                //get rid of the weird "(s)" formatting at the end
                toParse = s.Substring(0, s.Length - 3);
            }
            else {
                toParse = s;
            }
            return DateTime.Parse(toParse);
        }
        //Are we running on Linux?
        public static bool IsLinux {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}
