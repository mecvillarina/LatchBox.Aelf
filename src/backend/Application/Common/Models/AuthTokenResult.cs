using System;
using System.Security.Claims;
using Domain.Enums;

namespace Application.Common.Models
{
    public class AuthTokenResult
    {
        public ClaimsPrincipal Principal { get; private set; }

        public AuthTokenStatus Status { get; private set; }

        public Exception Exception { get; private set; }

        public static AuthTokenResult Success(ClaimsPrincipal principal)
        {
            return new AuthTokenResult()
            {
                    Principal = principal,
                    Status = AuthTokenStatus.Valid
            };
        }

        public static AuthTokenResult Expired()
        {
            return new AuthTokenResult()
            {
                    Status = AuthTokenStatus.Expired
            };
        }

        public static AuthTokenResult Error(Exception ex)
        {
            return new AuthTokenResult()
            {
                    Status = AuthTokenStatus.Error,
                    Exception = ex
            };
        }

        public static AuthTokenResult NoToken()
        {
            return new AuthTokenResult()
            {
                    Status = AuthTokenStatus.NoToken
            };
        }
    }
}