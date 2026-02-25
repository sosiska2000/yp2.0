using EquipmentManagement.Client.Services.Api;

public class ApiService
{
    public AuditoriiApiClient Auditorii { get; }
    public OborudovanieApiClient Oborudovanie { get; }  // Это свойство должно быть
    public PolzovateliApiClient Polzovateli { get; }
    public DokumentyApiClient Dokumenty { get; }
    public SetevyeNastroikiApiClient SetevyeNastroiki { get; }

    public ApiService(string baseUrl)
    {
        Auditorii = new AuditoriiApiClient(baseUrl);
        Oborudovanie = new OborudovanieApiClient(baseUrl);  // Инициализация
        Polzovateli = new PolzovateliApiClient(baseUrl);
        Dokumenty = new DokumentyApiClient(baseUrl);
        SetevyeNastroiki = new SetevyeNastroikiApiClient(baseUrl);
    }
}