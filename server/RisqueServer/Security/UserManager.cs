using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RisqueServer.Security {
     class UserManager {
        private User defaultUser = null;
        private int defaultSecurityValue = -1;
        private readonly SecurityManager originalManager;
        //private Dictionary<string, UserValue> userDictionary;
        
        public UserManager(User defaultUser, int defaultSecurityValue, SecurityManager originalManager) {
            this.defaultUser = defaultUser;
            this.originalManager = originalManager;
            this.defaultSecurityValue = defaultSecurityValue;
        }
        //public addUser
        public bool containsUser(string userName) {
            throw new NotImplementedException();
        }
        public User getDefault(SecurityManager manager, out int defaultSecurityValue) {
            if (manager == originalManager) {
                defaultSecurityValue = this.defaultSecurityValue;
                return defaultUser;
            }
            else {
                defaultSecurityValue = int.MinValue;
                return null;
            }
        }
    }
    public class User {
        private string name = null;             //Leave unencrypted for now
        private string pass = null;             //Leave unencrypted for now
        private string email = null;            //Leave unencrypted for now
        private readonly int securityValue;
        public static readonly string userFileHeader = "######BEGIN USER FILE######";
        public static readonly string userFileFooter = "######END USER FILE######";
        
        public User(string user, string pass, string email, out int securityValue) {
            this.name = user;
            this.pass = pass;
            this.email = email;
            int secValue = SecurityManager.randomSecurityValue();
            this.securityValue = secValue;
            securityValue = secValue;
        }
        public string createUserFileContents(int securityValue) {
            if (securityValue == this.securityValue) {
                StringBuilder userBuilder = new StringBuilder();
                userBuilder.AppendFormat("{0}\n", userFileHeader);
                userBuilder.AppendFormat("username={0}", this.name);
                userBuilder.AppendLine();
                userBuilder.AppendFormat("email={0}", this.email);
                userBuilder.AppendLine();
                userBuilder.AppendFormat("pass={0}", this.pass);
                userBuilder.AppendLine();
                userBuilder.Append(userFileFooter);
                return userBuilder.ToString();
            }
            return null;
        }
        public static User parseUserFile(string[] userFileLines, out int securityValue) {
            if (SecurityManager.isValidUserFile(userFileLines)) {
                string name = null, email = null, pass = null;
                foreach (string line in userFileLines) {
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
                return new User(name, pass, email, out securityValue);
            }
            else {
                throw new Exception("Not a valid UserFile!");
            }
        }

        /// <summary>
        /// Returns the username of the User
        /// </summary>
        /// <param name="securityValue"></param>
        /// <returns>Username if the securityValue matches the Users securityValue, else returns null</returns>
        public string getUser(int securityValue) {
            if (securityValue == this.securityValue) {
                return this.name;
            }
            return null;
        }
        /// <summary>
        /// Returns the password of the User
        /// </summary>
        /// <param name="securityValue"></param>
        /// <returns>Password if the securityValue matches the Users securityValue, else returns null</returns>
        public string getPass(int securityValue) {
            if (securityValue == this.securityValue) {
                return this.pass;
            }
            return null;
        }
        /// <summary>
        /// Returns the email of the User
        /// </summary>
        /// <param name="securityValue"></param>
        /// <returns>Email if the securityValue matches the Users securityValue, else returns null</returns>
        public string getEmail(int securityValue) {
            if (securityValue == this.securityValue) {
                return this.email;
            }
            return null;
        }
    }
    class UserValue {
        int securityValue { get; set; }
        User user { get; set; }
    }
}
