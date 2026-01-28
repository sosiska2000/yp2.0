using Microsoft.EntityFrameworkCore;
using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class ProgrammyService
    {
        private readonly ApplicationDbContext _context;

        public ProgrammyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Programma> GetAll(string? search, string? sortBy)
        {
            var query = _context.Programmy.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nazvanie.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(p => p.Nazvanie),
                "versia" => query.OrderBy(p => p.Versia),
                _ => query.OrderBy(p => p.Id)
            };
        }

        public Programma? GetById(int id)
        {
            return _context.Programmy.Find(id);
        }

        public void Create(Programma programma)
        {
            _context.Programmy.Add(programma);
            _context.SaveChanges();
        }

        public void Update(Programma programma)
        {
            _context.Programmy.Update(programma);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var programma = _context.Programmy.Find(id);
            if (programma == null) return false;

            _context.Programmy.Remove(programma);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
 
            return false;
        }
    }
}