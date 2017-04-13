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
            int hour = dt.Hour;
            string meridiem;
            if (isPM) {
                meridiem = "PM";
            }
            else {
                meridiem = "AM";
            }
            if (hour != 12) {
                if (hour - 12 > 0) hour = hour - 12;
            }
            builder.AppendFormat("{0}/{1}/{2} {3}:{4} {5}", dt.Month, dt.Day, dt.Year, dt.Minute, hour, meridiem);
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
        public static int CountChar (this string s, char c) {
            int count = 0;
            for (int i = 0; i < s.Length; i++) {
                if (s[i] == c) {
                    count = count + 1;
                }
            }
            return count;
        }
        public static DateTime decrement(this DateTime self) {
            int days = self.Day;
            int month = self.Month;
            int year = self.Year;
            days = days - 1;
            if (days <= 0) {
                //decrease month
                month = month - 1;
                if (month <= 0) {
                    //decrease year
                    year = year - 1;
                    if (year <= 0) {
                        return DateTime.MinValue;
                    }
                    month = 12;
                    days = DaysInMonth(month, isLeapYear(year));
                }
                else {
                    days = DaysInMonth(month, isLeapYear(year));
                }
            }
            return new DateTime(year, month, days, self.Hour, self.Minute, self.Second);
        }
        public static int DaysInMonth(int month, bool isLeap) {
            switch (month) {
                case 2:
                    if (isLeap) {
                        return 28;
                    }
                    else {
                        return 29;
                    }
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                default:
                    return -1;
            }
        }
        //https://support.microsoft.com/en-us/help/214019/method-to-determine-whether-a-year-is-a-leap-year
        public static bool isLeapYear(int year) {
            if (year % 4 == 0) {
                if (year % 100 == 0) {
                    if (year % 400 == 0) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }
    }
}
