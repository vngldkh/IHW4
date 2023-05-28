using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IHW4
{
    public record TokenInfo(String token, DateTime expires);

    public static class JWTGenerator
    {
        const string KEY = "mysupersecret_secretkey!IT_IS_REALLY_SUPER_SECRET!TRUST_ME";   // ключ для шифрации
        public const int LIFETIME = 5; // время жизни токена - 5 минут
        private static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        public static TokenInfo Generate()
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromMinutes(LIFETIME));
            var jwt = new JwtSecurityToken(
                    notBefore: now,
                    expires: expires,
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenInfo(encodedJwt, expires);
        }
    }
}
