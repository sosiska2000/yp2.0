using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class VidyModeleiService
    {
        private readonly ApplicationDbContext _context;

        public VidyModeleiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<VidModeli> GetAll(string? search, string? sortBy)
        {
            var query = _context.VidyModelei.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(v => v.Nazvanie),
                _ => query.OrderBy(v => v.Id)
            };
        }

        public VidModeli? GetById(int id)
        {
            return _context.VidyModelei.Find(id);
        }

        public void Create(VidModeli model)
        {
            _context.VidyModelei.Add(model);
            _context.SaveChanges();
        }

        public void Update(VidModeli model)
        {
            _context.VidyModelei.Update(model);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var model = _context.VidyModelei.Find(id);
            if (model == null) return false;

            _context.VidyModelei.Remove(model);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Oborudovanie.Any(o => o.VidModeliId == id);
        }
    }
}
