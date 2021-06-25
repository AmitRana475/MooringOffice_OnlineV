using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MSPS.Models
{
    public class ShipSpecificModel
    {
        public int Id { get; set; }
        public string AttachmentName { get; set; }
        public string Attachment { get; set; }
        public int MId { get; set; }
        public string ShipId { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string vesselname { get; set; }
    }
}