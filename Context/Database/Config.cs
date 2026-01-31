using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Context.Database
{
    public class Config
    {
        public static readonly string connection = "server=localhost;port=3306;database=YP02;uid=root;pwd=";
        //public static readonly string connection = "server=sql.freedb.tech;port=3306;database=freedb_ychp02;uid=freedb_qweqq;pwd=3#rtDPD%3Ggm9N&";
        public static readonly MySqlServerVersion version = new MySqlServerVersion(new Version(8, 0, 11));
    }
}
