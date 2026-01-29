using Newtonsoft.Json;

namespace EquipmentManagement.Client.Models
{
    public class Developer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazvanie")]
        public string Nazvanie { get; set; } = string.Empty;

        // Для DataGrid/ComboBox
        public override string ToString() => Nazvanie;
    }
}