using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace RisqueServer.Tickets {
    class Action {
        /*"Type" : enum,
				"picID" : String,
				"provider" : string
				"speed" : speedType,
				"vlan" : int,
				"voiceVlan" : int,
				"Services" : [
					String,
				]
                */
        public string picID { get; set; }
        public string provider { get; set; }
        [JsonProperty("speed")]
        private string raw_speed {
            set {

            } }
        public Speed speed;
        [JsonConverter(typeof(ActionType))]
        public ActionType type { get; set; }
    }
    enum ActionType {

    }
    class Speed {
        /*10/100/1000T-SW-A
        10/100T-SW-A
        1000T-SW-F
        100T-SW-A
        100T-SW-F
        100T-SW-H
        10T-SW-A
        10T-SW-F
        10T-SW-H*/
        static Speed ParseString(string rawString) {
            return null;
        }
    }
}
