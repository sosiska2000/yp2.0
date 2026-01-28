using RestAPI.Connect;
using RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RestAPI.Services
{
    public class ImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly LogiOshibokService _logService;

        public ImportService(ApplicationDbContext context, LogiOshibokService logService)
        {
            _context = context;
            _logService = logService;
        }

        public ImportResult ImportOborudovanieFromFile(Stream fileStream, string fileType)
        {
            var result = new ImportResult
            {
                StartTime = DateTime.Now,
                FileType = fileType
            };

            try
            {
                switch (fileType.ToLower())
                {
                    case "csv":
                        result = ImportOborudovanieFromCsv(fileStream);
                        break;

                    case "txt":
                        result = ImportOborudovanieFromText(fileStream);
                        break;

                    default:
                        result.Success = false;
                        result.Message = $"Неподдерживаемый формат файла: {fileType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ошибка импорта: {ex.Message}";
                result.Errors.Add(ex.Message);
                _logService.LogError("ImportService", $"Ошибка импорта: {ex.Message}", exception: ex);
            }

            return result;
        }

        private ImportResult ImportOborudovanieFromCsv(Stream fileStream)
        {
            var result = new ImportResult
            {
                FileType = "CSV"
            };

            try
            {
                using var reader = new StreamReader(fileStream);
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

    
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (lineNumber == 1) continue;

                    var fields = ParseCsvLine(line);

                    if (fields.Count >= 5)
                    {
                        try
                        {
                            var oborudovanie = new Oborudovanie
                            {
                                Nazvanie = fields[0].Trim(),
                                InventarnyiNomer = fields[1].Trim(),
                                Stoimost = ParseDecimal(fields[2].Trim()),
                                StatusId = ParseStatus(fields[3].Trim()),
                                TipOborudovaniaId = ParseTipOborudovania(fields[4].Trim()),
                                Kommentarii = fields.Count > 5 ? fields[5].Trim() : null
                            };

     
                            if (string.IsNullOrEmpty(oborudovanie.Nazvanie) ||
                                string.IsNullOrEmpty(oborudovanie.InventarnyiNomer))
                            {
                                result.Errors.Add($"Строка {lineNumber}: Отсутствуют обязательные поля");
                                continue;
                            }

                            if (_context.Oborudovanie.Any(o => o.InventarnyiNomer == oborudovanie.InventarnyiNomer))
                            {
                                result.Errors.Add($"Строка {lineNumber}: Инвентарный номер {oborudovanie.InventarnyiNomer} уже существует");
                                result.SkippedCount++;
                                continue;
                            }

                            if (!_context.Statusy.Any(s => s.Id == oborudovanie.StatusId))
                            {
                                oborudovanie.StatusId = 1; 
                                result.Warnings.Add($"Строка {lineNumber}: Статус не найден, установлен по умолчанию");
                            }


                            if (!_context.TipyOborudovania.Any(t => t.Id == oborudovanie.TipOborudovaniaId))
                            {
                                oborudovanie.TipOborudovaniaId = 1; 
                                result.Warnings.Add($"Строка {lineNumber}: Тип оборудования не найден, установлен по умолчанию");
                            }

                            _context.Oborudovanie.Add(oborudovanie);
                            result.ImportedCount++;
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Строка {lineNumber}: {ex.Message}");
                        }
                    }
                    else
                    {
                        result.Errors.Add($"Строка {lineNumber}: Недостаточно полей (ожидается минимум 5, получено {fields.Count})");
                    }
                }

                if (result.ImportedCount > 0)
                {
                    _context.SaveChanges();
                }

                result.EndTime = DateTime.Now;
                result.Success = result.ImportedCount > 0;
                result.Message = $"Импорт завершен. Успешно: {result.ImportedCount}, Пропущено: {result.SkippedCount}, Ошибок: {result.Errors.Count}";

                _logService.LogInfo("ImportService", result.Message);
            }
            catch (Exception ex)
            {
                result.EndTime = DateTime.Now;
                result.Success = false;
                result.Message = $"Ошибка импорта CSV: {ex.Message}";
                result.Errors.Add(ex.Message);

                _logService.LogError("ImportService", $"Ошибка импорта CSV: {ex.Message}", exception: ex);
            }

            return result;
        }

        private ImportResult ImportOborudovanieFromText(Stream fileStream)
        {
            var result = new ImportResult
            {
                FileType = "TXT"
            };

            try
            {
                using var reader = new StreamReader(fileStream);
                var content = reader.ReadToEnd();

       
                var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int lineNumber = 0;

                foreach (var line in lines)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (lineNumber == 1 && line.Contains("Название")) continue; 

                    var fields = line.Split('|');

                    if (fields.Length >= 3)
                    {
                        try
                        {
                            var oborudovanie = new Oborudovanie
                            {
                                Nazvanie = fields[0].Trim(),
                                InventarnyiNomer = fields[1].Trim(),
                                Stoimost = ParseDecimal(fields[2].Trim()),
                                StatusId = fields.Length > 3 ? ParseStatus(fields[3].Trim()) : 1,
                                TipOborudovaniaId = fields.Length > 4 ? ParseTipOborudovania(fields[4].Trim()) : 1,
                                Kommentarii = fields.Length > 5 ? fields[5].Trim() : null
                            };

                        
                            if (string.IsNullOrEmpty(oborudovanie.Nazvanie) ||
                                string.IsNullOrEmpty(oborudovanie.InventarnyiNomer))
                            {
                                result.Errors.Add($"Строка {lineNumber}: Отсутствуют обязательные поля");
                                continue;
                            }

                            if (_context.Oborudovanie.Any(o => o.InventarnyiNomer == oborudovanie.InventarnyiNomer))
                            {
                                result.Errors.Add($"Строка {lineNumber}: Инвентарный номер {oborudovanie.InventarnyiNomer} уже существует");
                                result.SkippedCount++;
                                continue;
                            }

                            _context.Oborudovanie.Add(oborudovanie);
                            result.ImportedCount++;
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Строка {lineNumber}: {ex.Message}");
                        }
                    }
                }

                if (result.ImportedCount > 0)
                {
                    _context.SaveChanges();
                }

                result.EndTime = DateTime.Now;
                result.Success = result.ImportedCount > 0;
                result.Message = $"Импорт TXT завершен. Успешно: {result.ImportedCount}";

                _logService.LogInfo("ImportService", result.Message);
            }
            catch (Exception ex)
            {
                result.EndTime = DateTime.Now;
                result.Success = false;
                result.Message = $"Ошибка импорта TXT: {ex.Message}";
                result.Errors.Add(ex.Message);

                _logService.LogError("ImportService", $"Ошибка импорта TXT: {ex.Message}", exception: ex);
            }

            return result;
        }

        private List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];

                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ';' && !inQuotes)
                {
                    result.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += ch;
                }
            }

            result.Add(currentField);
            return result;
        }

        private decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            if (decimal.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            throw new FormatException($"Невозможно преобразовать '{value}' в число");
        }

        private int ParseStatus(string statusName)
        {
            var status = _context.Statusy.FirstOrDefault(s =>
                s.Nazvanie.Equals(statusName, StringComparison.OrdinalIgnoreCase));

            if (status != null)
            {
                return status.Id;
            }

            if (int.TryParse(statusName, out int statusId))
            {
                if (_context.Statusy.Any(s => s.Id == statusId))
                {
                    return statusId;
                }
            }


            return 1;
        }

        private int ParseTipOborudovania(string tipName)
        {
            var tip = _context.TipyOborudovania.FirstOrDefault(t =>
                t.Nazvanie.Equals(tipName, StringComparison.OrdinalIgnoreCase));

            if (tip != null)
            {
                return tip.Id;
            }

            if (int.TryParse(tipName, out int tipId))
            {
                if (_context.TipyOborudovania.Any(t => t.Id == tipId))
                {
                    return tipId;
                }
            }

            return 1; 
        }

        public ImportResult TestImport(string testData)
        {
            var result = new ImportResult
            {
                StartTime = DateTime.Now,
                FileType = "TEST"
            };

            try
            {

                var testOborudovanie = new Oborudovanie
                {
                    Nazvanie = "Тестовое оборудование",
                    InventarnyiNomer = $"TEST-{DateTime.Now:yyyyMMddHHmmss}",
                    Stoimost = 1000.50m,
                    StatusId = 1,
                    TipOborudovaniaId = 1,
                    Kommentarii = "Импортировано для тестирования"
                };

                _context.Oborudovanie.Add(testOborudovanie);
                _context.SaveChanges();

                result.ImportedCount = 1;
                result.EndTime = DateTime.Now;
                result.Success = true;
                result.Message = "Тестовый импорт выполнен успешно";

                _logService.LogInfo("ImportService", result.Message);
            }
            catch (Exception ex)
            {
                result.EndTime = DateTime.Now;
                result.Success = false;
                result.Message = $"Ошибка тестового импорта: {ex.Message}";
                result.Errors.Add(ex.Message);

                _logService.LogError("ImportService", $"Ошибка тестового импорта: {ex.Message}", exception: ex);
            }

            return result;
        }

        public string GetTemplateCsv()
        {
            return "Название;Инвентарный номер;Стоимость;Статус;Тип оборудования;Комментарий\n" +
                   "Компьютер HP;INV-001;50000.00;Используется;Системный блок;Новый компьютер\n" +
                   "Монитор Dell;INV-002;15000.00;Используется;Монитор;Монитор 24 дюйма\n" +
                   "Принтер Canon;INV-003;8000.00;На ремонте;Принтер;Требуется заправка";
        }

        public List<string> GetSupportedFormats()
        {
            return new List<string>
            {
                "CSV (.csv) - разделитель точка с запятой",
                "TXT (.txt) - разделитель вертикальная черта (|)"
            };
        }

        public class ImportResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string FileType { get; set; } = string.Empty;
            public int ImportedCount { get; set; }
            public int SkippedCount { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public TimeSpan Duration => EndTime - StartTime;
            public List<string> Errors { get; set; } = new List<string>();
            public List<string> Warnings { get; set; } = new List<string>();
        }
    }
}