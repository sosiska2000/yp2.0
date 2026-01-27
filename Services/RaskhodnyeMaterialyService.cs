using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class RaskhodnyeMaterialyService
    {
        private readonly ApplicationDbContext _context;

        public RaskhodnyeMaterialyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<RaskhodnyMaterial> GetAll(string? search, string? sortBy)
        {
            var query = _context.RaskhodnyeMaterialy.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(r => r.Nazvanie),
                "kolichestvo" => query.OrderBy(r => r.Kolichestvo),
                "data" => query.OrderBy(r => r.DataPostuplenia),
                _ => query.OrderBy(r => r.Id)
            };
        }

        public RaskhodnyMaterial? GetById(int id)
        {
            return _context.RaskhodnyeMaterialy.Find(id);
        }

        public void Create(RaskhodnyMaterial material)
        {
            _context.RaskhodnyeMaterialy.Add(material);
            _context.SaveChanges();
        }

        public void Update(RaskhodnyMaterial material)
        {
            _context.RaskhodnyeMaterialy.Update(material);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var material = _context.RaskhodnyeMaterialy.Find(id);
            if (material == null) return false;

            _context.RaskhodnyeMaterialy.Remove(material);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.OborudovanieRaskhodnyeMaterialy.Any(orm => orm.RaskhodnyMaterialId == id) ||
                   _context.KharakteristikiMaterialov.Any(k => k.RaskhodnyMaterialId == id);
        }
    }
}