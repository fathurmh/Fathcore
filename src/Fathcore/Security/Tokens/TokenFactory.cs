using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fathcore.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fathcore.Security.Tokens
{
    /// <summary>
    /// Represents a token factory.
    /// </summary>
    [RegisterService(Lifetime.Scoped)]
    public class TokenFactory : ITokenFactory
    {
        private SecurityTokenDescriptor _tokenDescriptor;
        private readonly ITokenSetting _tokenSetting;

        /// <summary>
        /// Gets token descriptor expires value.
        /// </summary>
        public DateTime Expires => _tokenDescriptor.Expires.Value;

        public TokenFactory(ITokenSetting tokenSetting)
        {
            _tokenSetting = tokenSetting;
        }

        /// <summary>
        /// Generate token with customable claims.
        /// </summary>
        /// <param name="claims">Given claims.</param>
        /// <returns>Generated token.</returns>
        public string GenerateToken(params Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_tokenSetting.Key);

            _tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _tokenSetting.Audience,
                Issuer = _tokenSetting.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(_tokenSetting.Expires),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(_tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}
