﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using WebSockets;
using RisqueServer.Communication;
using RisqueServer.Methods;

//https://www.codeproject.com/articles/57060/web-socket-server Reference

namespace RisqueServer {
    class Program {

        ActiveConfig config;
        MethodMan methodMan;
        Tickets.TicketStorage ticketStorage;
        Scheduler scheduler;
        
        static void Main(string[] args) {
            Program p = new Program();
            parseArgs(p, args);
            if (p.config == null) {
                Console.WriteLine("Failed to read config file");
                return;
            }
            Debug.WriteLine("Creating logger");
            WebLogger logger = new WebLogger();
            //TODO Properly initialize
            Debug.WriteLine("Initalizing ticketStorage");
            if (p.config.ticketDirectory != null) p.ticketStorage = new Tickets.TicketStorage(p.config.ticketDirectory);
            else p.ticketStorage = new Tickets.TicketStorage();
            //TODO Properly initalize
            Debug.WriteLine("Creating Scheduler");
            p.scheduler = new Scheduler(p.ticketStorage);
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
        static void parseArgs(Program prog, string[] args) {
            Console.WriteLine("Args length: " + args.Length);
            ParsedConfigFile configFile = null;
            string directLocation = null;
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
                    //config file
                    string directoryLocation = String.Empty;
                    try {
                        directoryLocation = args[++i];   //get the string of the next argument
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to read ticket directory location");
                    }
                    directLocation = directoryLocation;
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
                Console.WriteLine("configFile field is not null");
                prog.config = new ActiveConfig();
                prog.config.hasConfig = true;
                prog.config.port = configFile.port;
                prog.config.portSecure = configFile.portSecure;
                prog.config.ticketDirectory = directLocation;
                if (markVerbose) { prog.config.verbose = true; }
                else { prog.config.verbose = configFile.verbose; }
            }
            else {
                Console.WriteLine("configFile field is null");
                prog.config = new ActiveConfig();
                prog.config.hasConfig = false;
                prog.config.port = 8181;
                prog.config.portSecure = 401;
                prog.config.ticketDirectory = null;
            }
            Console.WriteLine("Parsed args");
        }
        static void printHelp() {
            Console.WriteLine("-h --h \t Shows this Help Screen");
            Console.WriteLine("-c --c \t Supply the config file");
            Console.WriteLine("-v --v \t Enables verbose output");
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
