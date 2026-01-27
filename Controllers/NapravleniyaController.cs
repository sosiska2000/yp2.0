using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NapravleniyaController : ControllerBase
    {
        private readonly NapravleniyaService _service;

        public NapravleniyaController(NapravleniyaService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetNapravleniya([FromQuery] string? search)
        {
            try
            {
                var napravleniya = _service.GetAll(search).ToList();
                return Ok(napravleniya);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetNapravlenieById(int id)
        {
            try
            {
                var napravlenie = _service.GetById(id);
                if (napravlenie == null)
                {
                    return NotFound($"Направление с идентификатором {id} не найдено");
                }
                return Ok(napravlenie);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateNapravlenie([FromBody] Napravlenie napravlenie)
        {
            try
            {
                if (string.IsNullOrEmpty(napravlenie.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(napravlenie);
                return Ok(napravlenie);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateNapravlenie(int id, [FromBody] Napravlenie napravlenie)
        {
            try
            {
                if (id != napravlenie.Id)
                {
                    return BadRequest("Идентификаторы не совпадают");
                }

                _service.Update(napravlenie);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteNapravlenie(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить направление, так как оно связано с оборудованием");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Направление с идентификатором {id} не найдено");
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