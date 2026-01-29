using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriiController : ControllerBase
    {
        private readonly AuditoriiService _service;

        public AuditoriiController(AuditoriiService service)
        {
            _service = service;
        }

        // СУЩЕСТВУЮЩИЕ МЕТОДЫ...

        // 1. Получить аудиторию по ID с пользователями (для WPF)
        [HttpGet("item/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var auditoria = _service.GetById(id);
                if (auditoria == null)
                {
                    return NotFound($"Аудитория с ID {id} не найдена");
                }

                return Ok(new
                {
                    Id = auditoria.Id,
                    Nazvanie = auditoria.Nazvanie,
                    SokrashennoeNazvanie = auditoria.SokrashennoeNazvanie,
                    OtvetstvennyiPolzovatelId = auditoria.OtvetstvennyiPolzovatelId,
                    VremennoOtvetstvennyiPolzovatelId = auditoria.VremennoOtvetstvennyiPolzovatelId,
                    OtvetstvennyiPolzovatel = auditoria.OtvetstvennyiPolzovatel,
                    VremennoOtvetstvennyiPolzovatel = auditoria.VremennoOtvetstvennyiPolzovatel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 2. Получить оборудование в аудитории (для кнопки "Просмотр оборудования")
        [HttpGet("equipment/{audienceId}")]
        public IActionResult GetEquipmentInAudience(int audienceId)
        {
            try
            {
                var equipment = _service.GetEquipmentInAuditoria(audienceId)
                    .Select(e => new
                    {
                        Id = e.Id,
                        Nazvanie = e.Nazvanie,
                        InventarnyiNomer = e.InventarnyiNomer,
                        Stoimost = e.Stoimost,
                        Status = e.Status?.Nazvanie,
                        TipOborudovania = e.TipOborudovania?.Nazvanie,
                        Auditoria = e.Auditoria?.Nazvanie
                    })
                    .ToList();

                return Ok(equipment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 3. Получить количество оборудования в аудитории
        [HttpGet("equipment-count/{audienceId}")]
        public IActionResult GetEquipmentCount(int audienceId)
        {
            try
            {
                var count = _service.GetEquipmentCount(audienceId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 4. Создать аудиторию
        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateAudienceRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Nazvanie))
                {
                    return BadRequest("Название аудитории обязательно");
                }

                var auditoria = new Auditoria
                {
                    Nazvanie = request.Nazvanie,
                    SokrashennoeNazvanie = request.SokrashennoeNazvanie,
                    OtvetstvennyiPolzovatelId = request.OtvetstvennyiPolzovatelId,
                    VremennoOtvetstvennyiPolzovatelId = request.VremennoOtvetstvennyiPolzovatelId
                };

                var created = _service.Create(auditoria);
                return Ok(new
                {
                    Id = created.Id,
                    Message = "Аудитория успешно создана"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 5. Обновить аудиторию
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAudienceRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("Идентификаторы не совпадают");
                }

                var auditoria = new Auditoria
                {
                    Id = request.Id,
                    Nazvanie = request.Nazvanie,
                    SokrashennoeNazvanie = request.SokrashennoeNazvanie,
                    OtvetstvennyiPolzovatelId = request.OtvetstvennyiPolzovatelId,
                    VremennoOtvetstvennyiPolzovatelId = request.VremennoOtvetstvennyiPolzovatelId
                };

                var updated = _service.Update(id, auditoria);
                if (updated == null)
                {
                    return NotFound($"Аудитория с ID {id} не найдена");
                }

                return Ok(new
                {
                    Id = updated.Id,
                    Message = "Аудитория успешно обновлена"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // 6. Удалить аудиторию
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить аудиторию, так как в ней находится оборудование");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Аудитория с ID {id} не найдена");
                }

                return Ok(new { Message = "Аудитория успешно удалена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
    }

    // DTO классы для запросов
    public class CreateAudienceRequest
    {
        public string Nazvanie { get; set; } = string.Empty;
        public string? SokrashennoeNazvanie { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
    }

    public class UpdateAudienceRequest
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
        public string? SokrashennoeNazvanie { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
    }
}