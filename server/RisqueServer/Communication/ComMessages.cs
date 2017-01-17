﻿using System;
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
        public static readonly string ErrorNotValidMethod = @"Content-Type: json
        {
	        'ErrorMessage' : 'Method call does not contain a valid method'
        }";
        public static readonly string ErrorAsyncMethodFailedToStart = @"Content-Type: json
        {
	        'ErrorMessage' : 'Asynchronous method failed to start'
        }";
        public static readonly string ErrorMethodFailed = @"Content-Type: json
        {
	        'ErrorMessage' : 'Method failed to execute'
        }";
        public static readonly string MethodErrorIsAsync = @"'ErrorMessage' : 'Cannot run Async Method synchronously'";
        public static readonly string MethodErrorGeneric = @"'ErrorMessage' : 'Method failed to execute'";
        public static readonly string MethodErrorInvalidArguments = @"'ErrorMessage' : 'Method contained invalid arguments'";
        public static readonly string ContentTypeJson = @"Content-Type:json" + '\n';
    }
}
