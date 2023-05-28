using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IHW4
{
    /// <summary>
    /// Информация о токене
    /// </summary>
    /// <param name="token"> Закодированный токен </param>
    /// <param name="expires"> Дата окончания действия </param>
    public record TokenInfo(String token, DateTime expires);

    /// <summary>
    /// Класс для создания JWT токена
    /// </summary>
    public static class JWTGenerator
    {
        const string KEY = "mysupersecret_secretkey!IT_IS_REALLY_SUPER_SECRET!TRUST_ME";   // ключ шифрования
        public const int LIFETIME = 5; // время жизни токена - 5 минут
        private static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        /// <summary>
        /// Генерация токена
        /// </summary>
        /// <returns> Информация о сгенерированном токене </returns>
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
