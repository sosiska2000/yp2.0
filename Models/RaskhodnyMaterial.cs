using System;

namespace RestAPI.Models
{
    public class RaskhodnyMaterial
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
        public string? Opisanie { get; set; }
        public DateTime DataPostuplenia { get; set; }
        public string? Izobrazhenie { get; set; }
        public int Kolichestvo { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int TipRaskhodnogoMaterialaId { get; set; }

        public Polzovatel? OtvetstvennyiPolzovatel { get; set; }
        public TipRaskhodnogoMateriala? TipRaskhodnogoMateriala { get; set; }
    }
}