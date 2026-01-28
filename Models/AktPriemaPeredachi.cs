
namespace RestAPI.Models
{
    public class AktPriemaPeredachi
    {
        public int Id { get; set; }
        public string TipAkta { get; set; } = "oborudovanie"; 
        public string? Gorod { get; set; } = "Пермь";
        public DateTime DataSostavleniya { get; set; } = DateTime.Now;
        public string Uchrezhdenie { get; set; } = "КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова";
        public int PoluchatelId { get; set; } // ID сотрудника-получателя
        public string? Prichina { get; set; } = "в целях обеспечением необходимым оборудованием для исполнения должностных обязанностей";
        public string? Kommentarii { get; set; }
        public DateTime? DataVozvrata { get; set; }
        public int SostavilId { get; set; } // ID составившего акт

        // Навигационные свойства
        public Polzovatel? Poluchatel { get; set; }
        public Polzovatel? Sostavil { get; set; }
        public ICollection<AktOborudovanie>? OborudovanieList { get; set; }
        public ICollection<AktMaterialy>? MaterialyList { get; set; }
    }

    public class AktOborudovanie
    {
        public int Id { get; set; }
        public int AktId { get; set; }
        public int OborudovanieId { get; set; }
        public int Kolichestvo { get; set; } = 1;

        public AktPriemaPeredachi? Akt { get; set; }
        public Oborudovanie? Oborudovanie { get; set; }
    }

    public class AktMaterialy
    {
        public int Id { get; set; }
        public int AktId { get; set; }
        public int MaterialId { get; set; }
        public int Kolichestvo { get; set; } = 1;
        public decimal? StoimostEdinicy { get; set; }

        public AktPriemaPeredachi? Akt { get; set; }
        public RaskhodnyMaterial? Material { get; set; }
    }
}