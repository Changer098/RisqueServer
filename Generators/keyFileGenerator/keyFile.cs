using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators;

namespace keyFileGenerator {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("This Program generates a userFile for use with RisqueServer");
            Console.WriteLine("###########################################################");

            string keyDATA = null, ivDATA = null;
            string key64 = null, iv64 = null;
            string destpath = null;
            Console.Write("Do you want to supply your own IV and Key? For encrypting the keyFile. (y/n)");
            string response = Console.ReadLine();
            if (response.Length > 0 && response.Trim()[0] == 'y') {
                //Read key and IV
                Console.Write("Do you want to supply IV and Key as bytes? (y/n)");
                string byteResponse = Console.ReadLine();
                bool canContinue = false;
                if (byteResponse.Trim()[0] == 'y') {
                    Console.WriteLine("Key should be supplied as mod16 [0-255] integers seperated by spaces.");
                    do {
                        Console.WriteLine("Enter Key:");
                        string keyResponse = Console.ReadLine();
                        try {
                            byte[] bytes = keyResponse.ToByteArray();
                            if (bytes.Length % 16 == 0) {
                                canContinue = true;
                                key64 = Convert.ToBase64String(bytes);
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                    canContinue = false;
                    do {
                        Console.WriteLine("Enter IV:");
                        string keyResponse = Console.ReadLine();
                        try {
                            byte[] bytes = keyResponse.ToByteArray();
                            if (bytes.Length % 16 == 0) {
                                canContinue = true;
                                iv64 = Convert.ToBase64String(bytes);
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                }
                else {
                    Console.WriteLine("Key should be supplied as a base64 encoded value");
                    do {
                        Console.WriteLine("Enter Key:");
                        string rawKey = Console.ReadLine();
                        try {
                            byte[] array = Convert.FromBase64String(rawKey);
                            if (array.Length % 16 == 0) {
                                canContinue = true;
                                key64 = rawKey;
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                    canContinue = false;
                    Console.WriteLine("IV should be supplied as a base64 encoded value");
                    do {
                        Console.WriteLine("Enter IV:");
                        string rawIV = Console.ReadLine();
                        try {
                            byte[] array = Convert.FromBase64String(rawIV);
                            if (array.Length % 16 == 0) {
                                canContinue = true;
                                iv64 = rawIV;
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                }
            }
            else {
                //Generate Random values
                key64 = FileCrypto.randomBase64();
                iv64 = FileCrypto.randomBase64();
                Console.WriteLine("Generated Key: {0}", key64);
                Console.WriteLine("Generated IV: {0}", iv64);
            }
            Console.WriteLine();
            Console.Write("Do you want to supply your own IV and Key for the keyFile? (y/n)");
            response = Console.ReadLine();
            if (response.Length > 0 && response.Trim()[0] == 'y') {
                Console.Write("Do you want to supply IV and Key as bytes? (y/n)");
                string byteResponse = Console.ReadLine();
                bool canContinue = false;
                if (byteResponse.Trim()[0] == 'y') {
                    Console.WriteLine("Key should be supplied as mod16 [0-255] integers seperated by spaces.");
                    do {
                        Console.WriteLine("Enter Key:");
                        string keyResponse = Console.ReadLine();
                        try {
                            byte[] bytes = keyResponse.ToByteArray();
                            if (bytes.Length % 16 == 0) {
                                canContinue = true;
                                keyDATA = Convert.ToBase64String(bytes);
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                    canContinue = false;
                    do {
                        Console.WriteLine("Enter IV:");
                        string keyResponse = Console.ReadLine();
                        try {
                            byte[] bytes = keyResponse.ToByteArray();
                            if (bytes.Length % 16 == 0) {
                                canContinue = true;
                                ivDATA = Convert.ToBase64String(bytes);
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                }
                else {
                    Console.WriteLine("Key should be supplied as a base64 encoded value");
                    do {
                        Console.WriteLine("Enter Key:");
                        string rawKey = Console.ReadLine();
                        try {
                            byte[] array = Convert.FromBase64String(rawKey);
                            if (array.Length % 16 == 0) {
                                canContinue = true;
                                keyDATA = rawKey;
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                    Console.WriteLine("IV should be supplied as a base64 encoded value");
                    do {
                        Console.WriteLine("Enter IV:");
                        string rawIV = Console.ReadLine();
                        try {
                            byte[] array = Convert.FromBase64String(rawIV);
                            if (array.Length % 16 == 0) {
                                canContinue = true;
                                ivDATA = rawIV;
                            }
                            else {
                                Console.WriteLine("Key's length is not a divisor of 16. Try again");
                            }
                        }
                        catch {
                            Console.WriteLine("Could not parse");
                        }
                    }
                    while (!canContinue);
                }
            }
            else {
                keyDATA = FileCrypto.randomBase64();
                ivDATA = FileCrypto.randomBase64();
                Console.WriteLine("Generated Key: {0}", keyDATA);
                Console.WriteLine("Generated IV: {0}", ivDATA);
            }
            destpath = System.Environment.CurrentDirectory + '\\' + "keyFile.dat";
            FileCrypto.encryptKeyFile(destpath, key64, iv64, formKeyFile(keyDATA, ivDATA));
            Console.WriteLine();
            Console.WriteLine("Inputted Values");
            Console.WriteLine("Key to encrypt: {0}", key64);
            Console.WriteLine("IV to encrypt: {0}", iv64);
            Console.WriteLine("Key: {0}", keyDATA);
            Console.WriteLine("IV: {0}", ivDATA);
            Console.WriteLine("Successfully created keyFile at {0}", destpath);
            //Console.ReadLine();
        }

        public static string formKeyFile(string key, string iv) {
            return String.Format("######BEGIN KEY FILE######"
                + '\n' + "key={0}"
                + '\n' + "iv={1}"
                + '\n' + "######END KEY FILE######", key, iv);
        }
    }
}
