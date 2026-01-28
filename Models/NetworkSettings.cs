using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Models
{
    public class NetworkSettings
    {
        [Key]
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string MainGateway { get; set; }
        public string DNSServer1 { get; set; }
        public string? DNSServer2 { get; set; }
        public string? DNSServer3 { get; set; }
        public string? DNSServer4 { get; set; }
        public int OborudovanieId { get; set; }
    }
}
