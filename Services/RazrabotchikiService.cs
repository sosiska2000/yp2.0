using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class RazrabotchikiService
    {
        private readonly ApplicationDbContext _context;

        public RazrabotchikiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Razrabotchik> GetAll(string? search, string? sortBy)
        {
            var query = _context.Razrabotchiki.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(r => r.Nazvanie),
                _ => query.OrderBy(r => r.Id)
            };
        }

        public Razrabotchik? GetById(int id)
        {
            return _context.Razrabotchiki.Find(id);
        }

        public void Create(Razrabotchik razrabotchik)
        {
            _context.Razrabotchiki.Add(razrabotchik);
            _context.SaveChanges();
        }

        public void Update(Razrabotchik razrabotchik)
        {
            _context.Razrabotchiki.Update(razrabotchik);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var razrabotchik = _context.Razrabotchiki.Find(id);
            if (razrabotchik == null) return false;

            _context.Razrabotchiki.Remove(razrabotchik);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Programmy.Any(p => p.RazrabotchikId == id);
        }
    }
}