using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace IHW4.Models
{
    /// <summary>
    /// Класс, описывающий блюдо.
    /// </summary>
    [DataContract]
    public class Dish
    {
        /// <summary>
        /// Название блюда
        /// </summary>
        [DataMember, Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Описание блюда
        /// </summary>
        [DataMember, Required]
        public string Description { get; set; }

        /// <summary>
        /// Стоимость блюда
        /// </summary>
        [DataMember, Required]
        public decimal Price { get; set; }
        
        /// <summary>
        /// Количество
        /// </summary>
        [DataMember, Required]
        public int Quantity { get; set; }
        
        public Dish(string name, string description, decimal price, int quantity)
            => (Name, Description, Price, Quantity) = (name, description, price, quantity);
    }
}
