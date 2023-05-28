using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IHW4.Models;
using System.Text.RegularExpressions;

namespace IHW4.Controllers
{
    /// <summary>
    /// Контроллер пользователей.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : Controller
    {
        public static List<User> Users;
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="password">Пароль</param>
        /// <param name="role">Роль (chef, manager или customer)</param>
        /// <returns></returns>
        [HttpPost("register")]
        public IActionResult Post(string userName, string email, string password, string role)
        {
            Regex regex = new Regex("^\\S+@\\S+\\.\\S+$");
            String[] validRoles = {"chef", "manager", "customer"};
            if (userName is null || userName.Length == 0)
            {
                return new BadRequestObjectResult("Имя пользователя не может быть пустым");
            }
            if (email is null || !regex.IsMatch(email))
            {
                return new BadRequestObjectResult("Некорректный адрес электронной почты");
            }
            if (password is null)
            {
                return new BadRequestObjectResult("Пароль не может быть пустым");
            }
            if (role is null || !validRoles.Contains(role))
            {
                return new BadRequestObjectResult("Некорректная роль");
            }
            if (DBManager.CreateUser(userName, email, password, role))
            {
                return new OkObjectResult("Регистрация прошла успешно");
            }
            return new BadRequestObjectResult("Пользователь с таким именем или адресом электронной почты уже существует");
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <returns> Результат запроса. </returns>
        [HttpPost("login")]
        public IActionResult Post(String email, String password)
        {
            if (User is null || password is null)
            {
                return new BadRequestObjectResult("Поля не могут быть пустыми");
            }

            String hash;
            Int64 id;

            (hash, id)  = DBManager.CheckUser(email);
            if (hash is null)
            {
                return new BadRequestObjectResult("Пользователя с таким адресом электронной почты не существует");
            }
            if (!Hasher.CheckPassword(password, hash))
            {
                return new BadRequestObjectResult("Неверный пароль");
            }

            var token = JWTGenerator.Generate();
            if (DBManager.CreateSession(id, token))
            {
                return new OkObjectResult($"Вход прошёл успешно. Сгенерированный токен: {token.token}");
            }
            return new BadRequestObjectResult("Не удалось создать сессию");
        }

        [HttpGet("user_info/{sessionToken}")]
        public IActionResult Get(String sessionToken)
        {
            Int64 userId = DBManager.CheckSession(sessionToken);
            if (userId < 0)
            {
                return new BadRequestObjectResult("Нет активной сессии с таким токеном");
            }
            return new OkObjectResult(DBManager.GetUserInfo(userId));
        }
    }
}
