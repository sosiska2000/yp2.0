using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusyController : ControllerBase
    {
        private readonly StatusyService _service;

        public StatusyController(StatusyService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetStatusy([FromQuery] string? search)
        {
            try
            {
                var statusy = _service.GetAll(search).ToList();
                return Ok(statusy);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetStatusById(int id)
        {
            try
            {
                var status = _service.GetById(id);
                if (status == null)
                {
                    return NotFound($"Статус с идентификатором {id} не найден");
                }
                return Ok(status);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateStatus([FromBody] Status status)
        {
            try
            {
                if (string.IsNullOrEmpty(status.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(status);
                return Ok(status);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteStatus(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить статус, так как он связан с оборудованием");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Статус с идентификатором {id} не найден");
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}