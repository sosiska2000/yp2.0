using RestAPI.Connect;
using RestAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RestAPI.Services
{
    public class AuditoriiService
    {
        private readonly ApplicationDbContext _context;

        public AuditoriiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Auditoria> GetAll(string? search = null, string? sortBy = null)
        {
            var query = _context.Auditorii
                .Include(a => a.OtvetstvennyiPolzovatel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.Nazvanie.Contains(search) ||
                    (a.SokrashennoeNazvanie != null && a.SokrashennoeNazvanie.Contains(search))
                );
            }

 
            query = sortBy?.ToLower() switch
            {
                "nazvanie" => query.OrderBy(a => a.Nazvanie),
                "sokrashennoe_nazvanie" => query.OrderBy(a => a.SokrashennoeNazvanie),
                _ => query.OrderBy(a => a.Id)
            };

            return query;
        }

        public Auditoria? GetById(int id)
        {
            return _context.Auditorii
                .Include(a => a.OtvetstvennyiPolzovatel)
                .FirstOrDefault(a => a.Id == id);
        }

        public Auditoria Create(Auditoria auditoria)
        {
 
            if (string.IsNullOrEmpty(auditoria.Nazvanie))
            {
                throw new ArgumentException("Название аудитории обязательно");
            }

            _context.Auditorii.Add(auditoria);
            _context.SaveChanges();
            return auditoria;
        }

        public Auditoria? Update(int id, Auditoria auditoria)
        {
            var existing = _context.Auditorii.Find(id);
            if (existing == null) return null;

            if (string.IsNullOrEmpty(auditoria.Nazvanie))
            {
                throw new ArgumentException("Название аудитории обязательно");
            }

            existing.Nazvanie = auditoria.Nazvanie;
            existing.SokrashennoeNazvanie = auditoria.SokrashennoeNazvanie;
            existing.OtvetstvennyiPolzovatelId = auditoria.OtvetstvennyiPolzovatelId;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var auditoria = _context.Auditorii.Find(id);
            if (auditoria == null) return false;

            _context.Auditorii.Remove(auditoria);
            _context.SaveChanges();
            return true;
        }

        public bool HasConnections(int id)
        {

            return _context.Oborudovanie.Any(o => o.AuditoriaId == id);
        }

        public int GetEquipmentCount(int auditoriaId)
        {
            return _context.Oborudovanie.Count(o => o.AuditoriaId == auditoriaId);
        }

        public List<Oborudovanie> GetEquipmentInAuditoria(int auditoriaId)
        {
            return _context.Oborudovanie
                .Where(o => o.AuditoriaId == auditoriaId)
                .Include(o => o.Status)
                .Include(o => o.TipOborudovania)
                .ToList();
        }
    }
}