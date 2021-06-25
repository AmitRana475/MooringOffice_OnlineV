using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VesselLayer
{
       [Table("TextEditors")]
      public class TextEditors
       {
              [Key]
              public int Id { get; set; }

              [AllowHtml]
              [Display(Name = "Message")]
              public string Message { get; set; }
       }
}
