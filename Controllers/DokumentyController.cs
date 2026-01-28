
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DokumentyController : ControllerBase
    {
        private readonly DokumentyService _service;

        public DokumentyController(DokumentyService service)
        {
            _service = service;
        }

        // Получить все акты
        [HttpGet("list")]
        public IActionResult GetAkty([FromQuery] string? tipAkta,
            [FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            try
            {
                DateTime? sDate = null;
                DateTime? eDate = null;

                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var sd))
                    sDate = sd;

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var ed))
                    eDate = ed;

                var akty = _service.GetAll(tipAkta, sDate, eDate).ToList();
                return Ok(akty);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // Получить акт по ID
        [HttpGet("item/{id}")]
        public IActionResult GetAktById(int id)
        {
            try
            {
                var akt = _service.GetById(id);
                if (akt == null)
                {
                    return NotFound($"Акт с идентификатором {id} не найден");
                }
                return Ok(akt);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // Создать акт приема-передачи оборудования
        [HttpPost("create-akt-oborudovaniya")]
        public IActionResult CreateAktOborudovaniya([FromBody] CreateAktOborudovaniyaRequest request)
        {
            try
            {
                if (request.OborudovanieIds == null || !request.OborudovanieIds.Any())
                {
                    return BadRequest("Необходимо указать хотя бы одно оборудование");
                }

                var akt = _service.CreateAktOborudovanie(
                    request.PoluchatelId,
                    request.SostavilId,
                    request.OborudovanieIds,
                    request.Kommentarii,
                    request.DataVozvrata
                );

                return Ok(new
                {
                    message = "Акт успешно создан",
                    aktId = akt.Id,
                    text = _service.GenerateAktText(akt.Id)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // Создать акт приема-передачи материалов
        [HttpPost("create-akt-materialov")]
        public IActionResult CreateAktMaterialov([FromBody] CreateAktMaterialovRequest request)
        {
            try
            {
                if (request.Materialy == null || !request.Materialy.Any())
                {
                    return BadRequest("Необходимо указать хотя бы один материал");
                }

                var akt = _service.CreateAktMaterialy(
                    request.PoluchatelId,
                    request.SostavilId,
                    request.Materialy,
                    request.Kommentarii
                );

                return Ok(new
                {
                    message = "Акт успешно создан",
                    aktId = akt.Id,
                    text = _service.GenerateAktText(akt.Id)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // Получить текст акта для печати
        [HttpGet("generate-text/{id}")]
        public IActionResult GenerateAktText(int id)
        {
            try
            {
                var text = _service.GenerateAktText(id);
                return Content(text, "text/html");
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // Получить акты по пользователю (который получал)
        [HttpGet("by-poluchatel/{userId}")]
        public IActionResult GetAktyByPoluchatel(int userId, [FromQuery] string? tipAkta)
        {
            try
            {
                var query = _service.GetAll(tipAkta);
                var akty = query.Where(a => a.PoluchatelId == userId).ToList();
                return Ok(akty);
            }
            catch
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }

    // Классы для запросов
    public class CreateAktOborudovaniyaRequest
    {
        [Required]
        public int PoluchatelId { get; set; }

        [Required]
        public int SostavilId { get; set; }

        [Required]
        public List<int> OborudovanieIds { get; set; } = new List<int>();

        public string? Kommentarii { get; set; }

        public DateTime? DataVozvrata { get; set; }
    }

    public class CreateAktMaterialovRequest
    {
        [Required]
        public int PoluchatelId { get; set; }

        [Required]
        public int SostavilId { get; set; }

        [Required]
        public Dictionary<int, int> Materialy { get; set; } = new Dictionary<int, int>(); // materialId -> kolichestvo

        public string? Kommentarii { get; set; }
    }
}