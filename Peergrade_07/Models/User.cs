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
    /// Класс, описывающий пользователя.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Email пользователя (идентификатор).
        /// </summary>
        [DataMember, Required]
        public string Email { get; set; }
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [DataMember, Required]
        public string UserName { get; set; }

        [DataMember, Required]
        public string Role { get; set; }

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public User() { }
        
        /// <summary>
        /// Конструктор пользователя.
        /// </summary>
        /// <param name="email"> Идентифиактор пользователя. </param>
        /// <param name="userName"> Имя пользователя. </param>
        public User(string email, string userName, string role)
            => (Email, UserName, Role) = (email, userName, role);

        /// <summary>
        /// Изменение реализации метода Equals для проверки двух пользователей на равенство.
        /// </summary>
        /// <param name="obj"> Объект, с которым происходит сравнение. </param>
        /// <returns> True, если объекты равны, false - в ином случае. </returns>
        public override bool Equals(object obj)
            => obj is User u && this.Email == u.Email;
    }
}
