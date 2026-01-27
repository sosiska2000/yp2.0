using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Connect;
using RestAPI.Models;
using System;
using System.Linq;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OborudovanieController : ControllerBase
    {
        [HttpGet("list")]
        public IActionResult GetOborudovanie([FromQuery] string? search = null, [FromQuery] string? sort = null)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var query = context.Oborudovanie
                        .Include(o => o.Auditoria)
                        .Include(o => o.OtvetstvennyiPolzovatel)
                        .Include(o => o.Status)
                        .Include(o => o.TipOborudovania)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(search))
                        query = query.Where(o => o.Nazvanie.Contains(search));

                    query = sort?.ToLower() switch
                    {
                        "nazvanie" => query.OrderBy(o => o.Nazvanie),
                        "inventarnyi_nomer" => query.OrderBy(o => o.InventarnyiNomer),
                        "stoimost" => query.OrderBy(o => o.Stoimost),
                        _ => query.OrderBy(o => o.Id)
                    };

                    return Ok(query.ToList());
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetOborudovanieById(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var equipment = context.Oborudovanie
                        .Include(o => o.Auditoria)
                        .Include(o => o.OtvetstvennyiPolzovatel)
                        .Include(o => o.Status)
                        .Include(o => o.TipOborudovania)
                        .FirstOrDefault(x => x.Id == id);

                    if (equipment == null) return NotFound($"Оборудование с ID {id} не найдено");
                    return Ok(equipment);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateOborudovanie([FromBody] Oborudovanie equipment)
        {
            try
            {
                if (string.IsNullOrEmpty(equipment.Nazvanie) || string.IsNullOrEmpty(equipment.InventarnyiNomer))
                    return BadRequest("Название и инвентарный номер обязательны");

                using (var context = new ApplicationDbContext())
                {
                    if (context.Oborudovanie.Any(o => o.InventarnyiNomer == equipment.InventarnyiNomer))
                        return BadRequest("Инвентарный номер уже существует");

                    if (!decimal.TryParse(equipment.Stoimost.ToString(), out _))
                        return BadRequest("Стоимость должна быть числом");

                    context.Oborudovanie.Add(equipment);
                    context.SaveChanges();
                    return Ok(equipment);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateOborudovanie(int id, [FromBody] Oborudovanie equipment)
        {
            try
            {
                if (id != equipment.Id) return BadRequest("ID не совпадает");

                using (var context = new ApplicationDbContext())
                {
                    var existing = context.Oborudovanie.FirstOrDefault(x => x.Id == id);
                    if (existing == null) return NotFound($"Оборудование с ID {id} не найдено");

                    if (string.IsNullOrEmpty(equipment.Nazvanie) || string.IsNullOrEmpty(equipment.InventarnyiNomer))
                        return BadRequest("Название и инвентарный номер обязательны");

                    if (equipment.InventarnyiNomer != existing.InventarnyiNomer &&
                        context.Oborudovanie.Any(o => o.InventarnyiNomer == equipment.InventarnyiNomer))
                        return BadRequest("Инвентарный номер уже существует");

                    context.Entry(existing).CurrentValues.SetValues(equipment);
                    context.SaveChanges();
                    return Ok(equipment);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteOborudovanie(int id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var equipment = context.Oborudovanie.FirstOrDefault(x => x.Id == id);
                    if (equipment == null) return NotFound($"Оборудование с ID {id} не найдено");

                    bool hasConnections = context.SetevyeNastroiki.Any(s => s.OborudovanieId == id) ||
                                          context.InventarizatsiaDetali.Any(i => i.OborudovanieId == id);

                    if (hasConnections) return BadRequest("Оборудование связано с другими данными");

                    context.Oborudovanie.Remove(equipment);
                    context.SaveChanges();
                    return Ok("Оборудование удалено");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}