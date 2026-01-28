using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Connect;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDbController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestDbController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("tables")]
        public IActionResult CheckTables()
        {
            try
            {
                var tables = new
                {
                    Polzovateli = _context.Polzovateli.Count(),
                    Statusy = _context.Statusy.Count(),
                    Razrabotchiki = _context.Razrabotchiki.Count(),
                    Napravleniya = _context.Napravleniya.Count(),
                    TipyOborudovania = _context.TipyOborudovania.Count(),
                    VidyModelei = _context.VidyModelei.Count(),
                    Auditorii = _context.Auditorii.Count(),
                    Programmy = _context.Programmy.Count(),
                    Oborudovanie = _context.Oborudovanie.Count(),
                    SetevyeNastroiki = _context.SetevyeNastroiki.Count(),
                    TipyRaskhodnykhMaterialov = _context.TipyRaskhodnykhMaterialov.Count(),
                    RaskhodnyeMaterialy = _context.RaskhodnyeMaterialy.Count(),
                    KharakteristikiMaterialov = _context.KharakteristikiMaterialov.Count(),
                    Inventarizatsii = _context.Inventarizatsii.Count(),
                    InventarizatsiaDetali = _context.InventarizatsiaDetali.Count(),
                    LogiOshibok = _context.LogiOshibok.Count()
                };

                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка проверки таблиц: {ex.Message}");
            }
        }

        [HttpGet("test-inventarizatsia")]
        public IActionResult TestInventarizatsia()
        {
            try
            {
                var inventarizatsii = _context.Inventarizatsii
                    .Include(i => i.SozdalPolzovatel)
                    .Select(i => new
                    {
                        i.Id,
                        i.Nazvanie,
                        i.DataNachala,
                        Sozdal = i.SozdalPolzovatel.Familiia
                    })
                    .ToList();

                return Ok(inventarizatsii);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка Inventarizatsia: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
    }
}