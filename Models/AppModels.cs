/*
Copyright (c) 2022 Jim Thatcher

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace WebAPI.Models
{
    public class ServiceInfo
    {
        public string Version { get; set; } = "0.8.0.20220823";
        public string SchemaVersion { get; set; } = "1.0.0.0";
        public bool IsConsoleConnected {get; set;} 
        // TODO: Add an enum for console connection status (Disconnected, attempting, found serial port, connected to console)
    }

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