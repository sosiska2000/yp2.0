using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;

namespace RestAPI.Services
{
    public class PolzovateliService
    {
        private readonly ApplicationDbContext _context;

        public PolzovateliService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Polzovatel> GetAll(string? search)
        {
            var query = _context.Polzovateli.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Familiia.Contains(search) ||
                                         p.Login.Contains(search) ||
                                         p.Email.Contains(search));
            }

            return query;
        }

        public Polzovatel? GetById(int id)
        {
            return _context.Polzovateli.Find(id);
        }

        public void Create(Polzovatel user)
        {
            _context.Polzovateli.Add(user);
            _context.SaveChanges();
        }

        public void Update(Polzovatel user)
        {
            _context.Polzovateli.Update(user);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var user = _context.Polzovateli.Find(id);
            if (user == null) return false;

            _context.Polzovateli.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {
            return _context.Oborudovanie.Any(o => o.OtvetstvennyiPolzovatelId == id) ||
                   _context.Auditorii.Any(a => a.OtvetstvennyiPolzovatelId == id);
        }

        public Polzovatel? Authenticate(string login, string parol)
        {
            return _context.Polzovateli.FirstOrDefault(p => p.Login == login && p.Parol == parol);
        }
    }
}