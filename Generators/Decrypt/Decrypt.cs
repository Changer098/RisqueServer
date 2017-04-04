using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators;

namespace Decrypt {
    class Decrypt {
        static void Main(string[] args) {
            bool canContinue = true;
            do {
                printOptions();
                string response = Console.ReadLine();
                int resultCode = 0;
                bool hasResponse = false;
                try {
                    resultCode = int.Parse(response);
                    hasResponse = true;
                }
                catch {
                    hasResponse = false;
                }
                if (hasResponse) {
                    switch (resultCode) {
                        case -1:
                            return;
                        case 1:
                            DecryptString();
                            break;
                        case 2:
                            DecryptFile();
                            break;
                        default:
                            Console.WriteLine("Invalid Option.");
                            break;
                    }
                }
                else {
                    Console.WriteLine("Could not parse request. Try again with a number.");
                }
            }
            while (canContinue);
        }
        static void printOptions() {
            Console.WriteLine("-1 \t Quit the Application");
            Console.WriteLine("1 \t Decrypt String");
            Console.WriteLine("2 \t Decrypt File");
            Console.Write("> ");
        }
        static void DecryptString() {
            byte[] key, iv;
            getEnryptData(out key, out iv);
            Console.WriteLine("String to Decrypt? (In base64)");
            string enctext = Console.ReadLine();

            string plaintext = FileCrypto.decryptString(enctext, Convert.ToBase64String(key), Convert.ToBase64String(iv));
            Console.WriteLine("Plaintext: ");
            Console.WriteLine(plaintext);
        }
        static void DecryptFile() {
            byte[] key, iv;
            getEnryptData(out key, out iv);
            Console.WriteLine("File to Decrypt? (In base64)");
            string filePath = Console.ReadLine();
            
            //get rid of quotations
            int quoteIndex = filePath.IndexOf((char)34);
            while (quoteIndex >= 0) {
                filePath = filePath.Remove(quoteIndex, 1);
                quoteIndex = filePath.IndexOf((char)34);
            }
            string[] plaintext = FileCrypto.decryptFile(filePath, Convert.ToBase64String(key), Convert.ToBase64String(iv));
            Console.WriteLine("Plaintext: ");
            foreach(string plain in plaintext) {
                Console.WriteLine(plain);
            }
        }
        static void getEnryptData(out byte[] key, out byte[] iv) {
            key = null;
            iv = null;
            bool isValidKey = false, isValidIv = false;
            int keyLength = 0;
            do {
                Console.WriteLine("Enter your Base64 Key");
                string enteredKey = Console.ReadLine();
                try {
                    byte[] arr = Convert.FromBase64String(enteredKey);
                    if (arr.Length % 16 == 0 && arr.Length != 0) {
                        isValidKey = true;
                        key = arr;
                        keyLength = arr.Length;
                    }
                    else {
                        isValidKey = false;
                        Console.WriteLine("Key is of invalid length");
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            } while (!isValidKey);

            do {
                Console.WriteLine("Enter your Base64 IV");
                string enteredIv = Console.ReadLine();
                try {
                    byte[] ivArr = Convert.FromBase64String(enteredIv);
                    if (ivArr.Length % 16 == 0 && ivArr.Length != 0 && ivArr.Length == keyLength) {
                        iv = ivArr;
                        isValidIv = true;
                    }
                    else {
                        isValidIv = false;
                        Console.WriteLine("IV is of invalid length");
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            } while (!isValidIv);
            
        }
    }
}
