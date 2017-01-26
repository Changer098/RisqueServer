using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RisqueServer {
    /// <summary>
    /// Executes Scripts
    /// </summary>
    class Runner {
        public enum Scripts {
            ConfigTicket,

        }
        public static string pythonLoc { get {
                if (Extensions.IsLinux) {
                    return "/usr/bin/python";
                }
                else {
                    Debug.WriteLine("Requested Python Location on Windows.");
                    return "C:/Python27/python.exe";
                }
            }
        }
        //http://stackoverflow.com/a/11779234
    }
}
