using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generators {
    public static class Extensions {
        public static byte[] ToByteArray(this string s) {
            byte[] array = new byte[s.CountChar(' ') + 1];
            string[] split = s.Split(' ');
            for (int i = 0; i < array.Length; i++) {
                array[i] = byte.Parse(split[i]);
            }
            return array;
        }
        public static int CountChar(this string s, char c) {
            int count = 0;
            for (int i = 0; i < s.Length; i++) {
                if (s[i] == c) {
                    count = count + 1;
                }
            }
            return count;
        }
    }
}
