﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    interface IRPCMethod {
        JProperty[] run(JProperty[] args);
    }
}
