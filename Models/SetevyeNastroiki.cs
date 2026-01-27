namespace RestAPI.Models;

public class SetevyeNastroiki
{
    public int Id { get; set; }
    public int OborudovanieId { get; set; }
    public string IpAdres { get; set; } = string.Empty;
    public string MaskaPodseti { get; set; } = string.Empty;
    public string? GlavnyiShliuz { get; set; }
    public string? Dns1 { get; set; }
    public string? Dns2 { get; set; }

    public Oborudovanie? Oborudovanie { get; set; }
}