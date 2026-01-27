using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipyRaskhodnykhMaterialovController : ControllerBase
    {
        private readonly TipyRaskhodnykhMaterialovService _service;

        public TipyRaskhodnykhMaterialovController(TipyRaskhodnykhMaterialovService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetTipyRaskhodnykhMaterialov([FromQuery] string? search)
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
        public IActionResult GetTipRaskhodnogoMaterialaById(int id)
        {
            try
            {
                var tip = _service.GetById(id);
                if (tip == null)
                {
                    return NotFound($"Тип расходного материала с идентификатором {id} не найден");
                }
                return Ok(tip);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateTipRaskhodnogoMateriala([FromBody] TipRaskhodnogoMateriala tip)
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
        public IActionResult DeleteTipRaskhodnogoMateriala(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить тип расходного материала, так как он связан с материалами");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Тип расходного материала с идентификатором {id} не найден");
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