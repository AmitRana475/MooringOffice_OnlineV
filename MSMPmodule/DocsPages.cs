using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MSMPmodule
{
    public class DocsPages
    {
        [Key]
        public int Id { get; set; }

        //public int? DocsID { get; set; }

        public int Mid { get; set; }
        //public int Subid { get; set; }
        //public int SubSubid { get; set; }
        // public string PageTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreateBy { get; set; }
        public string ShipId { get; set; }
        public string ModifiedBy { get; set; }
        [AllowHtml]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}
