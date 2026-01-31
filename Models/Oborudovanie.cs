using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class Oborudovanie
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[]? Photo { get; set; }
        public string InventNumber { get; set; }
        public int IdClassroom { get; set; }
        public int IdResponUser { get; set; }
        public int IdTimeResponUser { get; set; }
        public string PriceObor { get; set; }
        public int IdNapravObor { get; set; }
        public int IdStatusObor { get; set; }
        public int IdModelObor { get; set; }
        public string Comments { get; set; }
    }
}
