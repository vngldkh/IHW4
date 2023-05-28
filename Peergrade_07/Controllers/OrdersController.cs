using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using IHW4.Models;

namespace IHW4.Controllers
{
    /// <summary>
    /// Контроллер заказов.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class OrdersController : Controller
    {
        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <param name="token"> Токен текущей сессии </param>
        /// <param name="dishes"> Список блюд (в JSON формате) </param>
        /// <param name="specialRequests"> Особые пожелания </param>
        /// <returns> Результат запроса </returns>
        [HttpPost("create/{token}")]
        public IActionResult Post(string token, ICollection<OrderItem> dishes, string specialRequests)
        {
            if (dishes.Count == 0)
            {
                return new BadRequestObjectResult("Заказ не может быть пустым");
            }
            Int64 userId = AuthManager.CheckSession(token);
            if (userId < 0)
            {
                return new BadRequestObjectResult("Неавторизованный пользователь");
            }

            var dishList = new List<(Int64, int, decimal)>();
            foreach (var dishInfo in dishes)
            {
                Int64 id = DishManager.FindDish(dishInfo.Name);
                if (id < 0)
                {
                    return new NotFoundObjectResult($"Блюда с названием {dishInfo.Name} не найдено");
                }

                var dish = DishManager.GetDishInfo(id);
                if (dish.Quantity < dishInfo.Quantity)
                {
                    return new BadRequestObjectResult(
                        $"Недостаток блюда под названием {dishInfo.Name}. Число доступных блюд: {dish.Quantity}");
                }
                
                dishList.Add((id, dishInfo.Quantity, dish.Price));
            }

            Int64 orderId = OrderManager.CreateOrder(new Order(userId, "в ожидании", specialRequests));

            if (orderId < 0)
            {
                return new BadRequestObjectResult("Не удалось создать заказ");
            }

            foreach (var dishItem in dishList)
            {
                OrderManager.CreateOrderDish(new OrderDish(orderId, dishItem.Item1, dishItem.Item2, dishItem.Item3));
                DishManager.ChangeQuantity(dishItem.Item1, -dishItem.Item2);
            }
            
            return new OkObjectResult($"Заказ №{orderId} успешно создан");
        }

        /// <summary>
        /// Получение информации о заказе по номеру
        /// </summary>
        /// <param name="id"> Номер заказа </param>
        /// <returns> Результат запроса </returns>
        [HttpGet("get/{id}")]
        public IActionResult Get(Int64 id)
        {
            var info = OrderManager.GetOrderInfo(id);
            if (info == null)
            {
                return new NotFoundObjectResult("Заказ с таким номером не найден");
            }

            return new OkObjectResult(info);
        }
    }
}
