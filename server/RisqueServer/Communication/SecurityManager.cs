using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RisqueServer.Communication {
    class SecurityManager {

        private string key = null;
        private string iv = null;

        /// <summary>
        /// Initializes the Security Manager by attempting to decrypt the keyFile
        /// </summary>
        /// <param name="keyFileLocation">Location of the keyFile</param>
        /// <param name="key">supplied encryption key given at program start</param>
        /// <param name="iv">supplied encryption iv given at program start</param>
        /// <exception cref="DecryptionException">Throws if @keyFileLocation cannot be decrypted</exception>
        public SecurityManager(string keyFileLocation, string key, string iv) {
            if (!File.Exists(keyFileLocation)) {
                throw new FileNotFoundException("keyFile not found");
            }
        }
    }

    /// <summary>
    /// Failed to decrypt file, FATAL EXCEPTION
    /// </summary>
    public class DecryptionException : Exception {
        public DecryptionException() : base("Failed to Decrypt File") { }
    }

    /// <summary>
    /// Failed to encrypt file, FATAL EXCEPTION
    /// </summary>
    public class EncryptionException : Exception {
        public EncryptionException() : base("Failed to Encrypt File") { }
    }
}
