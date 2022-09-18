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

// Borrowed from Moshe Binieli's GitHub repository: https://github.com/MosheWorld/AuthenticationService/tree/master/AuthenticationService%20C%23
// Based on his post at https://medium.com/@mmoshikoo/jwt-authentication-using-c-54e0c71f21b0
// Adapted for this simple project.

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Models
{
    public interface IAuthContainerModel
    {
        #region Members
        UInt64 SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }

        Claim[] Claims { get; set; }
        #endregion
    }
    public class JWTContainerModel : IAuthContainerModel
    {
        #region Public Methods
        public int ExpireMinutes { get; set; } = 10080 * 5; // 7 days * 5 = 35 days
        public UInt64 SecretKey { get; set; } = 0; // This secret key will be pulled from database at time token is generated.
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;

        public Claim[] Claims { get; set; } = new Claim[] { };
        #endregion
    }
}