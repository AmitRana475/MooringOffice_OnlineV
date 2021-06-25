using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Models
{
    public class RevisionModel
    {
        public class MasterRevisionModel
        {
            public int Id { get; set; }
            [Required]
            public string MasterRevisionNo { get; set; }
            public string RevisionsIncluded { get; set; }
            public DateTime CreatedDate { get; set; }
            public List<SelectListItem> RevList { get; set; }
        }

        public class RevisionNoList
        {
            public int Id { get; set; }
            public bool IsChecked { get; set; }
      
          

        }

        public class RevisionModelList
        {
            public MasterRevisionModel masterrev { get; set; }
            public List<MasterRevisionModel> masterrevisionListing { get; set; }
            public List<SelectListItem> RevList { get; set; }
        }
    }
   
}