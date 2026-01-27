using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class StatusyService
    {
        private readonly ApplicationDbContext _context;

        public StatusyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Status> GetAll(string? search)
        {
            var query = _context.Statusy.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Nazvanie.Contains(search));
            }

            return query;
        }

        public Status? GetById(int id)
        {
            return _context.Statusy.Find(id);
        }

        public void Create(Status status)
        {
            _context.Statusy.Add(status);
            _context.SaveChanges();
        }

        public void Update(Status status)
        {
            _context.Statusy.Update(status);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var status = _context.Statusy.Find(id);
            if (status == null) return false;

            _context.Statusy.Remove(status);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Oborudovanie.Any(o => o.StatusId == id);
        }
    }
}