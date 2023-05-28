using System;
using IHW4.Models;
using System.Data.SQLite;
using System.Data;

namespace IHW4
{
    /// <summary>
    /// Класс-посредник для взаимодействия с таблицами БД, связанными с сервисом авторизации
    /// </summary>
    public static class AuthManager
    {
        private static SQLiteConnection _connection;
        private static SQLiteCommand command;


        private static bool Connect(string fileName)
        {
            try
            {
                _connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
                _connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Access exception: {ex.Message}");
                return false;
            }
        }

        static AuthManager()
        {
            if (Connect("db.sqlite"))
            {
                command = new SQLiteCommand(_connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (" +
                                    "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                    "username VARCHAR(50) NOT NULL UNIQUE," +
                                    "email VARCHAR(100) UNIQUE NOT NULL," +
                                    "password_hash VARCHAR(255) NOT NULL," +
                                    "role VARCHAR(10) NOT NULL CHECK (role IN ('customer', 'chef', 'manager'))," +
                                    "created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                    "updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS session (" +
                                      "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                      "user_id INTEGER NOT NULL," +
                                      "session_token VARCHAR(255) NOT NULL," +
                                      "expires_at TIMESTAMP NOT NULL," +
                                      "FOREIGN KEY (user_id) REFERENCES users(id));";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Создание нового аккаунта
        /// </summary>
        /// <param name="userName"> Имя пользователя </param>
        /// <param name="email"> Адрес электронной почты </param>
        /// <param name="password"> Пароль (нехэшированный) </param>
        /// <param name="role"> Роль пользователя </param>
        /// <returns> Индикатор операции </returns>
        public static bool CreateUser(String userName, String email, String password, String role)
        {
            try
            {
                command.CommandText = "INSERT INTO users (username, email, password_hash, role) VALUES (:username, :email, :password_hash, :role)";
               
                command.Parameters.AddWithValue("username", userName);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("password_hash", Hasher.HashPassword(password));
                command.Parameters.AddWithValue("role", role);

                command.ExecuteNonQuery();

                return true;
            } catch
            {
                return false;
            }
        }

        /// <summary>
        /// Получение информации о пользователе (необходимой для авторизации)
        /// </summary>
        /// <param name="email"> Адрес электронной почты </param>
        /// <returns>
        /// Хэш пароля и идентификатор пользователя (или (null, -1) если такого пользователя нет в таблице)
        /// </returns>
        public static (String, Int64) CheckUser(String email)
        {
            command.CommandText = "SELECT * FROM users WHERE email = :email";
            command.Parameters.AddWithValue("email", email);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            if (data.Rows.Count == 0)
            {
                return (null, -1);
            }
            return (data.Select()[0].Field<String>("password_hash"), data.Select()[0].Field<Int64>("id"));
        }

        /// <summary>
        /// Создание новой сессии
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя </param>
        /// <param name="token"> Сгенерированный токен </param>
        /// <returns> Индикатор операции </returns>
        public static bool CreateSession(Int64 userId, TokenInfo token)
        {
            try
            {
                command.CommandText = "INSERT INTO session (user_id, session_token, expires_at) VALUES (:user_id, :session_token, :expires_at)";

                command.Parameters.AddWithValue("user_id", userId);
                command.Parameters.AddWithValue("session_token", token.token);
                command.Parameters.AddWithValue("expires_at", token.expires);

                command.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
        } 

        /// <summary>
        /// Проверка токена на наличие активной сессии
        /// </summary>
        /// <param name="token"> Токен сессии </param>
        /// <returns> Идентификатор авторизированного пользователя (или -1 если такой активной сессии нет) </returns>
        public static Int64 CheckSession(String token)
        {
            command.CommandText = "SELECT * FROM session WHERE session_token = :token AND expires_at > CURRENT_TIMESTAMP";
            command.Parameters.AddWithValue("token", token);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            if (data.Rows.Count == 0)
            {
                return -1;
            }
            return data.Select()[0].Field<Int64>("user_id");
        }

        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        /// <param name="id"> Идентификатор пользователя </param>
        /// <returns> Информация о пользователе </returns>
        public static User GetUserInfo(Int64 id)
        {
            command.CommandText = "SELECT * FROM users WHERE id = :id";
            command.Parameters.AddWithValue("id", id);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            return new User(data.Select()[0].Field<String>("email"), data.Select()[0].Field<String>("username"), data.Select()[0].Field<String>("role"));
        }
    }
}
