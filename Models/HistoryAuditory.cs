using System.ComponentModel.DataAnnotations;

namespace EquipmentManagement.Client.Models
{
    public class HistoryAuditory
    {
        [Key]
        public int Id { get; set; }
        public int IdClassroom { get; set; }
        public int IdObor { get; set; }
        public DateTime Date { get; set; }
    }
}
