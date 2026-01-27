namespace RestAPI.Models;

public class VidModeli
{
    public int Id { get; set; }
    public string Nazvanie { get; set; } = string.Empty;
    public int TipOborudovaniaId { get; set; }
    public TipOborudovania? TipOborudovania { get; set; }
}