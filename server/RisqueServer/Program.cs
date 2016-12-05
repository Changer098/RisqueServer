using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebSockets.Common;
using WebSockets;
using WebSockets.Server;

namespace RisqueServer {
    class Program {
        //https://www.codeproject.com/articles/57060/web-socket-server
        static void Main(string[] args) {
            WebLogger logger = new WebLogger();
            serviceFactory service = new serviceFactory(logger);
            WebServer server = new WebServer(service, logger);
            server.Listen(8181);
            Console.ReadKey();
        }
    }
}
