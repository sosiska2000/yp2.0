using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarizatsiaController : ControllerBase
    {
        private readonly InventarizatsiaService _service;

        public InventarizatsiaController(InventarizatsiaService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetInventarizatsii([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var inventarizatsii = _service.GetAll(search, sortBy).ToList();
                return Ok(inventarizatsii);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetInventarizatsiaById(int id)
        {
            try
            {
                var inventarizatsia = _service.GetById(id);
                if (inventarizatsia == null)
                {
                    return NotFound($"Инвентаризация с идентификатором {id} не найдена");
                }
                return Ok(inventarizatsia);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateInventarizatsia([FromBody] Inventarizatsia inventarizatsia)
        {
            try
            {
                if (string.IsNullOrEmpty(inventarizatsia.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(inventarizatsia);
                return Ok(inventarizatsia);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("start/{id}")]
        public IActionResult StartInventarizatsia(int id, [FromQuery] int userId)
        {
            try
            {
                _service.StartInventarizatsia(id, userId);
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("complete/{id}")]
        public IActionResult CompleteInventarizatsia(int id, [FromQuery] int userId)
        {
            try
            {
                _service.CompleteInventarizatsia(id, userId);
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}