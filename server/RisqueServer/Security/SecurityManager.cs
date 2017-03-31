using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace RisqueServer.Security {
    public class SecurityManager {

        private string key = null;
        private string iv = null;
        private AesManaged aes = null;
        private UTF8Encoding encoder = new UTF8Encoding();
        private UserManager userManager = null;

        /// <summary>
        /// Initializes the Security Manager by attempting to decrypt the keyFile
        /// </summary>
        /// <param name="keyFileLocation">Location of the keyFile</param>
        /// <param name="key">supplied encryption key (Base64) given at program start</param>
        /// <param name="iv">supplied encryption iv (Base64) given at program start</param>
        /// <exception cref="DecryptionException">Throws if @keyFileLocation cannot be decrypted</exception>
        /// <exception cref="IOException">Throws if @keyFileLocation cannot be read</exception>
        public SecurityManager(string keyFileLocation, string key, string iv, string userFileLocation) {
            if (key == null || iv == null || keyFileLocation == null || userFileLocation == null) {
                throw new ArgumentNullException();
            }

            if (!File.Exists(keyFileLocation)) {
                throw new FileNotFoundException("keyFile not found");
            }
            if (!File.Exists(userFileLocation)) {
                throw new FileNotFoundException("userFile not found");
            }
            if (!isValidLength(key) || !isValidLength(iv)) {
                throw new ArgumentException("Either Key or IV is an improper length");
            }
            string keyFileENC = keyFileENC = File.ReadAllText(keyFileLocation);             //Encrypted base64
            string[] keyLines = null;
            var aes = new AesManaged();
            try {
                //Try to decrypt keyFile
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                aes.Padding = PaddingMode.Zeros;
                aes.Mode = CipherMode.CBC;
                var decryptor = aes.CreateDecryptor();
                var cipher = Convert.FromBase64String(keyFileENC);
                string plaintext = encoder.GetString(decryptor.TransformFinalBlock(cipher, 0, cipher.Length));
                keyLines = plaintext.Split('\n');
                plaintext = null;
            }
            catch {
                throw new DecryptionException();
            }
            if (keyLines != null) {
                if (isValidKeyFile(keyLines)) {
                    for (int i = 0; i < keyLines.Length; i++) {
                        if (i == 0 || i == keyLines.Length - 1) {
                            continue;
                        }
                        string[] keyValuePair = keyLines[i].Split('=');
                        if (keyValuePair[0] == "iv") {
                            string fixIv = string.Empty;
                            for (int x = 1; x < keyValuePair.Length; x++) {
                                if (keyValuePair[x] == "") {
                                    fixIv = fixIv + '=';
                                }
                                else {
                                    fixIv = fixIv + keyValuePair[x];
                                }
                            }
                            this.iv = fixIv;
                        }
                        if (keyValuePair[0] == "key") {
                            string fixKey = String.Empty;
                            for (int x = 1; x < keyValuePair.Length; x++) {
                                if (keyValuePair[x] == "") {
                                    fixKey = fixKey + '=';
                                }
                                else {
                                    fixKey = fixKey + keyValuePair[1];
                                }
                            }
                            this.key = fixKey;
                        }
                    }
                }
                else {
                    throw new Exception("Either Decrypted an invalid keyfile or supplied key/iv is wrong");
                }
                keyLines = null;                    //Destroy
                this.aes = aes;                     //store AES Object
                this.aes.IV = Convert.FromBase64String(this.iv);
                this.aes.Key = Convert.FromBase64String(this.key);

                //Create UserManager with supplied userFileLocation and decrypted keyFile
                string[] userLines;
                try {
                    string userFileENC = File.ReadAllText(userFileLocation);
                    var decryptor = aes.CreateDecryptor();
                    var cipher = Convert.FromBase64String(userFileENC);
                    string plaintext = encoder.GetString(decryptor.TransformFinalBlock(cipher, 0, cipher.Length));
                    userLines = plaintext.Split('\n');
                    plaintext = null;
                    userFileENC = null;
                }
                catch {
                    throw new DecryptionException();
                }
                if (isValidUserFile(userLines)) {
                    string name = null, email = null, pass = null;
                    foreach (string line in userLines) {
                        string[] split = line.Split('=');
                        if (split[0].Trim() == "username") {
                            name = split[1].Trim();
                        }
                        else if (split[0].Trim() == "email") {
                            email = split[1].Trim();
                        }
                        else if (split[0].Trim() == "pass") {
                            pass = split[1].Trim();
                        }
                    }
                    int securityValue = 0;
                    User defaultUser = new User(name, pass, email, out securityValue);
                    this.userManager = new UserManager(defaultUser, securityValue, this);

                    //Try and destroy any remaining data
                    name = null;
                    email = null;
                    pass = null;
                    securityValue = int.MinValue;
                    keyLines = null;
                    keyFileENC = null;
                }
                else {
                    throw new Exception("Either Decrypted an invalid userFile or keyFile contains incorrect key/iv");
                }
            }
            else {
                throw new DecryptionException();
            }
        }
        public string getUserFileLocation(string userName, string key, string iv) {
            //create a new UserFile encrypted with the specified key and iv
            int secValue = -1;
            string userFileContents = null;
            string fileDest = null;
            if (userName == "default") {
                //string userFileContents = userManager.
                User defUser = userManager.getDefault(this, out secValue);
                if (secValue != -1 && defUser != null) {
                    userFileContents = defUser.createUserFileContents(secValue);
                    string encContents = encrypt(userFileContents, key, iv);
                    do {
                        string fileName = Guid.NewGuid().ToString() + ".dat";
                        if (Extensions.IsLinux) {
                            fileDest = Tickets.TicketStorage.folderRoot + "tmp/" + fileName;
                        }
                        else {
                            fileDest = Tickets.TicketStorage.folderRoot + "tmp\\" + fileName;
                        }
                    }
                    while (File.Exists(fileDest));
                    //write encContents to fileDest
                    File.WriteAllText(fileDest, encContents);
                    return fileDest;
                }
                else {
                    throw new Exception("Could not get User");
                }
            }
            else {
                if (userManager.containsUser(userName)) {
                    throw new NotImplementedException();
                }
                else {
                    throw new Exception("User does not exist");
                }
            }
        }
        public void destroyUserFile(string userFileLoc) {
            int fileLen = File.ReadAllText(userFileLoc).Length;
            byte[] zeroes = new byte[fileLen];
            for (int i = 0; i < fileLen; i++) {
                zeroes[i] = 0;
            }
            File.WriteAllBytes(userFileLoc, zeroes);
            File.Delete(userFileLoc);
        }

        /// <summary>
        /// Determines if string is a Valid AES length
        /// </summary>
        /// <param name="s">Base64 string to check length of</param>
        /// <returns>True if is valid length, else false</returns>
        public static bool isValidLength(string s) {
            //http://stackoverflow.com/questions/13378815/base64-length-calculation
            int origLen = ((s.Length * 3) / 4) - s.CountChar('=');
            if (origLen % 16 == 0 && origLen != 0) {
                return true;
            }
            else {
                return false;
            }
        }
        //Determine a keyFile is Valid or not
        private bool isValidKeyFile(string[] data) {
            if (data[0].Trim() != "######BEGIN KEY FILE######") {
                return false;
            }
            if (data[data.Length - 1].Trim().Substring(0, 24) != "######END KEY FILE######") {
                return false;
            }
            return true;
        }
        //Determine if a userFile is Valid or not
        private bool isValidUserFile(string[] data) {
            if (data[0].Trim() != "######BEGIN USER FILE######") {
                return false;
            }
            if (data[data.Length - 1].Trim().Substring(0, 25) != "######END USER FILE######") {
                return false;
            }
            return true;
        }
        //Generate a random number
        public static int randomSecurityValue() {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[16];
            byte[] sec = new byte[16];
            int sum = 0;
            rng.GetBytes(bytes);
            rng.GetBytes(sec);
            for (int i = 0; i < bytes.Length; i++) {
                sum = sum + (bytes[i] * sec[i]);
            }
            return sum;
        }
        //Generate a random cryptoValue
        public static string randomCrypto() {
            RandomNumberGenerator generator = RandomNumberGenerator.Create();
            byte[] bytes = new byte[16];
            for (int i = 0; i < 128; i++) {
                generator.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
        //Data is non-base64 encoded
        //Key and IV is base64 encoded
        //Returns a Base64 encoded string
        private string encrypt(string data, string key, string iv) {
            byte[] plaintext = encoder.GetBytes(data);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] ivBytes = Convert.FromBase64String(iv);
            var aesManaged = new AesManaged();
            aesManaged.IV = ivBytes;
            aesManaged.Key = keyBytes;
            aesManaged.Mode = CipherMode.CBC;
            aesManaged.Padding = PaddingMode.Zeros;
            var encryptor = aesManaged.CreateEncryptor();
            var text_bytes = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
            return Convert.ToBase64String(text_bytes);
        }
        //Everything is base64 encoded
        //Returns a UTF8 encoded string
        private string decrypt(string data, string key, string iv) {
            byte[] enctext = Convert.FromBase64String(data);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] ivBytes = Convert.FromBase64String(iv);

            var aesManaged = new AesManaged();
            aesManaged.Key = keyBytes;
            aesManaged.IV = ivBytes;
            aesManaged.Mode = CipherMode.CBC;
            aesManaged.Padding = PaddingMode.None;

            var decryptor = aesManaged.CreateDecryptor();
            var plaintext = decryptor.TransformFinalBlock(enctext, 0, enctext.Length);
            return encoder.GetString(plaintext, 0, plaintext.Length);
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
