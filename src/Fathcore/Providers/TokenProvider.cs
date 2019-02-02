using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Fathcore.Configuration;
using Fathcore.Providers.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Fathcore.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private SecurityTokenDescriptor _tokenDescriptor;
        private SecurityToken _securityToken;
        private string _token;

        /// <summary>
        /// Return generated token
        /// </summary>
        /// <value></value>
        public string Token => _token ?? GenerateToken();

        /// <summary>
        /// Return expiry of generated token
        /// </summary>
        public DateTime Expires => _securityToken.ValidTo;

        public TokenProvider(JwtAuthentication jwtAuthentication)
        {
            _jwtAuthentication = jwtAuthentication;

        }

        /// <summary>
        /// Generate a token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string GenerateToken(params Claim[] claims)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_jwtAuthentication.Key);

            _tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _jwtAuthentication.Audience,
                Issuer = _jwtAuthentication.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(_jwtAuthentication.Expires),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            _securityToken = tokenHandler.CreateToken(_tokenDescriptor);
            _token = tokenHandler.WriteToken(_securityToken);

            return _token;
        }
    }
}
