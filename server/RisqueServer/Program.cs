using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using WebSockets;
using RisqueServer.Communication;

//https://www.codeproject.com/articles/57060/web-socket-server Reference

namespace RisqueServer {
    class Program {

        ActiveConfig config;
        static void Main(string[] args) {
            Program p = new Program();
            parseArgs(p, args);
            if (p.config == null) {
                Console.WriteLine("Failed to read config file");
                return;
            }
            WebLogger logger = new WebLogger();
            ServiceFactory service = new ServiceFactory(logger);
            WebServer server = new WebServer(service, logger);
            server.Listen(p.config.port);
            Console.ReadKey();
        }
        /// <summary>
        /// Parses Arguments
        /// </summary>
        /// <param name="prog"></param>
        /// <param name="args"></param>
        static void parseArgs(Program prog, string[] args) {
            ParsedConfigFile configFile = null;
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
                        Debug.WriteLine("Failed to read config File location");
                    }
                    configFile = parseConfig(fileName);
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
                if (configFile != null) {
                    prog.config = new ActiveConfig();
                    prog.config.hasConfig = true;
                    prog.config.port = configFile.port;
                    prog.config.portSecure = configFile.portSecure;
                    if (markVerbose) { prog.config.verbose = true; }
                    else { prog.config.verbose = configFile.verbose; }
                }
            }
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
