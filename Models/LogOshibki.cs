using System;

namespace RestAPI.Models
{
    public class LogOshibki
    {
        public int Id { get; set; }
        public DateTime DataVremia { get; set; } = DateTime.Now;
        public int? PolzovatelId { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Soobshchenie { get; set; } = string.Empty;
        public string? StackTrace { get; set; }

        public Polzovatel? Polzovatel { get; set; }
    }
}