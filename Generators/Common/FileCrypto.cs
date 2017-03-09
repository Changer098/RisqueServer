﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Generators {
    public class FileCrypto {
        public static string[] decryptFile(string path, string key, string iv) {
            string ciphtext = File.ReadAllText(path);                                                   //Read from file
            string plaintext = decryptString(ciphtext, key, iv);                                        //Decrypt string
            string[] lines = plaintext.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Trim();
            }
            if (lines[lines.Length - 1].Length > 24) {
                lines[lines.Length - 1] = lines[lines.Length - 1].Substring(0, 24);
            }
            return lines;
        }
        public static bool isValidFormat(string[] data) {
            if (data[0] != "######BEGIN KEY FILE######") {
                return false;
            }
            if (data[data.Length - 1] != "######END KEY FILE######") {
                return false;
            }
            return true;
        }
        public static string decryptString(string ciphtext, string key, string iv) {
            var textEncoder = new UTF8Encoding();

            var aes = new AesManaged();
            aes.Key = textEncoder.GetBytes(key);
            aes.IV = textEncoder.GetBytes(iv);
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            var decryptor = aes.CreateDecryptor();
            var cipher = Convert.FromBase64String(ciphtext);
            var text_bytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            var text = textEncoder.GetString(text_bytes);
            //Console.WriteLine(text);
            return text;
        }
        public static string encryptData(string username, string email, string pass, string key, string iv) {
            string plaintext = String.Format("username={0}", username) + '\n' 
                + String.Format("email={0}", email) + '\n' 
                + String.Format("pass={0}", pass);

            var textEncoder = new UTF8Encoding();

            var aes = new AesManaged();
            aes.Key = textEncoder.GetBytes(key);
            aes.IV = textEncoder.GetBytes(iv);
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            var encryptor = aes.CreateEncryptor();
            var cipher = textEncoder.GetBytes(plaintext);
            var ciph_bytes = encryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Convert.ToBase64String(ciph_bytes);
        }
    }
}
