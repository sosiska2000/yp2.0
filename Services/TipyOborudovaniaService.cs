using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class TipyOborudovaniaService
    {
        private readonly ApplicationDbContext _context;

        public TipyOborudovaniaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<TipOborudovania> GetAll(string? search)
        {
            var query = _context.TipyOborudovania.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Nazvanie.Contains(search));
            }

            return query;
        }

        public TipOborudovania? GetById(int id)
        {
            return _context.TipyOborudovania.Find(id);
        }

        public void Create(TipOborudovania tip)
        {
            _context.TipyOborudovania.Add(tip);
            _context.SaveChanges();
        }

        public void Update(TipOborudovania tip)
        {
            _context.TipyOborudovania.Update(tip);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var tip = _context.TipyOborudovania.Find(id);
            if (tip == null) return false;

            _context.TipyOborudovania.Remove(tip);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Oborudovanie.Any(o => o.TipOborudovaniaId == id) ||
                   _context.VidyModelei.Any(v => v.TipOborudovaniaId == id);
        }
    }
}