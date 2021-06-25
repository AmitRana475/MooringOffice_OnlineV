using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSMPmodule
{
      public class DocTable
       {
              [Key]
              public int Id { get; set; }

              public string DocsName { get; set; }

       }
}
