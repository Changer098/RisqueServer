using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WebSockets.Server;
using WebSockets.Server.Http;
using WebSockets.Common;
using System.IO;


namespace RisqueServer.Communication {
    class ServiceFactory : IServiceFactory {
        WebLogger log;
        public ServiceFactory (WebLogger log) {
            this.log = log;
        }
        public IService CreateInstance(ConnectionDetails connectionDetails) {
            switch (connectionDetails.ConnectionType) {
                case ConnectionType.WebSocket:
                    return new SockHandler(connectionDetails.Stream, connectionDetails.TcpClient, connectionDetails.Header, log);
                case ConnectionType.Http:
                    break;
            }
            return new BadRequestService(connectionDetails.Stream, connectionDetails.Header, log);
        }
    }
}
