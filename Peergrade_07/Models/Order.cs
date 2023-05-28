using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IHW4.Models
{
    /// <summary>
    /// Класс, описывающий заказ
    /// </summary>
    [DataContract]
    public class Order
    {
        /// <summary>
        /// Идентификатор пользователя (который сделал заказ)
        /// </summary>
        [DataMember, Required]
        public Int64 UserID { get; set; }
        /// <summary>
        /// Статус заказа
        /// </summary>
        [DataMember, Required]
        public String Status { get; set; }
        /// <summary>
        /// Особые пожелания к заказу
        /// </summary>
        [DataMember]
        public String SpecialRequests { get; set; }
        /// <summary>
        /// Время создания заказа
        /// </summary>
        [DataMember, Required]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Время обновления заказа
        /// </summary>
        [DataMember, Required]
        public DateTime UpdatedAt { get; set; }
        
        public Order(long userId, string status, string specialRequests)
        {
            UserID = userId;
            Status = status;
            SpecialRequests = specialRequests;
        }
        
        public Order(long userId, string status, string specialRequests, DateTime createdAt, DateTime updatedAt)
        {
            UserID = userId;
            Status = status;
            SpecialRequests = specialRequests;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}