using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace WebAPI.Models
{
    public class Features
    {
        public bool device { get; set; }
        public bool project { get; set; }
        public bool security { get; set; }
        public bool mqtt { get; set; }
        public bool ntp { get; set; }
        public bool ota { get; set; }
        public bool upload_firmware { get; set; }
    }

    public class SignInRequest
    {
        public string username {get; set;} = string.Empty;
        public string password {get; set;} = string.Empty;
    }

    public class UserRecord
    {
        public long id { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public bool admin { get; set; } = false;
    }

    public class jwt_seed
    {
        public long id { get; set; }
        public UInt64 seed { get; set; }
    }

    public class getSecuritySettings
    {
        public getSecuritySettings(UInt64 seed) {
            this.jwt_value = seed;
        }
        private UInt64 jwt_value { get; set; }
        public string jwt_secret {
            get {
                int highWord = (int)(jwt_value >> 32);
                int lowWord = (int)(jwt_value & 0xFFFFFFFF);
                return string.Format("{0:x8}-{1:x8}", highWord, lowWord);
            }
        }
        public UserRecord[] users { get; set; } = new UserRecord[] { };
    }

    public class setSecuritySettings
    {
        public string jwt_secret { get; set; } = "0badf00d-cafe1234";
        public UserRecord[] users { get; set; } = new UserRecord[] { };
    }

    public class AuthResult
    {
        public string access_token {get; set;} = string.Empty;
    }

    public class JWTContent
    {
        public string username {get; set;} = string.Empty;
        public bool admin {get; set;}
    }
}