using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RisqueServer.Tickets {
    class Action {
        /*public string picID { get; set; }
        public string provider { get; set; }
        [JsonProperty("speed")]
        public string raw_speed {
            set {
                speed = portSpeed.ParseString(value);
            } }
        public portSpeed speed;
        [JsonConverter(typeof(ActionType))]
        public ActionType Type { get; set; }
        public int vlan { get; set; }
        public int voiceVlan { get; set; }
        public string[] Services { get; set; }*/

        public PortInfo portInfo { get; set; }
        public Settings settings { get; set; }
    }

    public class PortInfo {
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType actionType { get; set; }
        public string picID { get; set; }
        public string provider { get; set; }
    }

    public class Settings {
        public string currSpeed { get; set; }
        public portSpeed ParsedCurrSpeed { get; set; }
        public List<string> currVlans { get; set; }
        public string currVoiceVlan { get; set; }
        public portSpeed ParsedNewSpeed { get; set; }
        public string newSpeed { get; set; }
        public string newVlan { get; set; }
        public string newVoiceVlan { get; set; }
        public string serializeMessage;
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) {
            //parses currSpeed and newSpeed
            this.ParsedCurrSpeed = portSpeed.ParseString(currSpeed);
            this.ParsedNewSpeed = portSpeed.ParseString(newSpeed);
        }
    }

    public enum ActionType {
        Activate,
        Modify,
        Repair,             //Not Supported, identification purposes
        Unknown             //Needed for parsing an empty speed 
    }
    public enum Duplex {
        Auto,
        Full,
        Half,
        Unknown
    }

    public class portSpeed {
        
        public Tuple<int, int, int, int> speed { get; set; }           //10/100/100T = 10,100,1000,0 || 100T = 0,100,0,0 (In Megabits)
        public Duplex duplex { get; set; }
        //TODO: FIX NAME
        public char speedMod { get; set; }                             //Usually the 'T' in 10/100T
        //TODO: FIX NAME
        public string midMod { get; set; }                             //The SW in 10/100T-SW-A

        public portSpeed(Duplex duplex,
            Tuple<int, int, int, int> speed,
            char speedMod,
            string midMod) {
            this.duplex = duplex;
            this.speed = speed;
            this.speedMod = speedMod;
            this.midMod = midMod;
        }

        /*10/100/1000T-SW-A
        10/100T-SW-A
        1000T-SW-F
        100T-SW-A
        100T-SW-F
        100T-SW-H
        10T-SW-A
        10T-SW-F
        10T-SW-H*/
        public static portSpeed ParseString(string rawString) {
            if (rawString == null || rawString == "") {
                return new portSpeed(Duplex.Unknown, new Tuple<int, int, int, int>(0, 0, 0, 0), (char)0, "");
            }
            string[] split = rawString.Split('-');
            Tuple<int, int, int, int> speed = null;
            string midMod = string.Empty;
            char speedMod = (char)0;
            Duplex duplex = Duplex.Auto;
            if (split.Length != 3) {
                throw new Exception("portSpeed.ParseString failed with an improperly formatted string");
            }
            if (split[2].Length != 1) {
                throw new Exception("portSpeed.ParseString failed with an improperly formatted duplex string");
            }
            //remove the speedMod character for further calculations
            speedMod = split[0][split[0].Length - 1];
            split[0] = split[0].Trim(speedMod);

            if (split[0].Contains('/')) {
                //Many different speeds, like 10/100T-SW-A
                string[] splitStrike = split[0].Split('/');
                if (splitStrike.Length < 2) {
                    //There was no '/' in the string, which is weird...
                    throw new Exception("portSpeed.ParseString failed to split '/' when the string contained a '/'");
                }

                //Check if valid string - 100/10 is not valid, nor is 100/1000/10
                //Convert to numbers for comparison testing, then compare
                int[] numbers = new int[splitStrike.Length];
                for (int i = 0; i < splitStrike.Length; i++) {
                    try {
                        numbers[i] = int.Parse(splitStrike[i]);
                    }
                    catch {
                        throw new Exception("portSpeed.ParseString string is not a valid number: " + splitStrike[i]);
                    }
                }
                //Frankly this is pretty ugly, but should work
                for (int i = 0; i < splitStrike.Length - 1; i++) {
                    /*if ((numbers[i] > numbers[i+1]) && (numbers[i] > 0) && (numbers[i] % 10 == 0) && (numbers[i] % 5 == 0) && (numbers[i] % 2 == 0)) {
                        throw new Exception("portSpeed.ParseString string is invalid format");
                    }*/
                    if ((numbers[i] % 10 != 0) || (numbers[i] % 5 != 0) || (numbers[i] % 2 != 0)) {
                        throw new Exception("portSpeed.ParseString number is not valid");
                    }
                    if (numbers[i] > numbers[i+1]) {
                        throw new Exception("portSpeed.ParseString number is not valid");
                    }
                    if ((numbers[i].ToString()[0] != '1') && (numbers[i].ToString().Length <= 4)) {
                        throw new Exception("porSpeed.ParseString string is in invalid format");
                    }
                }
                if (numbers.Length > 4) {
                    throw new Exception("portSpeed.ParseString speeds with more than four possible options are not supported");
                }
                //numbers are valid, assign to tuple
                int[] tmpItems = { 0, 0, 0, 0 };    //assign numbers to item, then assign items to tuple
                for (int i = 0; i < 4; i++) {
                    if (i < numbers.Length) {
                        tmpItems[i] = numbers[i];
                    }
                }
                speed = new Tuple<int, int, int, int>(tmpItems[0], tmpItems[1], tmpItems[2], tmpItems[3]);
                
            }
            else {
                //One speed, like 1000T-SW-A
                int tmpNumber = -1;
                try {
                    tmpNumber = int.Parse(split[0]);
                }
                catch {
                    throw new Exception("portSpeed.ParseString string is not a number");
                }
                if ((tmpNumber % 10 == 0) && (tmpNumber % 5 == 0) && (tmpNumber % 2 == 0) && (tmpNumber > 0)) {
                    if (tmpNumber < 100) {
                        speed = new Tuple<int, int, int, int>(tmpNumber, 0, 0, 0);
                    }
                    else if (tmpNumber < 1000) {
                        speed = new Tuple<int, int, int, int>(0, tmpNumber, 0, 0);
                    }
                    else if (tmpNumber < 10000) {
                        speed = new Tuple<int, int, int, int>(0, 0, tmpNumber, 0);
                    }
                    else {
                        speed = new Tuple<int, int, int, int>(0, 0, 0, tmpNumber);
                    }
                }
            }
            midMod = split[1];
            char duplexChar = split[2][0];
            switch (duplexChar) {
                case 'A':
                    duplex = Duplex.Auto;
                    break;
                case 'F':
                    duplex = Duplex.Full;
                    break;
                case 'H':
                    duplex = Duplex.Half;
                    break;
            }
            return new portSpeed(duplex, speed, speedMod, midMod);
        }
        //Create a string of the object
        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            bool lastSet = false;
            //Convert Tuple to string
            if (speed.Item1 != 0) {
                builder.Append(speed.Item1);
                lastSet = true;
            }
            if (speed.Item2 != 0) {
                if (lastSet) builder.Append('/');
                builder.Append(speed.Item2);
                lastSet = true;
            }
            if (speed.Item3 != 0) {
                if (lastSet) builder.Append('/');
                builder.Append(speed.Item3);
                lastSet = true;
            }
            if (speed.Item4 != 0) {
                if (lastSet) builder.Append('/');
                builder.Append(speed.Item4);
                lastSet = true;
            }
            builder.Append('-');
            builder.Append(midMod + '-');
            switch (duplex) {
                case Duplex.Auto:
                    builder.Append('A');
                    break;
                case Duplex.Full:
                    builder.Append('F');
                    break;
                case Duplex.Half:
                    builder.Append('H');
                    break;
            }
            return builder.ToString();
        }
    }
}
