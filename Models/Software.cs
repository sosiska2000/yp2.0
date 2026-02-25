namespace RestAPI.Models
{
    public class Software
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int? DeveloperId { get; set; }
    }
}