using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class NapravleniyaService
    {
        private readonly ApplicationDbContext _context;

        public NapravleniyaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Napravlenie> GetAll(string? search)
        {
            var query = _context.Napravleniya.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(n => n.Nazvanie.Contains(search));
            }

            return query;
        }

        public Napravlenie? GetById(int id)
        {
            return _context.Napravleniya.Find(id);
        }

        public void Create(Napravlenie napravlenie)
        {
            _context.Napravleniya.Add(napravlenie);
            _context.SaveChanges();
        }

        public void Update(Napravlenie napravlenie)
        {
            _context.Napravleniya.Update(napravlenie);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var napravlenie = _context.Napravleniya.Find(id);
            if (napravlenie == null) return false;

            _context.Napravleniya.Remove(napravlenie);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Oborudovanie.Any(o => o.NapravlenieId == id);
        }
    }
}