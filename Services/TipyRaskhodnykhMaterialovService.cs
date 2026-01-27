using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class TipyRaskhodnykhMaterialovService
    {
        private readonly ApplicationDbContext _context;

        public TipyRaskhodnykhMaterialovService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<TipRaskhodnogoMateriala> GetAll(string? search)
        {
            var query = _context.TipyRaskhodnykhMaterialov.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Nazvanie.Contains(search));
            }

            return query;
        }

        public TipRaskhodnogoMateriala? GetById(int id)
        {
            return _context.TipyRaskhodnykhMaterialov.Find(id);
        }

        public void Create(TipRaskhodnogoMateriala tip)
        {
            _context.TipyRaskhodnykhMaterialov.Add(tip);
            _context.SaveChanges();
        }

        public void Update(TipRaskhodnogoMateriala tip)
        {
            _context.TipyRaskhodnykhMaterialov.Update(tip);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var tip = _context.TipyRaskhodnykhMaterialov.Find(id);
            if (tip == null) return false;

            _context.TipyRaskhodnykhMaterialov.Remove(tip);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.RaskhodnyeMaterialy.Any(r => r.TipRaskhodnogoMaterialaId == id);
        }
    }
}
