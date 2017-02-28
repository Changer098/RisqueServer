using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Security;

namespace RisqueServer {
    /// <summary>
    /// Executes Scripts
    /// </summary>
    public class ScriptRunner {
        public static readonly string pyModifyScriptLocationWin = @"";
        public static readonly string pyModifyScriptLocationNix = @"";
        public static readonly string pyVerifyScriptLocationWin = @"";
        public static readonly string pyVerifyScriptLocationNix = @"";
        public static readonly string pyTestScriptLocationWin = @"C:\Users\everettr\Desktop\testy.py";
        public static readonly string pyTestScriptLocationNix = @"/home/ONEPURDUE/everettr/testy.py";

        public enum ScriptType {
            Python,
            Ruby                //Hope to support
        }
        //
        public enum RunnableScript {
            modifyScript,           //Script for scheduled modifies
            verifyScript,           //Script for verifying tickets
            testScript              //Simple test script
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
        public static string scriptLoc(RunnableScript script) {
            switch (script) {
                case RunnableScript.modifyScript:
                    if (Extensions.IsLinux) {
                        return pyModifyScriptLocationNix;
                    }
                    else {
                        return pyModifyScriptLocationWin;
                    }
                case RunnableScript.verifyScript:
                    if (Extensions.IsLinux) {
                        return pyVerifyScriptLocationNix;
                    }
                    else {
                        return pyVerifyScriptLocationWin;
                    }
                case RunnableScript.testScript:
                    if (Extensions.IsLinux) {
                        return pyTestScriptLocationNix;
                    }
                    else {
                        return pyTestScriptLocationWin;
                    }
            }
            return null;
        }
        //http://stackoverflow.com/a/11779234
        //jobLocation: absolute location to ticket.json or job.json where job is the generic name for whatever needs to be done
        //Blocks main caller thread
        /*public static void runScript(Scheduler sched, int ticketId,  ScriptType type, RunnableScript toRun, string jobLocation) {
            
            bool canRun = false;
            if (type == ScriptType.Python) {
                
                switch (toRun) {
                    case RunnableScript.modifyScript:
                        Console.WriteLine("Not Implemented! Modify Script");
                        canRun = false;
                        break;
                    case RunnableScript.verifyScript:
                        Console.WriteLine("Not Implemented! Verify Script");
                        canRun = false;
                        break;
                    case RunnableScript.testScript:
                        canRun = true;
                        start.Arguments = string.Format("{0} {1}", scriptLoc(toRun), jobLocation);
                        break;
                    default:
                        Console.WriteLine("Default case called for runScript");
                        break;
                }
            }
            else {
                Console.WriteLine("Not Implemented! Type: {0}", type);
            }
            if (canRun && start != null) {
                Process process = Process.Start(start);
               
                new Task(() => {
                    
                }).Start();
            }   
        }*/
        public static void modifyTicket(int TicketId, Scheduler sched) {
            //try and get job location
            string ticketLocation = sched.getTicketLocation(TicketId);
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonLoc;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //CHANGE IN PRODUCTION
            //start.Arguments = String.Format("{0} {1}", scriptLoc(RunnableScript.modifyScript), ticketLocation);
            start.Arguments = String.Format("{0} {1}", scriptLoc(RunnableScript.testScript), ticketLocation);
            try {
                Process process = Process.Start(start);
                using (StreamReader reader = process.StandardOutput) {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                    sched.completeTicket(TicketId);
                }
            }
            catch (Exception e) {
                string messsage = e.Message;
            }
        }
    }
}
