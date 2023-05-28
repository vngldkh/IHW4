using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IHW4.Models
{
    /// <summary>
    /// Класс, описывающий блюдо.
    /// </summary>
    [DataContract]
    public class Dish
    {
        [DataMember, Required]
        public string Name { get; set; }
        
        [DataMember, Required]
        public string Description { get; set; }

        [DataMember, Required]
        public decimal Price { get; set; }
        
        [DataMember, Required]
        public int Quantity { get; set; }
        
        public Dish(string name, string description, decimal price, int quantity)
            => (Name, Description, Price, Quantity) = (name, description, price, quantity);
    }
}
