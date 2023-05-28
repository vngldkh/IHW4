using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using IHW4.Models;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace IHW4
{
    public static class OrderManager
    {
        private static SQLiteConnection connection;
        private static SQLiteCommand command;

        private static bool Connect(string fileName)
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

        static OrderManager()
        {
            if (Connect("db.sqlite"))
            {
                command = new SQLiteCommand(connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS orders (" +
                                      "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                      "user_id INT NOT NULL," +
                                      "status VARCHAR(50) NOT NULL," +
                                      "special_requests TEXT," +
                                      "created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                      "updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                      "FOREIGN KEY (user_id) REFERENCES users(id));";
                command.ExecuteNonQuery();
                
                command.CommandText = "CREATE TABLE IF NOT EXISTS order_dish (" +
                                      "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                      "order_id INT NOT NULL," +
                                      "dish_id INT NOT NULL," +
                                      "quantity INT NOT NULL," +
                                      "price DECIMAL(10, 2) NOT NULL," +
                                      "FOREIGN KEY (order_id) REFERENCES orders(id)," +
                                      "FOREIGN KEY (dish_id) REFERENCES dish(id));";
                command.ExecuteNonQuery();
            }
        }
        
    }
}
