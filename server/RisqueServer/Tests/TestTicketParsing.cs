using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;

namespace RisqueServer.Tests {
    [TestFixture]
    class TestTicketParsing {
        [TestCase]
        public void doesParse() {
            string testJSON = @"{
                'ticketID': '59110',
                'dueBy': '1/11/2017 4:01 PM',
                'Actions':
                [
                    {
                        'portInfo':
                        {
                            'actionType': 'Activate',
                            'picID': 'NLSN-B239A-A',
                            'provider': 'nlsn-b195e-c3750ep-01:01-Gi1/0/39'
                        },
                        'settings':
                        {
                            'currVlans': [],
                            'newSpeed': '10/100/1000T-SW-A',
                            'newVlan': '010.163.019.000/24-NLSN-AGIT-FoodSci_Desktop_Users (1201)',
                            'newVoiceVlan': '010.011.048.000/23-NLSN-VoIP-B195E_Voice (2975)'
                        }
                    }
                ]
            }";
            Assert.DoesNotThrow(() => JsonConvert.DeserializeObject<Tickets.Ticket>(testJSON), "Deserializing Ticket failed", null);
            //ckets.Ticket ticket = JsonConvert.DeserializeObject<Tickets.Ticket>(testJSON);
        }
        [TestCase]
        public void TicketInfo() {
            string testJSON = @"{
                'ticketID': '59110',
                'dueBy': '1/11/2017 4:01 PM',
                'Actions':
                [
                    {
                        'portInfo':
                        {
                            'actionType': 'Activate',
                            'picID': 'NLSN-B239A-A',
                            'provider': 'nlsn-b195e-c3750ep-01:01-Gi1/0/39'
                        },
                        'settings':
                        {
                            'currVlans': [],
                            'newSpeed': '10/100/1000T-SW-A',
                            'newVlan': '010.163.019.000/24-NLSN-AGIT-FoodSci_Desktop_Users (1201)',
                            'newVoiceVlan': '010.011.048.000/23-NLSN-VoIP-B195E_Voice (2975)'
                        }
                    }
                ]
            }";
            Tickets.Ticket ticket = JsonConvert.DeserializeObject<Tickets.Ticket>(testJSON);
            Assert.AreEqual("1/11/2017 4:01 PM", ticket.dueBy);
            Assert.AreEqual(59110, ticket.ticketID);
        }
    }
}
