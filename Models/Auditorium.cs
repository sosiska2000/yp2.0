using Newtonsoft.Json;

namespace EquipmentManagement.Client.Models
{
    public class Auditorium
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazvanie")]
        public string Nazvanie { get; set; } = string.Empty;

        [JsonProperty("sokrashennoeNazvanie")]
        public string? SokrashennoeNazvanie { get; set; }

        [JsonProperty("otvetstvennyiPolzovatelId")]
        public int? OtvetstvennyiPolzovatelId { get; set; }

        [JsonProperty("vremennoOtvetstvennyiPolzovatelId")]
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }

        // Навигационные свойства (могут не приходить из API)
        [JsonProperty("otvetstvennyiPolzovatel")]
        public User? OtvetstvennyiPolzovatel { get; set; }

        [JsonProperty("vremennoOtvetstvennyiPolzovatel")]
        public User? VremennoOtvetstvennyiPolzovatel { get; set; }
    }
}