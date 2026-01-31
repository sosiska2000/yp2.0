using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class RasxodMaterials
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DatePostupleniya { get; set; }
        public byte[]? Photo { get; set; }
        public double Quantity { get; set; }
        public int UserRespon { get; set; }
        public int ResponUserTime { get; set; }
        public int CharacteristicsType { get; set; }
        public int Characteristics { get; set; }
        public int IdValue { get; set; }
    }
}
