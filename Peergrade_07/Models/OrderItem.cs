using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IHW4.Models
{
    [DataContract]
    public class OrderItem
    {
        [DataMember, Required]
        public string Name { get; set; }
        
        [DataMember, Required]
        public int Quantity { get; set; }
    }
}