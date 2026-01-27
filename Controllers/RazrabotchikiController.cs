using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

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

        [HttpGet("list")]
        public IActionResult GetRazrabotchiki([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var razrabotchiki = _service.GetAll(search, sortBy).ToList();
                return Ok(razrabotchiki);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetRazrabotchikById(int id)
        {
            try
            {
                var razrabotchik = _service.GetById(id);
                if (razrabotchik == null)
                {
                    return NotFound($"Разработчик с идентификатором {id} не найден");
                }
                return Ok(razrabotchik);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateRazrabotchik([FromBody] Razrabotchik razrabotchik)
        {
            try
            {
                if (string.IsNullOrEmpty(razrabotchik.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(razrabotchik);
                return Ok(razrabotchik);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteRazrabotchik(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить разработчика, так как он связан с программами");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Разработчик с идентификатором {id} не найден");
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