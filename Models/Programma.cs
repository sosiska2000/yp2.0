namespace RestAPI.Models;

public class Programma
{
    public int Id { get; set; }
    public string Nazvanie { get; set; } = string.Empty;
    public int RazrabotchikId { get; set; }
    public string? Versia { get; set; }
    public Razrabotchik? Razrabotchik { get; set; }
}