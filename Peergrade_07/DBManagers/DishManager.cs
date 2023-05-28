using System;
using System.Collections.Generic;
using System.Linq;
using IHW4.Models;
using System.Data.SQLite;
using System.Data;

namespace IHW4
{
    /// <summary>
    /// Класс-посредник для взаимодействия с таблицами БД, связанными с блюдами
    /// </summary>
    public static class DishManager
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

        static DishManager()
        {
            if (Connect("db.sqlite"))
            {
                command = new SQLiteCommand(_connection);
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
        
        /// <summary>
        /// Создание нового блюда
        /// </summary>
        /// <param name="dish"> Информация о блюде </param>
        /// <returns> Индикатор операции </returns>
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

        /// <summary>
        /// Поиск блюда по названию
        /// </summary>
        /// <param name="name"> Название блюда </param>
        /// <returns> Идентификатор блюда (или -1, если оно не было найдено в таблице) </returns>
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

        /// <summary>
        /// Изменение количества блюда
        /// </summary>
        /// <param name="id"> Идентификатор блюда </param>
        /// <param name="diff"> Величина изменения </param>
        /// <returns> Итоговое количество блюда </returns>
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
            
            command.CommandText = "UPDATE dish SET quantity = :quantity, updated_at = CURRENT_TIMESTAMP WHERE id = :id";
            command.Parameters.AddWithValue("quantity", newQuantity);
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            return newQuantity;
        }
        
        /// <summary>
        /// Удаление блюда
        /// </summary>
        /// <param name="id"> Идентификатор блюда </param>
        /// <returns> Индикатор операции </returns>
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

        /// <summary>
        /// Получение информации о блюде
        /// </summary>
        /// <param name="id"> Идентификатор блюда </param>
        /// <returns> Информация о блюде (или null, если оно не было найдено) </returns>
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

        /// <summary>
        /// Получение меню (списка доступных блюд)
        /// </summary>
        /// <returns> Список доступных блюд </returns>
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
