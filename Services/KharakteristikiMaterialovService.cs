using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class KharakteristikiMaterialovService
    {
        private readonly ApplicationDbContext _context;

        public KharakteristikiMaterialovService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<KharakteristikaMateriala> GetAll(string? search, string? sortBy)
        {
            var query = _context.KharakteristikiMaterialov.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k => k.NazvanieKharakteristiki.Contains(search) ||
                                        k.Znachenie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(k => k.NazvanieKharakteristiki),
                "znachenie" => query.OrderBy(k => k.Znachenie),
                _ => query.OrderBy(k => k.Id)
            };
        }

        public KharakteristikaMateriala? GetById(int id)
        {
            return _context.KharakteristikiMaterialov.Find(id);
        }

        public void Create(KharakteristikaMateriala kharakteristika)
        {
            _context.KharakteristikiMaterialov.Add(kharakteristika);
            _context.SaveChanges();
        }

        public void Update(KharakteristikaMateriala kharakteristika)
        {
            _context.KharakteristikiMaterialov.Update(kharakteristika);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var kharakteristika = _context.KharakteristikiMaterialov.Find(id);
            if (kharakteristika == null) return false;

            _context.KharakteristikiMaterialov.Remove(kharakteristika);
            _context.SaveChanges();
            return true;
        }
    }
}
