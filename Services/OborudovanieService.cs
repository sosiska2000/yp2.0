using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class OborudovanieService
    {
        private readonly ApplicationDbContext _context;

        public OborudovanieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Oborudovanie> GetAll(string? search, string? sortBy)
        {
            var query = _context.Oborudovanie.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(o => o.Nazvanie),
                "inventarnyi_nomer" => query.OrderBy(o => o.InventarnyiNomer),
                "stoimost" => query.OrderBy(o => o.Stoimost),
                _ => query.OrderBy(o => o.Id)
            };
        }

        public Oborudovanie? GetById(int id)
        {
            return _context.Oborudovanie.Find(id);
        }

        public void Create(Oborudovanie oborudovanie)
        {
            _context.Oborudovanie.Add(oborudovanie);
            _context.SaveChanges();
        }

        public void Update(Oborudovanie oborudovanie)
        {
            _context.Oborudovanie.Update(oborudovanie);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var oborudovanie = _context.Oborudovanie.Find(id);
            if (oborudovanie == null) return false;

            _context.Oborudovanie.Remove(oborudovanie);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.SetevyeNastroiki.Any(s => s.OborudovanieId == id) ||
                   _context.InventarizatsiaDetali.Any(i => i.OborudovanieId == id);
        }
    }
}