using System;

namespace RestAPI.Models
{
    public class InventarizatsiaDetal
    {
        public int Id { get; set; }
        public int InventarizatsiaId { get; set; }
        public int OborudovanieId { get; set; }
        public bool Proinventarizirovano { get; set; } = false;
        public int? ProinventarizirovalPolzovatelId { get; set; }
        public DateTime? DataProverki { get; set; }
        public string? Kommentarii { get; set; }

        public Inventarizatsia? Inventarizatsia { get; set; }
        public Oborudovanie? Oborudovanie { get; set; }
        public Polzovatel? ProinventarizirovalPolzovatel { get; set; }
    }
}