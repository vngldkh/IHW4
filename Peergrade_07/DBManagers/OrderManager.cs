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

        public static bool CreateOrderDish(OrderDish dish)
        {
            command.CommandText = "INSERT INTO order_dish (order_id, dish_id, quantity, price)" +
                                  "VALUES (:order_id, :dish_id, :quantity, :price)";
            command.Parameters.AddWithValue("order_id", dish.OrderID);
            command.Parameters.AddWithValue("dish_id", dish.DishID);
            command.Parameters.AddWithValue("quantity", dish.Quantity);
            command.Parameters.AddWithValue("price", dish.Price);
            
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static Int64 CreateOrder(Order order)
        {
            command.CommandText = "INSERT INTO orders (user_id, status, special_requests)" +
                                  "VALUES (:user_id, :status, :special_requests);";
            command.Parameters.AddWithValue("user_id", order.UserID);
            command.Parameters.AddWithValue("status", order.Status);
            command.Parameters.AddWithValue("special_requests", order.SpecialRequests);
            
            try
            {
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid()";
                return (Int64) command.ExecuteScalar();
            }
            catch
            {
                return -1;
            }
        }

        public static Order GetOrderInfo(Int64 id)
        {
            command.CommandText = "SELECT * FROM orders WHERE id = :id";
            command.Parameters.AddWithValue("id", id);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            
            if (data.Rows.Count == 0)
            {
                return null;
            }

            return new Order(data.Select()[0].Field<int>("user_id"),
                                data.Select()[0].Field<string>("status"),
                                data.Select()[0].Field<string>("special_requests"),
                                data.Select()[0].Field<DateTime>("created_at"),
                                data.Select()[0].Field<DateTime>("updated_at"));
        }

        public static List<Int64> GetAwaitingOrders()
        {
            command.CommandText = "SELECT * FROM orders WHERE status = 'в ожидании'";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            return (from DataRow row in data.Rows select row.Field<Int64>("id")).ToList();
        }

        public static bool UpdateOrderState(Int64 id, String state)
        {
            command.CommandText = "UPDATE orders SET status = :state, updated_at = CURRENT_TIMESTAMP WHERE id = :id";
            command.Parameters.AddWithValue("state", state);
            command.Parameters.AddWithValue("id", id);

            try
            {
                command.ExecuteNonQuery();
                return true;
            } 
            catch
            {
                return false;
            }
        }
    }
}
