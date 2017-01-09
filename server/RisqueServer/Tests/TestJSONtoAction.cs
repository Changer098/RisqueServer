using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using RisqueServer.Tickets;

namespace RisqueServer.Tests {
    //All Tests passing 1/9/2017
    [TestFixture]
    class TestJSONtoAction {
        [TestCase]
        public void basicParseTest() {
            string testJSON = @"{
                'portInfo':
                {
                    'actionType': 'Modify',
                    'picID': 'FREH-2B9-B',
                    'provider': 'freh-g60a-c3750ep-01:01-Gi1/0/42'
                },
                'settings':
                {
                    'currSpeed': '10/100T-SW-A',
                    'currVlans':
                    [
                        '128.210.100.000/24 Public Subnet (100)'
                    ],
                    'currVoiceVlan': '010.009.146.000/23-FREH-VoIP-G60A_Voice (2976)',
                    'newSpeed': '10/100/1000T-SW-A',
                    'newVlan': '010.162.006.000/24-FREH-CSDS-Supported_Computers_1 (1225)',
                    'newVoiceVlan': '010.009.146.000/23-FREH-VoIP-G60A_Voice (2976)'
                }
            }";
            Tickets.Action act = JsonConvert.DeserializeObject<Tickets.Action>(testJSON);       //check th
            Assert.AreEqual(act.portInfo.actionType, Tickets.ActionType.Modify);
            Assert.AreEqual(act.portInfo.picID, "FREH-2B9-B");
            Assert.AreEqual(act.portInfo.provider, "freh-g60a-c3750ep-01:01-Gi1/0/42");
            Assert.IsTrue(doEqual(act.settings.ParsedCurrSpeed, portSpeed.ParseString("10/100T-SW-A")));
            Assert.IsTrue(doEqual(act.settings.ParsedNewSpeed, portSpeed.ParseString("10/100/1000T-SW-A")));
        }
        [TestCase]
        public void parseNoCurrVlans() {
            string testJSON = @"{
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
                }";
            Tickets.Action act = JsonConvert.DeserializeObject<Tickets.Action>(testJSON);
            Assert.AreEqual(act.portInfo.actionType, Tickets.ActionType.Activate);
            Assert.AreEqual(act.portInfo.picID, "NLSN-B239A-A");
            Assert.AreEqual(act.portInfo.provider, "nlsn-b195e-c3750ep-01:01-Gi1/0/39");
            Assert.IsTrue(doEqual(act.settings.ParsedCurrSpeed, portSpeed.ParseString("")));
            Assert.IsTrue(doEqual(act.settings.ParsedNewSpeed, portSpeed.ParseString("10/100/1000T-SW-A")));
        }


                //For some reason, NUnit's AreEqual doesn't work properly with portSpeed Objects
        public bool doEqual(portSpeed old, portSpeed newer) {
            if (old.duplex != newer.duplex) {
                Console.WriteLine("Duplex wrong");
                return false;
            }
            if (old.midMod != newer.midMod) {
                Console.WriteLine("midMod wrong");
                return false;
            }
            //check tuples
            if (old.speed.Item1 != newer.speed.Item1) {
                Console.WriteLine("item1 wrong");
                return false;
            }
            if (old.speed.Item2 != newer.speed.Item2) {
                Console.WriteLine("item2 wrong");
                return false;
            }
            if (old.speed.Item3 != newer.speed.Item3) {
                Console.WriteLine("item3 wrong");
                return false;
            }
            if (old.speed.Item4 != newer.speed.Item4) {
                Console.WriteLine("item4 wrong");
                return false;
            }
            if (old.speedMod != newer.speedMod) {
                Console.WriteLine("speedMod wrong");
                Console.WriteLine("new speedMod: {0}, old speedMod: {1}", newer.speedMod, old.speedMod);
                return false;
            }
            return true;
        }
    }
    }
