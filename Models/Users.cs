using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FIO { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }        
        public string Number { get; set; }
        public string Address { get; set; }
    }
}
