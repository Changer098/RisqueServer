using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using ServiceProgs.Common;

namespace ServiceProgs {
    class Stopper {
        static void Main(string[] args) {
            const string code = "umH8h3amsIljvMIGqL2N";                 //Hardcoded because lazy

            Console.WriteLine("Attempting to stop Server on 127.0.0.1:" + Consts.servicePort);
            try {
                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Loopback, Consts.servicePort));
                if (client.Connected) {
                    using (StreamWriter writer = new StreamWriter(client.GetStream())) {
                        using (StreamReader reader = new StreamReader(client.GetStream())) {
                            writer.AutoFlush = true;
                            writer.WriteLine("Method=Stop, code=" + code);
                            writer.Flush();
                            //Console.WriteLine("Requested Stop");
                            string result = reader.ReadLine();
                            //Console.WriteLine("Recieved=" + result);
                            string[] resultSplit = result.Split(',');
                            string[] keyValue = resultSplit[0].Split('=');
                            if (keyValue[1] != null && keyValue[1].Equals("success", StringComparison.CurrentCultureIgnoreCase)) {
                                Console.WriteLine("Successfully stopped Server");
                            }
                            else {
                                Console.WriteLine("Failed to stop server. Error Message:");
                                if (resultSplit[1] != null) {
                                    keyValue = resultSplit[1].Split('=');
                                    if (keyValue[1] != null) {
                                        Console.WriteLine(keyValue[1]);
                                    }
                                    else {
                                        Console.WriteLine("INVALID PROTOCOL. Couldn't parse the Error Message.");
                                    }
                                }
                                else {
                                    Console.WriteLine("INVALID PROTOCOL. Didn't return an Error Message.");
                                }
                            }
                        }
                    }
                }
                else {
                    Console.WriteLine("ERROR, Connect() finished but the TCP Client isn't actually connected");
                }
            }
            catch (Exception e) {
                Console.WriteLine("Caught an exception: " + e.Message);
            }
        }
    }
}
