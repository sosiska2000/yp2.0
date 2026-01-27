using Microsoft.AspNetCore.Mvc;
using RestAPI.Connect;
using RestAPI.Models;
using System;
using System.Linq;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolzovateliController : ControllerBase
    {
        [HttpGet("list")]
        public IActionResult GetPolzovateli()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var users = context.Polzovateli.ToList();
                    foreach (var user in users) user.Parol = "";
                    return Ok(users);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetPolzovatelById(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Polzovateli.FirstOrDefault(x => x.Id == id);
                    if (user == null) return NotFound($"Пользователь с ID {id} не найден");
                    user.Parol = "";
                    return Ok(user);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item")]
        public IActionResult GetPolzovatel([FromQuery] int id)
        {
            if (id <= 0) return BadRequest("ID должен быть положительным числом");

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Polzovateli.FirstOrDefault(x => x.Id == id);
                    if (user == null) return NotFound($"Пользователь с ID {id} не найден");
                    user.Parol = "";
                    return Ok(user);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreatePolzovatel([FromBody] Polzovatel user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Parol) || string.IsNullOrEmpty(user.Familiia))
                    return BadRequest("Логин, пароль и фамилия обязательны");

                using (var context = new ApplicationDbContext())
                {
                    if (context.Polzovateli.Any(p => p.Login == user.Login))
                        return BadRequest("Логин уже существует");

                    context.Polzovateli.Add(user);
                    context.SaveChanges();
                    user.Parol = "";
                    return Ok(user);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeletePolzovatel(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Polzovateli.FirstOrDefault(x => x.Id == id);
                    if (user == null) return NotFound($"Пользователь с ID {id} не найден");

                    bool hasConnections = context.Oborudovanie.Any(o => o.OtvetstvennyiPolzovatelId == id) ||
                                          context.Auditorii.Any(a => a.OtvetstvennyiPolzovatelId == id);

                    if (hasConnections) return BadRequest("Пользователь связан с другими данными");

                    context.Polzovateli.Remove(user);
                    context.SaveChanges();
                    return Ok("Пользователь удален");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}