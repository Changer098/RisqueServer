using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisqueServer.Communication {
    class ComMessages {
        public static readonly string UnknownContentType = @"Content-Type: json
        {
	        'ErrorMessage' : 'Transmission included an unknown Content-Type'
        }";
        public static readonly string ErrorNoContentType = @"Content-Type: json
        {
	        'ErrorMessage' : 'Message does not contain a Content-Type'
        }";
        public static readonly string KeepAlive = @"Content-Type: keep-alive";
        public static readonly string ErrorIncorrectJson = @"Content-Type: json
        {
	        'ErrorMessage' : 'Message does not contain properly formatted json'
        }";
        public static readonly string ErrorNonParsableJson = @"Content-Type: json
        {
	        'ErrorMessage' : 'JSON could not be parsed'
        }";
    }
}
