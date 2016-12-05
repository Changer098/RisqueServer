using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using WebSockets.Server.WebSocket;
using WebSockets.Common;
using System.IO;

namespace RisqueServer {
    class TestSocketHandler : WebSocketService {
        private readonly WebLogger _logger;

        public TestSocketHandler(Stream stream, TcpClient tcpClient, string header, IWebSocketLogger logger)
            : base(stream, tcpClient, header, true, logger)
        {
            _logger = (WebLogger)logger;
        }

        protected override void OnTextFrame(string text) {
            //string response = "Recieved: " + text;
            string response = "Received some shit";
            _logger.Information(this.GetType(),"Recieved " + text);
            base.Send(response);
            _logger.Information(this.GetType(), response);
        }
    }
}
