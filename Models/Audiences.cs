using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class Audiences
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int ResponUser { get; set; }
        public int TimeResponUser { get; set; }

    }
}
