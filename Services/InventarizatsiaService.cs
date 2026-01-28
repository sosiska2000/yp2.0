using RestAPI.Connect;
using RestAPI.Models;
using System;
using System.Linq;

namespace RestAPI.Services
{
    public class InventarizatsiaService
    {
        private readonly ApplicationDbContext _context;

        public InventarizatsiaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Inventarizatsia> GetAll(string? search, string? sortBy)
        {
            var query = _context.Inventarizatsii.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(i => i.Nazvanie),
                "data_nachala" => query.OrderBy(i => i.DataNachala),
                "data_okonchania" => query.OrderBy(i => i.DataOkonchania),
                _ => query.OrderBy(i => i.Id)
            };
        }

        public Inventarizatsia? GetById(int id)
        {
            return _context.Inventarizatsii.Find(id);
        }

        public void Create(Inventarizatsia inventarizatsia)
        {
            _context.Inventarizatsii.Add(inventarizatsia);
            _context.SaveChanges();
        }

        public void Update(Inventarizatsia inventarizatsia)
        {
            _context.Inventarizatsii.Update(inventarizatsia);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var inventarizatsia = _context.Inventarizatsii.Find(id);
            if (inventarizatsia == null) return false;

            _context.Inventarizatsii.Remove(inventarizatsia);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.InventarizatsiaDetali.Any(detal => detal.InventarizatsiaId == id);
        }

        public void StartInventarizatsia(int id, int userId)
        {
            var inventarizatsia = _context.Inventarizatsii.Find(id);
            if (inventarizatsia != null)
            {
                inventarizatsia.DataNachala = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void CompleteInventarizatsia(int id, int userId)
        {
            var inventarizatsia = _context.Inventarizatsii.Find(id);
            if (inventarizatsia != null)
            {
                inventarizatsia.DataOkonchania = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}