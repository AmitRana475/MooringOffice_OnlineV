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
    [Table("Revision")]
    public class Revision
    {
        [Key]
        public int Id { get; set; }

        //[Display(Name = "DocsName")]
        //public string DocsName { get; set; }

        [Display(Name = "Revision Number")]
        public int? RNumber { get; set; }
        [Display(Name = "Revision Prefix")]
        public string RPrefix { get; set; }

        public DateTime? ReviseDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string CreateBy { get; set; }

        public string ApprovedBy { get; set; }
        public int MId { get; set; }
        // public int SubId { get; set; }
        // public int SubSubId { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public string ContentPath { get; set; }
        public string Status { get; set; }
        public string RevisionType { get; set; }
        // public int DocumentID { get; set; }
        public string RevisionText { get; set; }
    }
}