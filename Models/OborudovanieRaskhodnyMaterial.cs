namespace RestAPI.Models
{
    public class OborudovanieRaskhodnyMaterial
    {
        public int OborudovanieId { get; set; }
        public int RaskhodnyMaterialId { get; set; }
        public int Kolichestvo { get; set; } = 1;
        public Oborudovanie? Oborudovanie { get; set; }
        public RaskhodnyMaterial? RaskhodnyMaterial { get; set; }
    }
}