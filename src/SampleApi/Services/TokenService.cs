using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleApi.Options;

namespace SampleApi.Services
{
    internal class TokenService : ITokenService
    {
        private readonly TokenOptions _tokenOptions;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SigningCredentials _signingCredentials;

        public TokenService(IOptions<TokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
            _tokenHandler = new JwtSecurityTokenHandler();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SecretKey));
            _signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateBearerToken(int userId)
        {
            var userClaims = new List<Claim>
            {
                // Allows the middleware to log the UserId
                new Claim(ClaimTypes.Name, userId.ToString())
            };

            var now = DateTime.UtcNow;
            var expires = now.AddYears(10);

            var accessToken = new JwtSecurityToken(
                _tokenOptions.Issuer,
                claims: userClaims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentials,
                audience: _tokenOptions.Audience);

            return _tokenHandler.WriteToken(accessToken);
        }
    }

    public interface ITokenService
    {
        string GenerateBearerToken(int userId);
    }
}
