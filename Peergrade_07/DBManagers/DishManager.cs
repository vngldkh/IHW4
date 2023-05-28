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
    public static class DishManager
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

        static DishManager()
        {
            if (Connect("db.sqlite"))
            {
                command = new SQLiteCommand(connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS dish (" +
                                      "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," + 
                                      "name VARCHAR(100) NOT NULL UNIQUE," +
                                      "description TEXT," +
                                      "price DECIMAL(10, 2) NOT NULL," +
                                      "quantity INT NOT NULL," +
                                      "created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                      "updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
                command.ExecuteNonQuery();
            }
        }
        
        public static bool CreateDish(Dish dish)
        {
            command.CommandText = "INSERT INTO dish (name, description, price, quantity)" +
                                  "VALUES (:name, :description, :price, :quantity)";
            command.Parameters.AddWithValue("name", dish.Name);
            command.Parameters.AddWithValue("description", dish.Description);
            command.Parameters.AddWithValue("price", dish.Price);
            command.Parameters.AddWithValue("quantity", dish.Quantity);

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

        public static Int64 FindDish(string name)
        {
            command.CommandText = "SELECT * FROM dish WHERE name = :name";
            command.Parameters.AddWithValue("name", name);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            if (data.Rows.Count == 0)
            {
                return -1;
            }
            return data.Select()[0].Field<Int64>("id");
        }

        public static int ChangeQuantity(Int64 id, int diff)
        {
            command.CommandText = "SELECT * FROM dish WHERE id = :id";
            command.Parameters.AddWithValue("id", id);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            
            if (data.Rows.Count == 0)
            {
                return -1;
            }
            int quantity = data.Select()[0].Field<int>("quantity");
            int newQuantity = Math.Max(0, quantity + diff);
            
            command.CommandText = "UPDATE dish SET quantity = :quantity WHERE id = :id";
            command.Parameters.AddWithValue("quantity", newQuantity);
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            return newQuantity;
        }
        
        public static bool DeleteDish(Int64 id)
        {
            command.CommandText = "DELETE FROM dish WHERE id = :id";
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

        public static Dish GetDishInfo(Int64 id)
        {
            command.CommandText = "SELECT * FROM dish WHERE id = :id";
            command.Parameters.AddWithValue("id", id);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            
            if (data.Rows.Count == 0)
            {
                return null;
            }

            return new Dish(data.Select()[0].Field<string>("name"), 
                            data.Select()[0].Field<string>("description"),
                            data.Select()[0].Field<decimal>("price"),
                            data.Select()[0].Field<int>("quantity"));
        }

        public static List<Dish> GetMenu()
        {
            command.CommandText = "SELECT * FROM dish WHERE quantity > 0";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            return (from DataRow row in data.Rows 
                        select new Dish(
                            row.Field<string>("name"), 
                            row.Field<string>("description"), 
                            row.Field<decimal>("price"), 
                            row.Field<int>("quantity")
                            )
                        ).ToList();
        }
    }
}
