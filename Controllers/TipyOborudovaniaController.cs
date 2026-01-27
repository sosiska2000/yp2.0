using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipyOborudovaniaController : ControllerBase
    {
        private readonly TipyOborudovaniaService _service;

        public TipyOborudovaniaController(TipyOborudovaniaService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetTipyOborudovania([FromQuery] string? search)
        {
            try
            {
                var tipy = _service.GetAll(search).ToList();
                return Ok(tipy);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetTipOborudovaniaById(int id)
        {
            try
            {
                var tip = _service.GetById(id);
                if (tip == null)
                {
                    return NotFound($"Тип оборудования с идентификатором {id} не найден");
                }
                return Ok(tip);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateTipOborudovania([FromBody] TipOborudovania tip)
        {
            try
            {
                if (string.IsNullOrEmpty(tip.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(tip);
                return Ok(tip);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTipOborudovania(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить тип оборудования, так как он связан с оборудованием");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Тип оборудования с идентификатором {id} не найден");
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

