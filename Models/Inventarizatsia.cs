namespace RestAPI.Models
{
    public class Inventarizatsia
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty; 
        public DateTime DataNachala { get; set; }
        public DateTime? DataOkonchania { get; set; }
        public int SozdalPolzovatelId { get; set; }

        public Polzovatel? SozdalPolzovatel { get; set; }
    }
}