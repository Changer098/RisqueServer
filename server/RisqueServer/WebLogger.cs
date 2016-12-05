using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using WebSockets.Server;
using WebSockets.Server.Http;
using WebSockets.Common;

namespace RisqueServer {
    class WebLogger : IWebSocketLogger {
        public WebLogger() {
            Console.WriteLine("Called Logger Constructor");
        }
        public void Information(Type type, string format, params object[] args) {
            Console.WriteLine("Info: " + String.Format(format, args));
        }
        public void Warning(Type type, string format, params object[] args) {
            Console.WriteLine("Warning: " + String.Format(format, args));
        }
        public void Error(Type type, string format, params object[] args) {
            Console.WriteLine("Error: " + String.Format(format, args));
        }
        public void Error(Type type, Exception exception) {
            Console.WriteLine("Warning: " + exception.Message);
        }
    }
}
