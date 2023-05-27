using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using IHW4.Controllers;
using System.Data.SQLite;

namespace IHW4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Connect("auth.sqlite"))
            {
                command = new SQLiteCommand(connection)
                {
                    CommandText = "CREATE TABLE IF NOT EXISTS users (" +
                                    "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                    "username VARCHAR(50) NOT NULL UNIQUE," +
                                    "email VARCHAR(100) UNIQUE NOT NULL," +
                                    "password_hash VARCHAR(255) NOT NULL," +
                                    "role VARCHAR(10) NOT NULL CHECK (role IN ('customer', 'chef', 'manager'))," +
                                    "created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                    "updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);"
                };
                command.ExecuteNonQuery();

                command = new SQLiteCommand(connection)
                {
                    CommandText = "CREATE TABLE IF NOT EXISTS session (" +
                                    "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                    "user_id INTEGER NOT NULL," +
                                    "session_token VARCHAR(255) NOT NULL," +
                                    "expires_at TIMESTAMP NOT NULL," +
                                    "FOREIGN KEY (user_id) REFERENCES users(id));"
                };
                command.ExecuteNonQuery();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static SQLiteConnection connection;
        static SQLiteCommand command;

        static public bool Connect(string fileName)
        {
            try
            {
                connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
                connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Access exception: {ex.Message}");
                return false;
            }
        }
    }
}
