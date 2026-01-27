using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VidyModeleiController : ControllerBase
    {
        private readonly VidyModeleiService _service;

        public VidyModeleiController(VidyModeleiService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetVidyModelei([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var vidy = _service.GetAll(search, sortBy).ToList();
                return Ok(vidy);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetVidModeliById(int id)
        {
            try
            {
                var vid = _service.GetById(id);
                if (vid == null)
                {
                    return NotFound($"Вид модели с идентификатором {id} не найден");
                }
                return Ok(vid);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateVidModeli([FromBody] VidModeli vid)
        {
            try
            {
                if (string.IsNullOrEmpty(vid.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(vid);
                return Ok(vid);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteVidModeli(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить вид модели, так как он связан с оборудованием");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Вид модели с идентификатором {id} не найден");
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