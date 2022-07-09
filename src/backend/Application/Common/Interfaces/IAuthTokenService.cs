using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IAuthTokenService
    {
        AuthTokenHandler GenerateToken(Dictionary<string, string> claimDict, DateTime? expires = null);
        AuthTokenResult ValidateToken(string token);
    }
}