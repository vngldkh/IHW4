using System;

namespace IHW4.Models
{
    /// <summary>
    /// Класс, содержащий информацию о блюде в заказе
    /// </summary>
    public class OrderDish
    {
        /// <summary>
        /// Идентификатор соответствующего заказа
        /// </summary>
        public Int64 OrderID { get; }
        /// <summary>
        /// Идентификатор соответствующего блюда
        /// </summary>
        public Int64 DishID { get; }
        /// <summary>
        /// Количество
        /// </summary>
        public int Quantity { get; }
        /// <summary>
        /// Стоимость во время заказа
        /// </summary>
        public decimal Price { get; }
        
        public OrderDish(Int64 orderId, Int64 dishId, int quantity, decimal price)
            => (OrderID, DishID, Quantity, Price) = (orderId, dishId, quantity, price);
    }
}