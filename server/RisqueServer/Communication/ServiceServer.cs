using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace RisqueServer.Communication {
    class ServiceServer {
        public const int servicePort = 9090;                //Really shouldn't do this
        private const string secCode = "umH8h3amsIljvMIGqL2N";
        Security.SecurityManager securityManager;
        Thread listenerThread;
        TcpListener listener;
        public ServiceServer(RisqueServer.Security.SecurityManager secManager) {
            this.securityManager = secManager;
            listener = new TcpListener(System.Net.IPAddress.Loopback, servicePort);
            listener.Start();
            //create listener thread
            listenerThread = new Thread(() => {
                Console.WriteLine("Service Server started listening");
                while (true) {
                    try {
                        TcpClient connected = listener.AcceptTcpClient();
                        if (connected.Connected) {
                            //read data
                            //Console.WriteLine("Recieved client");
                            using (StreamReader reader = new StreamReader(connected.GetStream())) {
                                using (StreamWriter writer = new StreamWriter(connected.GetStream())) {
                                    writer.AutoFlush = true;            //Ultimate smartz

                                    string message = reader.ReadLine();
                                    //Console.WriteLine("Read: " + message);
                                    string[] csvSplit = message.Split(',');
                                    //get method
                                    string[] methodSplit = csvSplit[0].Split('=');

                                    if (methodSplit[1] == null) {
                                        Console.WriteLine("Request contains no Method Value");
                                        writer.WriteLine("Returns=failure, ErrorMessage=Request contained no Method Value");
                                    }
                                    else if (methodSplit[1].Equals("Stop", StringComparison.CurrentCultureIgnoreCase)) {
                                        methodSplit = csvSplit[1].Split('=');
                                        if (methodSplit[1] == null) {
                                            Console.WriteLine("Recieved shutdown request with improperly formatted arguments");
                                            writer.WriteLine("Returns=failure, ErrorMessage=Stop Method recieved improperly formatted arguments");
                                        }
                                        else if (methodSplit[1] == secCode) {
                                            Console.WriteLine("Recieved Shutdown request, shutting down");
                                            writer.WriteLine("Returns=success, ErrorMessage=");
                                            secManager.sendShutdown("Recieved Stop Command");
                                            Environment.Exit(0);
                                        }
                                        else {
                                            Console.WriteLine("Recieved Shutdown request with incorrect security value");
                                            writer.WriteLine("Returns=failure, ErrorMessage=Security Value is incorrect");
                                        }
                                    }
                                    else {
                                        writer.WriteLine("Returns=failure, ErrorMesage=Method not found");
                                        Console.WriteLine("Not supported");
                                    }
                                }
                            }
                            //Console.WriteLine("Closing client");
                            connected.Close();
                        }
                        else {
                            Console.WriteLine("Recieved a non-connceted tcp client");
                        }
                    }
                    catch {
                        //Console.WriteLine(e.Message);
                        //Console.WriteLine(e.StackTrace);
                    }
                }
                
            });
            listenerThread.Start();
        }
    }
}
