using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Client.Infrastructure.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly AppSettings _appSettings;
        private readonly IDateTime _dateTime;
        public AuthTokenService(IOptions<AppSettings> settingsOptions, IDateTime dateTime)
        {
            _appSettings = settingsOptions.Value;
            _dateTime = dateTime;
        }

        public AuthTokenHandler GenerateToken(Dictionary<string, string> claimDict, DateTime? expires = null)
        {
            var jwtAudience = _appSettings.JwtAudience;
            var jwtIssuer = _appSettings.JwtIssuer;
            var jwtSecret = _appSettings.JwtSecret;

            if (!expires.HasValue) expires = _dateTime.UtcNow.AddDays(90);

            var utcExpires = DateTime.SpecifyKind(expires.Value, DateTimeKind.Utc);

            claimDict ??= new Dictionary<string, string>();
            var claims = claimDict.Select(x => new Claim(x.Key, x.Value)).ToList();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = utcExpires,
                Audience = jwtAudience,
                Issuer = jwtIssuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = (JwtSecurityToken)token;

            return new AuthTokenHandler
            {
                Token = tokenHandler.WriteToken(token),
                ExpireAt = long.Parse(jwtToken.Claims.First(x => x.Type == "exp").Value)
            };
        }

        public AuthTokenResult ValidateToken(string token)
        {
            try
            {
                var jwtAudience = _appSettings.JwtAudience;
                var jwtIssuer = _appSettings.JwtIssuer;
                var jwtSecret = _appSettings.JwtSecret;

                var tokenParams = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = jwtAudience,
                    ValidateAudience = true,
                    ValidIssuer = jwtIssuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret))
                };

                var handler = new JwtSecurityTokenHandler();
                var result = handler.ValidateToken(token, tokenParams, out var securityToken);
                return AuthTokenResult.Success(result);
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthTokenResult.Expired();
            }
            catch (Exception ex)
            {
                return AuthTokenResult.Error(ex);
            }
        }
    }
}