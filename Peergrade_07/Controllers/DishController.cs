﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IHW4.Models;
using System.Text.RegularExpressions;

namespace IHW4.Controllers
{
    /// <summary>
    /// Контроллер пользователей.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class DishController : Controller
    {
        [HttpPost("create/{token}")]
        public IActionResult Post(string name, string description, decimal price, int quantity, string token)
        {
            Int64 res = AuthManager.CheckSession(token);
            if (res < 0 || AuthManager.GetUserInfo(res).Role != "manager")
            {
                return new BadRequestObjectResult("Недостаточно прав");
            }

            var dish = new Dish(name, description, price, quantity);
            if (!DishManager.CreateDish(dish))
            {
                return new BadRequestObjectResult("Такое блюдо уже существует");
            }

            return new OkObjectResult("Блюдо было успешно добавлено");
        }
        
       
        [HttpPost("change_quantity/{token}")]
        public IActionResult Post(string name, int quantity, string token)
        {
            Int64 res = AuthManager.CheckSession(token);
            if (res < 0 || AuthManager.GetUserInfo(res).Role != "manager")
            {
                return new BadRequestObjectResult("Недостаточно прав");
            }

            Int64 dishId = DishManager.FindDish(name);
            if (dishId < 0)
            {
                return new NotFoundObjectResult("Блюда с таким названием не найдено");
            }
            
            int newQuantity = DishManager.ChangeQuantity(dishId, quantity);
            return new OkObjectResult($"Блюдо было успешно обновлено. Количество установлено на {newQuantity}");
        }
        
        [HttpPost("delete/{token}")]
        public IActionResult Post(string name, string token)
        {
            Int64 res = AuthManager.CheckSession(token);
            if (res < 0 || AuthManager.GetUserInfo(res).Role != "manager")
            {
                return new BadRequestObjectResult("Недостаточно прав");
            }

            Int64 dishId = DishManager.FindDish(name);
            if (dishId < 0)
            {
                return new NotFoundObjectResult("Блюда с таким названием не найдено");
            }

            if (!DishManager.DeleteDish(dishId))
            {
                return new BadRequestObjectResult("Удаление не удалось");
            }
            return new OkObjectResult($"Блюдо было успешно удалено");
        }
        
        [HttpGet("info/{token}")]
        public IActionResult Get(string name, string token)
        {
            Int64 res = AuthManager.CheckSession(token);
            if (res < 0 || AuthManager.GetUserInfo(res).Role != "manager")
            {
                return new BadRequestObjectResult("Недостаточно прав");
            }

            Int64 dishId = DishManager.FindDish(name);
            if (dishId < 0)
            {
                return new NotFoundObjectResult("Блюда с таким названием не найдено");
            }

            var dish = DishManager.GetDishInfo(dishId);
            if (dish == null)
            {
                return new BadRequestObjectResult("Произошла ошибка");
            }
            return new OkObjectResult(dish);
        }
        
        [HttpGet("menu")]
        public IActionResult Get()
        {
            var menu = DishManager.GetMenu();
            if (menu.Count == 0)
            {
                return new NotFoundObjectResult("Меню пустое");
            }
            return new OkObjectResult(menu);
        }
    }
}