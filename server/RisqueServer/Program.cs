//nohup


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using WebSockets;
using RisqueServer.Communication;
using RisqueServer.Methods;
using RisqueServer.Security;

//https://www.codeproject.com/articles/57060/web-socket-server Reference

namespace RisqueServer {
    class Program {

        ActiveConfig config;
        MethodMan methodMan;
        Tickets.TicketStorage ticketStorage;
        Scheduler scheduler;
        SecurityManager securityMan;
        
        static void Main(string[] args) {
            string key = null, iv = null;
            Program p = new Program();
            parseArgs(p, args);
            if (p.config == null) {
                Console.WriteLine("Failed to read config file");
                return;
            }
            //Read key and iv
            Console.WriteLine("Enter Encryption key (base64)");
            key = Console.ReadLine();
            Console.WriteLine("Enter Encryption IV (base64)");
            iv = Console.ReadLine();
            Console.WriteLine();
            p.securityMan = new SecurityManager(p.config.keyFileLocation, key, iv, p.config.userFileLocation, p.config.emailUserFileLocation);

            Debug.WriteLine("Creating logger");
            WebLogger logger = new WebLogger();
            //TODO Properly initialize
            Debug.WriteLine("Initalizing ticketStorage");
            if (p.config.ticketDirectory != null) p.ticketStorage = new Tickets.TicketStorage(p.config.ticketDirectory);
            else p.ticketStorage = new Tickets.TicketStorage();
            //TODO Properly initalize
            Debug.WriteLine("Creating Scheduler");
            p.scheduler = new Scheduler(p.ticketStorage, p.securityMan);
            Debug.WriteLine("Creating Method Manager");
            p.methodMan = new MethodMan(p.ticketStorage, p.scheduler);
            Debug.WriteLine("Creating Service Factory");
            ServiceFactory service = new ServiceFactory(logger, p.methodMan);
            Debug.WriteLine("Creating Web Server");
            WebServer server = new WebServer(service, logger);
            server.Listen(p.config.port);
            Debug.WriteLine("Main() is listening");
            Console.ReadKey();
        }
        /// <summary>
        /// Parses Arguments
        /// </summary>
        /// <param name="prog"></param>
        /// <param name="args"></param> 
        
        //ABSOLUTE MESS
        static void parseArgs(Program prog, string[] args) {
            Console.WriteLine("Args length: " + args.Length);
            ParsedConfigFile configFile = null;
            string directoryLocation = null, keyFileLoc = null, userFileLoc = null;
            bool markVerbose = false;
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("-c", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--c", StringComparison.CurrentCultureIgnoreCase)) {
                    //config file
                    string fileName = String.Empty;
                    try {
                        fileName = args[++i];   //get the string of the next argument
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to read config File location");
                    }
                    configFile = parseConfig(fileName);
                }
                else if (args[i].Equals("-t", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--t", StringComparison.CurrentCultureIgnoreCase)) {
                    //directory location
                    string directLocation = String.Empty;
                    try {
                        directLocation = args[++i];   //get the string of the next argument
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to read ticket directory location");
                    }
                    directoryLocation = directLocation;
                }
                else if (args[i].Equals("-k", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--k", StringComparison.CurrentCultureIgnoreCase)) {
                    //Keyfile location
                    string loc = String.Empty;
                    try {
                        loc = args[++i];   //get the string of the next argument
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to read keyFile location");
                    }
                    keyFileLoc = loc;
                }
                else if (args[i].Equals("-u", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--u", StringComparison.CurrentCultureIgnoreCase)) {
                    //Userfile location
                    string loc = String.Empty;
                    try {
                        loc = args[++i];   //get the string of the next argument
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to read userFile location");
                    }
                    userFileLoc = loc;
                }
                else if (args[i].Equals("-h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--h", StringComparison.CurrentCultureIgnoreCase)) {
                    printHelp();
                    System.Environment.Exit(0);
                }
                else if (args[i].Equals("-v", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("--v", StringComparison.CurrentCultureIgnoreCase)) {
                    //verbose output. OVERRIDES CONFIG Settings
                    markVerbose = true;
                }
            }
            if (configFile != null) {
                //Console.WriteLine("configFile field is not null");
                prog.config = new ActiveConfig();
                prog.config.hasConfig = true;
                prog.config.port = configFile.port;
                prog.config.portSecure = configFile.portSecure;
                if (directoryLocation != null) {
                    //use supplied Ticket Directory instead of the one in the config
                    prog.config.ticketDirectory = directoryLocation;
                }
                else {
                    prog.config.ticketDirectory = configFile.ticketDirectory;
                }
                if (keyFileLoc != null) {
                    //use supplied Key File instead of the one in the config
                    prog.config.keyFileLocation = keyFileLoc;
                }
                else {
                    prog.config.keyFileLocation = configFile.keyFileLocation;
                }
                if (userFileLoc != null) {
                    //use supplied User File instead of the one in the config
                    prog.config.userFileLocation = userFileLoc;
                }
                else {
                    prog.config.userFileLocation = configFile.userFileLocation;
                }

                prog.config.emailUserFileLocation = configFile.emailUserFileLocation;
                prog.config.verbose = configFile.verbose | markVerbose; //If either is true, equal to true
                if (markVerbose) { prog.config.verbose = true; }
                else { prog.config.verbose = configFile.verbose; }
            }
            else {
                //Console.WriteLine("configFile field is null");
                prog.config = new ActiveConfig();
                prog.config.hasConfig = false;
                prog.config.port = 8181;
                prog.config.portSecure = 401;
                prog.config.ticketDirectory = null;
                prog.config.keyFileLocation = keyFileLoc;
                prog.config.userFileLocation = userFileLoc;
                prog.config.emailUserFileLocation = null;
            }
            Console.WriteLine("Parsed args");
        }
        static void printHelp() {
            Console.WriteLine("-h --h \t Shows this Help Screen");
            Console.WriteLine("-c --c \t Supply the config file");
            Console.WriteLine("-v --v \t Enables verbose output");
            Console.WriteLine("-u --u \t Supply the location to the UserFile");
            Console.WriteLine("-k --k \t Supply the location to the keyFile");
        }
        static ParsedConfigFile parseConfig(string fileName) {
            ParsedConfigFile parsed;
            try {
                parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<ParsedConfigFile>(File.ReadAllText(fileName));
            }
            catch (Exception e) {
                Console.WriteLine("Failed to parse Config file. Reason " + e.Message);
                return null;
            }
            return parsed;
        }
    }
}
