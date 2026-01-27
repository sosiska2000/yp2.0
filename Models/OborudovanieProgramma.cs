namespace RestAPI.Models
{
    public class OborudovanieProgramma
    {
        public int OborudovanieId { get; set; }
        public int ProgrammaId { get; set; }
        public Oborudovanie? Oborudovanie { get; set; }
        public Programma? Programma { get; set; }
    }
}