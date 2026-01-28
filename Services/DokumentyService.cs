
using RestAPI.Connect;
using RestAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RestAPI.Services
{
    public class DokumentyService
    {
        private readonly ApplicationDbContext _context;

        public DokumentyService(ApplicationDbContext context)
        {
            _context = context;
        }


        public AktPriemaPeredachi CreateAktOborudovanie(int poluchatelId, int sostavilId,
            List<int> oborudovanieIds, string? kommentarii = null, DateTime? dataVozvrata = null)
        {
            var akt = new AktPriemaPeredachi
            {
                TipAkta = "oborudovanie",
                PoluchatelId = poluchatelId,
                SostavilId = sostavilId,
                DataSostavleniya = DateTime.Now,
                DataVozvrata = dataVozvrata,
                Kommentarii = kommentarii
            };

            _context.AktyPriemaPeredachi.Add(akt);
            _context.SaveChanges();

            // Добавляем оборудование
            foreach (var oborudovanieId in oborudovanieIds)
            {
                var aktOborudovanie = new AktOborudovanie
                {
                    AktId = akt.Id,
                    OborudovanieId = oborudovanieId,
                    Kolichestvo = 1
                };
                _context.AktyOborudovanie.Add(aktOborudovanie);
            }

            _context.SaveChanges();
            return akt;
        }

        // Создать акт приема-передачи материалов
        public AktPriemaPeredachi CreateAktMaterialy(int poluchatelId, int sostavilId,
            Dictionary<int, int> materialyKolichestvo, // materialId -> kolichestvo
            string? kommentarii = null)
        {
            var akt = new AktPriemaPeredachi
            {
                TipAkta = "materialy",
                PoluchatelId = poluchatelId,
                SostavilId = sostavilId,
                DataSostavleniya = DateTime.Now,
                Kommentarii = kommentarii
            };

            _context.AktyPriemaPeredachi.Add(akt);
            _context.SaveChanges();

            foreach (var kvp in materialyKolichestvo)
            {
                var aktMaterial = new AktMaterialy
                {
                    AktId = akt.Id,
                    MaterialId = kvp.Key,
                    Kolichestvo = kvp.Value
                };
                _context.AktyMaterialy.Add(aktMaterial);
            }

            _context.SaveChanges();
            return akt;
        }

        // Получить все акты
        public IQueryable<AktPriemaPeredachi> GetAll(string? tipAkta = null,
            DateTime? sDate = null, DateTime? poDate = null)
        {
            var query = _context.AktyPriemaPeredachi
                .Include(a => a.Poluchatel)
                .Include(a => a.Sostavil)
                .Include(a => a.OborudovanieList!)
                    .ThenInclude(ao => ao.Oborudovanie)
                .Include(a => a.MaterialyList!)
                    .ThenInclude(am => am.Material)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tipAkta))
                query = query.Where(a => a.TipAkta == tipAkta);

            if (sDate.HasValue)
                query = query.Where(a => a.DataSostavleniya >= sDate.Value);

            if (poDate.HasValue)
                query = query.Where(a => a.DataSostavleniya <= poDate.Value);

            return query.OrderByDescending(a => a.DataSostavleniya);
        }

        // Получить акт по ID
        public AktPriemaPeredachi? GetById(int id)
        {
            return _context.AktyPriemaPeredachi
                .Include(a => a.Poluchatel)
                .Include(a => a.Sostavil)
                .Include(a => a.OborudovanieList!)
                    .ThenInclude(ao => ao.Oborudovanie)
                .Include(a => a.MaterialyList!)
                    .ThenInclude(am => am.Material)
                .FirstOrDefault(a => a.Id == id);
        }

        // Генерация текста акта в формате HTML/PDF
        public string GenerateAktText(int aktId)
        {
            var akt = GetById(aktId);
            if (akt == null) return "Акт не найден";

            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"<h2>АКТ приема-передачи {(akt.TipAkta == "oborudovanie" ? "оборудования" : "расходных материалов")}</h2>");
            sb.AppendLine($"<p>г. {akt.Gorod}</p>");
            sb.AppendLine($"<p>{akt.DataSostavleniya:dd.MM.yyyy}</p>");
            sb.AppendLine($"<p>{akt.Uchrezhdenie} {akt.Prichina}</p>");
            sb.AppendLine($"<p>передаёт сотруднику {akt.Poluchatel?.Familiia} {akt.Poluchatel?.Imia} {akt.Poluchatel?.Otchestvo}, а сотрудник принимает от учебного учреждения следующее:</p>");

            if (akt.TipAkta == "oborudovanie" && akt.OborudovanieList != null)
            {
                sb.AppendLine("<ul>");
                foreach (var item in akt.OborudovanieList)
                {
                    var ob = item.Oborudovanie;
                    sb.AppendLine($"<li>{ob?.Nazvanie}, инвентарный номер: {ob?.InventarnyiNomer}, стоимостью {ob?.Stoimost} руб.</li>");
                }
                sb.AppendLine("</ul>");
            }
            else if (akt.TipAkta == "materialy" && akt.MaterialyList != null)
            {
                sb.AppendLine("<ul>");
                foreach (var item in akt.MaterialyList)
                {
                    var mat = item.Material;
                    sb.AppendLine($"<li>{mat?.Nazvanie}, в количестве {item.Kolichestvo} шт.</li>");
                }
                sb.AppendLine("</ul>");
            }

            if (akt.DataVozvrata.HasValue)
            {
                sb.AppendLine($"<p>По окончанию должностных работ {akt.DataVozvrata:dd.MM.yyyy} года, работник обязуется вернуть полученное оборудование.</p>");
            }

            sb.AppendLine($"<p>{akt.Poluchatel?.Familiia} {akt.Poluchatel?.Imia} {akt.Poluchatel?.Otchestvo}</p>");
            sb.AppendLine("<p>_________________________</p>");
            sb.AppendLine("<p>_____________</p>");

            return sb.ToString();
        }
    }
}