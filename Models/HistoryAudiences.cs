using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class HistoryAudiences
    {
        [Key]
        public int Id { get; set; }
        public int IdClassroom { get; set; }
        public int IdObor { get; set; }
        public DateTime Date { get; set; }
    }
}
