using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace RisqueServer.Tests {
    //All Tests passing 1/9/2017
    [TestFixture]
    class TestHeaderSeperation {
        string testText = @"Content-Type: json
        {
	        'method' : 'doesTicketExist',
	        'params' : {
		        'id' : 59133
	        }
        }";
        
        [TestCase]
        public void getHeader() {
            int endlineIndex = testText.IndexOf('\n');
            string header = testText.Substring(0, endlineIndex);
            try {
                Assert.IsTrue(header.Contains("Content-Type"));
            }
            catch {
                Console.WriteLine("Header: " + header);
                throw new AssertionException("The header does not contain a Content-Type");
            }
            string headerValue = header.Split(':')[1].Trim();
            try {
                Assert.IsTrue(headerValue.Equals("json", StringComparison.Ordinal));
            }
            catch {
                Console.WriteLine("HeaderValue:" + headerValue);
                throw new AssertionException("The header does not contain the json keyword");
            }
            /*if (header.Contains("Content-Type")) {
                string headerValue = header.Split(':')[1];
                if (headerValue.Equals("json", StringComparison.Ordinal)) {

                }
                else if (headerValue.Equals("keep-alive", StringComparison.Ordinal)) {

                }
                else {
                    //Unknown Content-Type, send error
                }
            }*/

        }
        [TestCase]
        public void getBody() {
            int endlineIndex = testText.IndexOf('\n');
            string body = testText.Substring(endlineIndex + 1, testText.Length - endlineIndex - 1);
            /*try {
                //Assert.IsTrue(body[0] != '\n', "body has a newline beginning");
                //Assert.IsTrue(body[0] == '{' || body[0] == '\t');
                Assert.IsTrue(body.First<char>() == '{');
            }
            catch {
                Console.WriteLine("First character is: " + (int)'{');
                throw new AssertionException("body does not have a { beginning");
            }*/
            Assert.DoesNotThrow(() => Newtonsoft.Json.Linq.JObject.Parse(body), "Could not parse");
        }
    }
}
