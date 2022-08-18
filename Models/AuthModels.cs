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