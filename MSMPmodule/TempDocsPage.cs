using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MSMPmodule
{
    [Table("tempDocsPage")]
    public class TempDocsPage
    {
        [Key]
        public int Id { get; set; }

        
       // public int DocsId { get; set; }
        public int Mid { get; set; }
        public int Subid { get; set; }
        public int SubSubid { get; set; }


       // public string PageTitle { get; set; }
        [AllowHtml]
        public string Content { get; set; }

        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ApprovedDate { get; set; }
       // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Status { get; set; }

    }
}
