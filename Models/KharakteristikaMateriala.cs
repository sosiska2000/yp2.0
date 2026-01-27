namespace RestAPI.Models
{
    public class KharakteristikaMateriala
    {
        public int Id { get; set; }
        public int RaskhodnyMaterialId { get; set; }
        public string NazvanieKharakteristiki { get; set; } = string.Empty;
        public string Znachenie { get; set; } = string.Empty;

        public RaskhodnyMaterial? RaskhodnyMaterial { get; set; }
    }
}