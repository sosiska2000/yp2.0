using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Connect;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetevyeNastroikiController : ControllerBase
    {
        private readonly SetevyeNastroikiService _service;
        private readonly ApplicationDbContext _context;

        public SetevyeNastroikiController(SetevyeNastroikiService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
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

        [HttpPost("check-all-network")]
        public IActionResult CheckAllNetworkDevices()
        {
            try
            {
                var results = _service.CheckAllNetworkDevices();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpPost("check-network-advanced")]
        public IActionResult CheckNetworkDeviceAdvanced([FromBody] NetworkCheckRequest request)
        {
            try
            {
                var result = _service.CheckNetworkDevice(request.IpAddress, request.Port, request.Timeout);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("network-statistics")]
        public IActionResult GetNetworkStatistics()
        {
            try
            {
                var devices = _context.SetevyeNastroiki.Count();
                var results = _service.CheckAllNetworkDevices();

                var online = results.Count(r => r.IsOnline);
                var offline = results.Count(r => !r.IsOnline);
                var avgResponseTime = results
                    .Where(r => r.ResponseTime.HasValue)
                    .Average(r => r.ResponseTime) ?? 0;

                return Ok(new
                {
                    TotalDevices = devices,
                    Online = online,
                    Offline = offline,
                    AvailabilityPercentage = devices > 0 ? (online * 100 / devices) : 0,
                    AverageResponseTime = avgResponseTime,
                    LastChecked = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateSetevyeNastroiki(int id, [FromBody] SetevyeNastroiki nastroiki)
        {
            try
            {
                var existing = _context.SetevyeNastroiki.Find(id);
                if (existing == null)
                    return NotFound($"Сетевые настройки с ID {id} не найдены");

                // Валидация IP адреса
                if (!_service.IsValidIPAddress(nastroiki.IpAdres))
                    return BadRequest("Некорректный IP адрес");

                if (!_service.IsValidIPAddress(nastroiki.MaskaPodseti))
                    return BadRequest("Некорректная маска подсети");

                if (!string.IsNullOrEmpty(nastroiki.GlavnyiShliuz) && !_service.IsValidIPAddress(nastroiki.GlavnyiShliuz))
                    return BadRequest("Некорректный главный шлюз");

                if (!string.IsNullOrEmpty(nastroiki.Dns1) && !_service.IsValidIPAddress(nastroiki.Dns1))
                    return BadRequest("Некорректный DNS сервер 1");

                if (!string.IsNullOrEmpty(nastroiki.Dns2) && !_service.IsValidIPAddress(nastroiki.Dns2))
                    return BadRequest("Некорректный DNS сервер 2");

                // Проверяем уникальность IP (если изменился)
                if (existing.IpAdres != nastroiki.IpAdres && _service.IpExists(nastroiki.IpAdres))
                    return BadRequest("IP адрес уже существует");

                existing.IpAdres = nastroiki.IpAdres;
                existing.MaskaPodseti = nastroiki.MaskaPodseti;
                existing.GlavnyiShliuz = nastroiki.GlavnyiShliuz;
                existing.Dns1 = nastroiki.Dns1;
                existing.Dns2 = nastroiki.Dns2;

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteSetevyeNastroiki(int id)
        {
            try
            {
                var nastroiki = _context.SetevyeNastroiki.Find(id);
                if (nastroiki == null)
                    return NotFound($"Сетевые настройки с ID {id} не найдены");

                _context.SetevyeNastroiki.Remove(nastroiki);
                _context.SaveChanges();

                return Ok(new { Message = "Сетевые настройки успешно удалены" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // Класс для запроса
        public class NetworkCheckRequest
        {
            public string IpAddress { get; set; } = string.Empty;
            public int Port { get; set; } = 80;
            public int Timeout { get; set; } = 2000;
        }
    }
}