using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class СharacteristicValue
    {
        [Key]
        public int Id { get; set; }
        public int IdRasxod { get; set; }
        public int IdCharacter { get; set; }
        public string Znachenie { get; set; }
    }
}
