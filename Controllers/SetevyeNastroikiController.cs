using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetevyeNastroikiController : ControllerBase
    {
        private readonly SetevyeNastroikiService _service;

        public SetevyeNastroikiController(SetevyeNastroikiService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetSetevyeNastroiki([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var nastroiki = _service.GetAll(search, sortBy).ToList();
                return Ok(nastroiki);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetSetevyeNastroikiById(int id)
        {
            try
            {
                var nastroiki = _service.GetById(id);
                if (nastroiki == null)
                {
                    return NotFound($"Сетевые настройки с идентификатором {id} не найдены");
                }
                return Ok(nastroiki);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateSetevyeNastroiki([FromBody] SetevyeNastroiki nastroiki)
        {
            try
            {
                if (string.IsNullOrEmpty(nastroiki.IpAdres))
                {
                    return BadRequest("IP адрес обязателен");
                }

                if (_service.IpExists(nastroiki.IpAdres))
                {
                    return BadRequest("IP адрес уже существует");
                }

                _service.Create(nastroiki);
                return Ok(nastroiki);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("check-network")]
        public IActionResult CheckNetworkDevice([FromBody] string ip)
        {
            try
            {
                var isAvailable = _service.CheckNetworkDevice(ip);
                return Ok(new { ip, isAvailable });
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}

