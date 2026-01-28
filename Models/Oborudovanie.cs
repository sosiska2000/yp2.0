using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI.Models
{
    public class Oborudovanie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nazvanie { get; set; } = string.Empty;

        public string? Fotografia { get; set; }

        [Required]
        public string InventarnyiNomer { get; set; } = string.Empty;

        public int? AuditoriaId { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Stoimost { get; set; }

        public int? NapravlenieId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int TipOborudovaniaId { get; set; }

        public int? VidModeliId { get; set; }

        public string? Kommentarii { get; set; }

        [ForeignKey("AuditoriaId")]
        public Auditoria? Auditoria { get; set; }

        [ForeignKey("OtvetstvennyiPolzovatelId")]
        public Polzovatel? OtvetstvennyiPolzovatel { get; set; }

        [ForeignKey("VremennoOtvetstvennyiPolzovatelId")]
        public Polzovatel? VremennoOtvetstvennyiPolzovatel { get; set; }

        [ForeignKey("NapravlenieId")]
        public Napravlenie? Napravlenie { get; set; }

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        [ForeignKey("TipOborudovaniaId")]
        public TipOborudovania? TipOborudovania { get; set; }

        [ForeignKey("VidModeliId")]
        public VidModeli? VidModeli { get; set; }
        public ICollection<Programma>? Programmy { get; set; }
        public ICollection<RaskhodnyMaterial>? RaskhodnyeMaterialy { get; set; }
    }
}