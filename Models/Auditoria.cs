namespace RestAPI.Models;

public class Auditoria
{
    public int Id { get; set; }
    public string Nazvanie { get; set; } = string.Empty;
    public string? SokrashennoeNazvanie { get; set; }
    public int? OtvetstvennyiPolzovatelId { get; set; }
    public Polzovatel? OtvetstvennyiPolzovatel { get; set; }
    public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
    public Polzovatel? VremennoOtvetstvennyiPolzovatel { get; set; }
}