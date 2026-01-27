using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaskhodnyeMaterialyController : ControllerBase
    {
        private readonly RaskhodnyeMaterialyService _service;

        public RaskhodnyeMaterialyController(RaskhodnyeMaterialyService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetRaskhodnyeMaterialy([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var materialy = _service.GetAll(search, sortBy).ToList();
                return Ok(materialy);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetRaskhodnyMaterialById(int id)
        {
            try
            {
                var material = _service.GetById(id);
                if (material == null)
                {
                    return NotFound($"Расходный материал с идентификатором {id} не найден");
                }
                return Ok(material);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateRaskhodnyMaterial([FromBody] RaskhodnyMaterial material)
        {
            try
            {
                if (string.IsNullOrEmpty(material.Nazvanie))
                {
                    return BadRequest("Название обязательно");
                }

                _service.Create(material);
                return Ok(material);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteRaskhodnyMaterial(int id)
        {
            try
            {
                if (_service.HasConnections(id))
                {
                    return BadRequest("Невозможно удалить расходный материал, так как он связан с оборудованием");
                }

                var success = _service.Delete(id);
                if (!success)
                {
                    return NotFound($"Расходный материал с идентификатором {id} не найден");
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