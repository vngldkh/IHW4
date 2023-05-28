using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IHW4.Models
{
    /// <summary>
    /// Класс, описывающий блюда во время создания заказа
    /// </summary>
    [DataContract]
    public class OrderItem
    {
        /// <summary>
        /// Название блюда
        /// </summary>
        [DataMember, Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Количество
        /// </summary>
        [DataMember, Required]
        public int Quantity { get; set; }
    }
}