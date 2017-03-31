using System;
using System.IO;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators;

/// <summary>
/// Generates a userFile.dat
/// </summary>
namespace userFileGenerator {
    class userFile {
        static void Main(string[] args) {

            bool keyFileExists = false;
            string keyFilePath = null;

            Console.WriteLine("This Program generates a userFile for use with RisqueServer");
            Console.WriteLine("###########################################################");

            do {
                Console.WriteLine("Key file? ");
                string keyFile = Console.ReadLine();

                //remove quotations
                int quoteIndex = keyFile.IndexOf((char)34);
                while (quoteIndex >= 0) {
                    keyFile = keyFile.Remove(quoteIndex, 1);
                    quoteIndex = keyFile.IndexOf((char)34);
                }
                //check if file exists
                try {
                    if (File.Exists(keyFile)) {
                        keyFileExists = true;
                    }
                    else {
                        Console.WriteLine("File does not exist");
                        keyFileExists = false;
                    }
                }
                catch {
                    Console.WriteLine("Failed trying to locate file");
                }
                if (keyFileExists) {
                    keyFilePath = keyFile;
                    break;
                }
            }
            while (!keyFileExists);
            Console.Write("Key File Password: ");
            string keyFilePass = ConsoleExtend.ReadPassword();
            Console.Write("Key File IV: ");
            string keyFileIV = Console.ReadLine();
            Console.WriteLine("Are Password/IV Base64? (y/n): ");
            string base64Response = Console.ReadLine();
            if (base64Response.Trim()[0] != 'y') {
                keyFilePass = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyFilePass));
                keyFileIV = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyFileIV));
            }
            //Try and decrypt the keyFile

            string[] keyFileText = FileCrypto.decryptFile(keyFilePath, keyFilePass, keyFileIV);
            if (!FileCrypto.isValidFormat(keyFileText)) {
                Console.WriteLine("Key File is not in proper format or could not be decrypted");
                Console.WriteLine("Click enter to quit");
                Console.ReadLine();
                return;
            }
            string KEY = null, IV = null;
            for (int i = 0; i < keyFileText.Length; i++) {
                if (i == 0 || i == (keyFileText.Length - 1))
                    continue;
                if (keyFileText[i].Contains("iv=")) {
                    string[] split = keyFileText[i].Split('=');
                    string fixIV = string.Empty;
                    for (int x = 1; x < split.Length; x++) {
                        if (split[x] == "") {
                            fixIV = fixIV + '=';
                        }
                        else {
                            fixIV = fixIV + split[x];
                        }
                    }
                    IV = fixIV;
                    continue;
                }
                else if (keyFileText[i].Contains("key=")) {
                    string[] split = keyFileText[i].Split('=');
                    string fixKey = string.Empty;
                    for (int x = 1; x < split.Length; x++) {
                        if (split[x] == "") {
                            fixKey = fixKey + '=';
                        }
                        else {
                            fixKey = fixKey + split[x];
                        }
                    }
                    KEY = fixKey;
                    continue;
                }
            }
            Console.WriteLine("Key: {0}", KEY);
            Console.WriteLine("IV: {0}", IV);

            Console.WriteLine("Creating a new userFile");
            Console.Write("Username: ");
            string userName = Console.ReadLine();
            Console.Write("Email (userName@purdue.edu): ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string pass = ConsoleExtend.ReadPassword();

            string destPath = System.Environment.CurrentDirectory + '\\' + userName + ".dat";

            FileStream newKeyFile = File.Create(destPath);
            byte[] keyFileBytes = System.Text.Encoding.UTF8.GetBytes(FileCrypto.encryptData(userName, email, pass, KEY, IV));
            newKeyFile.Write(keyFileBytes, 0, keyFileBytes.Length);
            newKeyFile.Flush();
            Console.WriteLine("Successfully generated KeyFile at location: " + destPath);
            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
        }
    }
}
