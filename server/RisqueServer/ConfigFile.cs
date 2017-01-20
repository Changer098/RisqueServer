﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer {
    class ParsedConfigFile {
        public int port { get; set; }
        public int portSecure { get; set; }
        public bool verbose { get; set; }
        public string ticketDirectory { get; set; }
    }
    class ActiveConfig {
        public int port { get; set; }
        public int portSecure { get; set; }
        public bool verbose { get; set; }
        public string ticketDirectory { get; set; }
        public bool hasConfig;
    }
}