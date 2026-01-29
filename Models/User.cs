using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; } = string.Empty;

        [JsonProperty("parol")]
        public string Parol { get; set; } = string.Empty;

        [JsonProperty("rol")]
        public string Rol { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("familiia")]
        public string Familiia { get; set; } = string.Empty;

        [JsonProperty("imia")]
        public string Imia { get; set; } = string.Empty;

        [JsonProperty("otchestvo")]
        public string? Otchestvo { get; set; }

        [JsonProperty("telefon")]
        public string? Telefon { get; set; }

        [JsonProperty("adres")]
        public string? Adres { get; set; }

        // Вычисляемое свойство для отображения
        [JsonIgnore]
        public string FullName => $"{Familiia} {Imia} {Otchestvo}".Trim();

        // Для ComboBox
        public override string ToString() => FullName;
    }
}