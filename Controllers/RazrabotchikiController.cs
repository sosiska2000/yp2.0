using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;
using System.Linq;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RazrabotchikiController : ControllerBase
    {
        private readonly RazrabotchikiService _service;

        public RazrabotchikiController(RazrabotchikiService service)
        {
            _service = service;
        }

        // СУЩЕСТВУЮЩИЕ МЕТОДЫ...

        // 1. Получить разработчика по ID (для WPF)
        [HttpGet("item/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var razrabotchik = _service.GetById(id);
                if (razrabotchik == null)
                {
                    return NotFound($"Разработчик с ID {id} не найден");
                }

                return Ok(new
                {
                    Id = razrabotchik.Id,
                    Nazvanie = razrabotchik.Nazvanie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 2. Создать разработчика
        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateDeveloperRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Nazvanie))
                {
                    return BadRequest("Название разработчика обязательно");
                }

                var razrabotchik = new Razrabotchik
                {
                    Nazvanie = request.Nazvanie
                };

                _service.Create(razrabotchik);
                return Ok(new
                {
                    Id = razrabotchik.Id,
                    Message = "Разработчик успешно создан"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 3. Обновить разработчика
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDeveloperRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("Идентификаторы не совпадают");
                }

                var razrabotchik = new Razrabotchik
                {
                    Id = request.Id,
                    Nazvanie = request.Nazvanie
                };

                _service.Update(razrabotchik);
                return Ok(new
                {
                    Id = razrabotchik.Id,
                    Message = "Разработчик успешно обновлен"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 4. Удалить разработчика
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить разработчика, так как у него есть связанные программы");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Разработчик с ID {id} не найден");
                }

                return Ok(new { Message = "Разработчик успешно удален" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 5. Получить программы разработчика (для кнопки "Просмотр программ")
        [HttpGet("{id}/programs")]
        public IActionResult GetDeveloperPrograms(int id)
        {
            try
            {
                // Нужно добавить метод в RazrabotchikiService
                // или использовать ProgrammyService
                // Пока заглушка
                return Ok(new
                {
                    DeveloperId = id,
                    Programs = new List<object>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
    }

    // DTO классы для запросов
    public class CreateDeveloperRequest
    {
        public string Nazvanie { get; set; } = string.Empty;
    }

    public class UpdateDeveloperRequest
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
    }
}