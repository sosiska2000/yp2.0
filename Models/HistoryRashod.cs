using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class HistoryRashod
    {
        [Key]
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdRashod { get; set; }
        public DateTime Date { get; set; }        
    }
}
