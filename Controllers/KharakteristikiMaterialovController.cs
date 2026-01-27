using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KharakteristikiMaterialovController : ControllerBase
    {
        private readonly KharakteristikiMaterialovService _service;

        public KharakteristikiMaterialovController(KharakteristikiMaterialovService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public IActionResult GetKharakteristikiMaterialov([FromQuery] string? search, [FromQuery] string? sortBy)
        {
            try
            {
                var kharakteristiki = _service.GetAll(search, sortBy).ToList();
                return Ok(kharakteristiki);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("item/{id}")]
        public IActionResult GetKharakteristikaMaterialaById(int id)
        {
            try
            {
                var kharakteristika = _service.GetById(id);
                if (kharakteristika == null)
                {
                    return NotFound($"Характеристика материала с идентификатором {id} не найдена");
                }
                return Ok(kharakteristika);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateKharakteristikaMateriala([FromBody] KharakteristikaMateriala kharakteristika)
        {
            try
            {
                if (string.IsNullOrEmpty(kharakteristika.NazvanieKharakteristiki))
                {
                    return BadRequest("Название характеристики обязательно");
                }

                _service.Create(kharakteristika);
                return Ok(kharakteristika);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}