using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using WebSockets.Server.WebSocket;
using WebSockets.Common;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Communication {
    class SockHandler : WebSocketService {
        private readonly WebLogger _logger;

        public SockHandler(Stream stream, TcpClient tcpClient, string header, IWebSocketLogger logger)
            : base(stream, tcpClient, header, true, logger)
        {
            _logger = (WebLogger)logger;
        }

        protected override void OnTextFrame(string text) {

            //TODO Fix handling of no json content, currently breaks everything

            //string response = "Recieved: " + text;
            /*string response = "Received some shit";
            _logger.Information(this.GetType(),"Recieved " + text);
            base.Send(response);
            _logger.Information(this.GetType(), response);*/
            Console.WriteLine("Recieved: " + text);
            base.Send("Hello");
            if (text.AtleastTwoLines()) {
                //has Content-Type and Content-body
                int endlineIndex = text.IndexOf('\n');
                string header = text.Substring(0, endlineIndex);
                if (header.Contains("Content-Type")) {
                    string headerValue = header.Split(':')[1].Trim();
                    if (headerValue.Equals("json", StringComparison.Ordinal)) {
                        //Actual JSON parsing
                        string body = text.Substring(endlineIndex + 1, text.Length - endlineIndex - 1);
                        //TODO FIX URL ENCODING
                        JObject jobj;
                        try {
                            jobj = JObject.Parse(text);
                            string methodName;
                            JProperty parameters;
                            foreach (JProperty prop in jobj.Children()) {
                                if (prop.Name.Equals("method",StringComparison.Ordinal)) {
                                    methodName = prop.Value.ToString();
                                }
                                else if (prop.Name.Equals("params", StringComparison.Ordinal)) {
                                    parameters = prop;
                                }
                                else {
                                    string namesy = prop.Name;
                                }
                            }
                        }
                        catch (Exception e) {
                            //ERROR
                            string error = e.Message;
                            base.Send(ComMessages.ErrorNonParsableJson);
                        }
                    }
                    else if (headerValue.Equals("keep-alive", StringComparison.Ordinal)) {
                        //keep-alive is improperly formatted, ignore it
                        base.Send(ComMessages.KeepAlive);
                    }
                    else {
                        //we don't know what we recieved, tell the client it's wrong
                        base.Send(ComMessages.UnknownContentType);
                    }
                }
                else {
                    //No Content-Type
                    base.Send(ComMessages.ErrorNoContentType);
                }
            }
            else {
                //Probably just a keep-alive
                string header;
                if (text.Contains<char>('\n')) {
                    header = text.Substring(0, text.IndexOf('\n'));
                }
                else {
                    header = text;
                }

                if (header.Contains("Content-Type")) {
                    string headerValue = header.Split(':')[1].Trim();
                    if (headerValue.Equals("keep-alive", StringComparison.Ordinal)) {
                        base.Send(ComMessages.KeepAlive);
                    }
                    else if (headerValue.Equals("json", StringComparison.Ordinal)) {
                        base.Send(ComMessages.ErrorIncorrectJson);
                    }
                    else {
                        base.Send(ComMessages.UnknownContentType);
                    }
                }
                else {
                    base.Send(ComMessages.ErrorNoContentType);
                }
            }

            /*int endlineIndex = text.IndexOf('\n');
            string header = text.Substring(0, endlineIndex);
            bool isJson = false;
            bool isError = false;
            if (header.Contains("Content-Type")) {
                string headerValue = header.Split(':')[1].Trim();
                if (headerValue.Equals("json",StringComparison.Ordinal)) {
                    isJson = true;
                }
                else if (headerValue.Equals("keep-alive", StringComparison.Ordinal)) {
                    isJson = false;
                }
                else {
                    //Unknown Content-Type, send error
                    //base.Send(ComMessages.UnknownHeaderValue);
                    isError = true;
                }
            }*/
            //string body = text.Substring(endlineIndex + 1, text.Length - endlineIndex - 1);
            if (true) {
                JObject jobj;
                try {
                    jobj = JObject.Parse(text);
                }
                catch {
                    //ERROR
                }
            }
            else {
                //Assume is keep-alive, return keep-alive
                //base.Send("'Content-Type': keep-alive");
            }

        }
    }
}
