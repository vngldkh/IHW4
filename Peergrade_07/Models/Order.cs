using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IHW4.Models
{
    [DataContract]
    public class Order
    {
        [DataMember, Required]
        public Int64 UserID { get; set; }
        [DataMember, Required]
        public String Status { get; set; }
        [DataMember]
        public String SpecialRequests { get; set; }
        [DataMember, Required]
        public DateTime CreatedAt { get; set; }
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