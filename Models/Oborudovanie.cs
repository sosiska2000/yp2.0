namespace RestAPI.Models;

public class Oborudovanie
{
    public int Id { get; set; }
    public string Nazvanie { get; set; } = string.Empty;
    public string? Fotografia { get; set; } 
    public string InventarnyiNomer { get; set; } = string.Empty;

    public int? AuditoriaId { get; set; }
    public int? OtvetstvennyiPolzovatelId { get; set; }
    public int? VremennoOtvetstvennyiPolzovatelId { get; set; }

    public decimal Stoimost { get; set; }

    public int? NapravlenieId { get; set; }
    public int StatusId { get; set; }
    public int TipOborudovaniaId { get; set; }
    public int? VidModeliId { get; set; }

    public string? Kommentarii { get; set; }

    public Auditoria? Auditoria { get; set; }
    public Polzovatel? OtvetstvennyiPolzovatel { get; set; }
    public Polzovatel? VremennoOtvetstvennyiPolzovatel { get; set; }
    public Napravlenie? Napravlenie { get; set; }
    public Status? Status { get; set; }
    public TipOborudovania? TipOborudovania { get; set; }
    public VidModeli? VidModeli { get; set; }


    public ICollection<Programma>? Programmy { get; set; }
}