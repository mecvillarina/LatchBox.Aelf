using Client.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IAuthTokenService
    {
        AuthTokenHandler GenerateToken(Dictionary<string, string> claimDict, DateTime? expires = null);
        AuthTokenResult ValidateToken(string token);
    }
}