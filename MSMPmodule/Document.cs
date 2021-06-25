using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSMPmodule
{
    [Table("Document")]
   public class Document
    {
        [Key]
        public int DocumentID { get; set; }
        [Required]
        public string DocumentName { get; set; }
        public DateTime CreatedDate { get; set; }

        //public List<System.Web.Mvc.SelectListItem> DocumentList { get; set; }

    }
}
