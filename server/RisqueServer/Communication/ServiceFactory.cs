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
        Methods.MethodMan methodMan;
        public ServiceFactory (WebLogger log, Methods.MethodMan methodMan) {
            this.log = log;
            this.methodMan = methodMan;
        }
        public IService CreateInstance(ConnectionDetails connectionDetails) {
            switch (connectionDetails.ConnectionType) {
                case ConnectionType.WebSocket:
                    return new SockHandler(connectionDetails.Stream, connectionDetails.TcpClient, connectionDetails.Header, log, methodMan);
                case ConnectionType.Http:
                    break;
            }
            return new BadRequestService(connectionDetails.Stream, connectionDetails.Header, log);
        }
    }
}
