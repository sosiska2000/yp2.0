using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;
using System.Net;

namespace RestAPI.Services
{
    public class SetevyeNastroikiService
    {
        private readonly ApplicationDbContext _context;

        public SetevyeNastroikiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<SetevyeNastroiki> GetAll(string? search, string? sortBy)
        {
            var query = _context.SetevyeNastroiki.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.IpAdres.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "ip" => query.OrderBy(s => s.IpAdres),
                _ => query.OrderBy(s => s.Id)
            };
        }

        public SetevyeNastroiki? GetById(int id)
        {
            return _context.SetevyeNastroiki.Find(id);
        }

        public void Create(SetevyeNastroiki nastroiki)
        {
            _context.SetevyeNastroiki.Add(nastroiki);
            _context.SaveChanges();
        }

        public void Update(SetevyeNastroiki nastroiki)
        {
            _context.SetevyeNastroiki.Update(nastroiki);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var nastroiki = _context.SetevyeNastroiki.Find(id);
            if (nastroiki == null) return false;

            _context.SetevyeNastroiki.Remove(nastroiki);
            _context.SaveChanges();
            return true;
        }

        public bool IpExists(string ip)
        {
            return _context.SetevyeNastroiki.Any(s => s.IpAdres == ip);
        }

        public bool CheckNetworkDevice(string ip)
        {
            try
            {
         
                return true; 
            }
            catch
            {
                return false;
            }
        }
    }
}