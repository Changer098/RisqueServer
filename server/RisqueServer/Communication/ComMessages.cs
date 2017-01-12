using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Communication {
    class ComMessages {
        public static readonly string UnknownHeaderValue = @"Content-Type: json
        {
	        'ErrorMessage' : 'Transmission included an unknown Header Value'
        }";
    }
}
