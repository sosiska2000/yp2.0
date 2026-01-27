namespace RestAPI.Models;

public class Polzovatel
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Parol { get; set; } = string.Empty;
    public string Rol { get; set; } = "admin"; 
    public string? Email { get; set; }
    public string Familiia { get; set; } = string.Empty;
    public string Imia { get; set; } = string.Empty;
    public string? Otchestvo { get; set; }
    public string? Telefon { get; set; }
    public string? Adres { get; set; }
}