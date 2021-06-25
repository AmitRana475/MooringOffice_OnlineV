using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSMPmodule
{
    [Table("TblMenu")]
    public class Menus
    {
        [Key]
        public int MId { get; set; }

      
       
        public string MenuName { get; set; }

        public string Action { get; set; }
        public string Controller { get; set; }
        public string AreaName { get; set; }
        public string Role { get; set; }
      
       
      
    }
}
