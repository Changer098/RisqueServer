using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RisqueServer.Methods {
    interface IRPCMethod {
        /// <summary>
        /// Executes the Method
        /// </summary>
        /// <param name="args">Method specific arguments, methods are responsible for parsing arguments</param>
        /// <returns>Results of the Method as defined by MethodSummary.txt</returns>
        /// <see cref="MethodSummary.txt"/>
        JObject run(JObject args);

        /// <summary>
        /// Whether a method uses a keep alive in order to run asynchronously 
        /// </summary>
        bool usesKeepAlive();
    }
}
