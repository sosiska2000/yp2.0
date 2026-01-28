//using Microsoft.AspNetCore.Mvc;
//using RestAPI.Services;

//namespace RestAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ImportController : ControllerBase
//    {
//        private readonly ImportService _importService;

//        public ImportController(ImportService importService)
//        {
//            _importService = importService;
//        }

//        [HttpPost("oborudovanie")]
//        public IActionResult ImportOborudovanie([FromForm] IFormFile file)
//        {
//            try
//            {
//                if (file == null || file.Length == 0)
//                {
//                    return BadRequest(new { message = "Файл не выбран" });
//                }

//                var fileExtension = Path.GetExtension(file.FileName).ToLower();
//                var contentType = file.ContentType;

//                // Определяем тип файла
//                string fileType;
//                if (fileExtension == ".csv" || contentType == "text/csv")
//                {
//                    fileType = "csv";
//                }
//                else if (fileExtension == ".txt" || contentType == "text/plain")
//                {
//                    fileType = "txt";
//                }
//                else
//                {
//                    return BadRequest(new
//                    {
//                        message = "Неподдерживаемый формат файла",
//                        supportedFormats = _importService.GetSupportedFormats()
//                    });
//                }

//                // Проверяем размер файла (максимум 10 MB)
//                if (file.Length > 10 * 1024 * 1024)
//                {
//                    return BadRequest(new { message = "Файл слишком большой (максимум 10 MB)" });
//                }

//                using var stream = file.OpenReadStream();
//                var result = _importService.ImportOborudovanieFromFile(stream, fileType);

//                if (result.Success)
//                {
//                    return Ok(new
//                    {
//                        success = true,
//                        message = result.Message,
//                        importedCount = result.ImportedCount,
//                        skippedCount = result.SkippedCount,
//                        duration = result.Duration.TotalSeconds,
//                        warnings = result.Warnings,
//                        errors = result.Errors
//                    });
//                }
//                else
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = result.Message,
//                        errors = result.Errors
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Внутренняя ошибка сервера при импорте",
//                    error = ex.Message
//                });
//            }
//        }

//        [HttpGet("template/csv")]
//        public IActionResult GetTemplateCsv()
//        {
//            try
//            {
//                var template = _importService.GetTemplateCsv();

//                // Возвращаем CSV как файл для скачивания
//                var bytes = System.Text.Encoding.UTF8.GetBytes(template);
//                return File(bytes, "text/csv", "template_oborudovanie.csv");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Ошибка получения шаблона", error = ex.Message });
//            }
//        }

//        [HttpGet("formats")]
//        public IActionResult GetSupportedFormats()
//        {
//            try
//            {
//                var formats = _importService.GetSupportedFormats();
//                return Ok(new { formats });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Ошибка получения форматов", error = ex.Message });
//            }
//        }

//        [HttpPost("test")]
//        public IActionResult TestImport()
//        {
//            try
//            {
//                var result = _importService.TestImport("test");

//                if (result.Success)
//                {
//                    return Ok(new
//                    {
//                        success = true,
//                        message = result.Message,
//                        importedCount = result.ImportedCount
//                    });
//                }
//                else
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = result.Message,
//                        errors = result.Errors
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Ошибка тестового импорта",
//                    error = ex.Message
//                });
//            }
//        }

//        [HttpPost("validate")]
//        public IActionResult ValidateFile([FromForm] IFormFile file)
//        {
//            try
//            {
//                if (file == null || file.Length == 0)
//                {
//                    return BadRequest(new { message = "Файл не выбран" });
//                }

//                var fileExtension = Path.GetExtension(file.FileName).ToLower();

//                if (fileExtension != ".csv" && fileExtension != ".txt")
//                {
//                    return BadRequest(new
//                    {
//                        message = "Неподдерживаемый формат файла",
//                        supportedFormats = new[] { ".csv", ".txt" }
//                    });
//                }

//                // Простая валидация по размеру
//                if (file.Length > 10 * 1024 * 1024)
//                {
//                    return BadRequest(new { message = "Файл слишком большой (максимум 10 MB)" });
//                }

//                // Читаем первые несколько строк для проверки
//                using var reader = new StreamReader(file.OpenReadStream());
//                var firstLine = reader.ReadLine();
//                var secondLine = reader.ReadLine();

//                return Ok(new
//                {
//                    success = true,
//                    message = "Файл валиден",
//                    fileName = file.FileName,
//                    fileSize = file.Length,
//                    firstLine,
//                    hasMoreData = !string.IsNullOrEmpty(secondLine)
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Ошибка валидации файла",
//                    error = ex.Message
//                });
//            }
//        }
//    }
//}