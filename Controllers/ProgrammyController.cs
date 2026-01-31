    using Microsoft.AspNetCore.Mvc;
    using RestAPI.Models;
    using RestAPI.Services;

    namespace RestAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ProgrammyController : ControllerBase
        {
            private readonly ProgrammyService _service;

            public ProgrammyController(ProgrammyService service)
            {
                _service = service;
            }

            [HttpGet("list")]
            public IActionResult GetProgrammy([FromQuery] string? search, [FromQuery] string? sortBy)
            {
                try
                {
                    var programmy = _service.GetAll(search, sortBy).ToList();
                    return Ok(programmy);
                }
                catch
                {
                    return StatusCode(500, "Внутренняя ошибка сервера");
                }
            }

            [HttpGet("item/{id}")]
            public IActionResult GetProgrammaById(int id)
            {
                try
                {
                    var programma = _service.GetById(id);
                    if (programma == null)
                    {
                        return NotFound($"Программа с идентификатором {id} не найдена");
                    }
                    return Ok(programma);
                }
                catch
                {
                    return StatusCode(500, "Внутренняя ошибка сервера");
                }
            }

            [HttpPost("create")]
            public IActionResult CreateProgramma([FromBody] Programma programma)
            {
                try
                {
                    if (string.IsNullOrEmpty(programma.Nazvanie))
                    {
                        return BadRequest("Название обязательно");
                    }

                    _service.Create(programma);
                    return Ok(programma);
                }
                catch
                {
                    return StatusCode(500, "Внутренняя ошибка сервера");
                }
            }

            [HttpDelete("delete/{id}")]
            public IActionResult DeleteProgramma(int id)
            {
                try
                {
                    if (_service.HasConnections(id))
                    {
                        return BadRequest("Невозможно удалить программу, так как она связана с оборудованием");
                    }

                    var success = _service.Delete(id);
                    if (!success)
                    {
                        return NotFound($"Программа с идентификатором {id} не найдена");
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
