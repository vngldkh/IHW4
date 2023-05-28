using System;

namespace IHW4.Models
{
    public class OrderDish
    {
        public Int64 OrderID { get; set; }
        public Int64 DishID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderDish(Int64 orderId, Int64 dishId, int quantity, decimal price)
            => (OrderID, DishID, Quantity, Price) = (orderId, dishId, quantity, price);
    }
}