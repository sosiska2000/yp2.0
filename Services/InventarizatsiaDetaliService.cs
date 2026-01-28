using RestAPI.Connect;
using RestAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestAPI.Services
{
    public class InventarizatsiaDetaliService
    {
        private readonly ApplicationDbContext _context;

        public InventarizatsiaDetaliService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<InventarizatsiaDetal> GetAll(int? inventarizatsiaId = null, int? oborudovanieId = null,
            bool? proinventarizirovano = null, string? sortBy = null)
        {
            var query = _context.InventarizatsiaDetali
                .Include(d => d.Inventarizatsia)
                .Include(d => d.Oborudovanie)
                .Include(d => d.ProinventarizirovalPolzovatel)
                .AsQueryable();


            if (inventarizatsiaId.HasValue)
            {
                query = query.Where(d => d.InventarizatsiaId == inventarizatsiaId.Value);
            }

            if (oborudovanieId.HasValue)
            {
                query = query.Where(d => d.OborudovanieId == oborudovanieId.Value);
            }

            if (proinventarizirovano.HasValue)
            {
                query = query.Where(d => d.Proinventarizirovano == proinventarizirovano.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "data_proverki" => query.OrderBy(d => d.DataProverki),
                "inventarizatsia" => query.OrderBy(d => d.InventarizatsiaId),
                "oborudovanie" => query.OrderBy(d => d.OborudovanieId),
                _ => query.OrderBy(d => d.Id)
            };

            return query;
        }

        public InventarizatsiaDetal? GetById(int id)
        {
            return _context.InventarizatsiaDetali
                .Include(d => d.Inventarizatsia)
                .Include(d => d.Oborudovanie)
                .Include(d => d.ProinventarizirovalPolzovatel)
                .FirstOrDefault(d => d.Id == id);
        }

        public InventarizatsiaDetal Create(InventarizatsiaDetal detal)
        {

            var inventarizatsia = _context.Inventarizatsii.Find(detal.InventarizatsiaId);
            if (inventarizatsia == null)
            {
                throw new ArgumentException("Инвентаризация не найдена");
            }

            var oborudovanie = _context.Oborudovanie.Find(detal.OborudovanieId);
            if (oborudovanie == null)
            {
                throw new ArgumentException("Оборудование не найдено");
            }

    
            var exists = _context.InventarizatsiaDetali
                .Any(d => d.InventarizatsiaId == detal.InventarizatsiaId &&
                         d.OborudovanieId == detal.OborudovanieId);

            if (exists)
            {
                throw new ArgumentException("Это оборудование уже добавлено в инвентаризацию");
            }

            _context.InventarizatsiaDetali.Add(detal);
            _context.SaveChanges();
            return detal;
        }

        public InventarizatsiaDetal? MarkAsChecked(int id, int userId, string? kommentarii = null)
        {
            var detal = _context.InventarizatsiaDetali.Find(id);
            if (detal == null) return null;

            detal.Proinventarizirovano = true;
            detal.ProinventarizirovalPolzovatelId = userId;
            detal.DataProverki = DateTime.Now;
            detal.Kommentarii = kommentarii;

            _context.SaveChanges();
            return detal;
        }

        public InventarizatsiaDetal? MarkAsUnchecked(int id)
        {
            var detal = _context.InventarizatsiaDetali.Find(id);
            if (detal == null) return null;

            detal.Proinventarizirovano = false;
            detal.ProinventarizirovalPolzovatelId = null;
            detal.DataProverki = null;

            _context.SaveChanges();
            return detal;
        }

        public bool Delete(int id)
        {
            var detal = _context.InventarizatsiaDetali.Find(id);
            if (detal == null) return false;

            _context.InventarizatsiaDetali.Remove(detal);
            _context.SaveChanges();
            return true;
        }

        public Stats GetStats(int inventarizatsiaId)
        {
            var detali = _context.InventarizatsiaDetali
                .Where(d => d.InventarizatsiaId == inventarizatsiaId)
                .ToList();

            var total = detali.Count;
            var checkedCount = detali.Count(d => d.Proinventarizirovano);
            var uncheckedCount = total - checkedCount;

            return new Stats
            {
                Total = total,
                Checked = checkedCount,
                Unchecked = uncheckedCount,
                Percentage = total > 0 ? (checkedCount * 100 / total) : 0
            };
        }

        public class Stats
        {
            public int Total { get; set; }
            public int Checked { get; set; }
            public int Unchecked { get; set; }
            public int Percentage { get; set; }
        }

        public List<InventarizatsiaDetal> GetByInventarizatsia(int inventarizatsiaId)
        {
            return _context.InventarizatsiaDetali
                .Where(d => d.InventarizatsiaId == inventarizatsiaId)
                .Include(d => d.Oborudovanie)
                .Include(d => d.ProinventarizirovalPolzovatel)
                .ToList();
        }
    }
}