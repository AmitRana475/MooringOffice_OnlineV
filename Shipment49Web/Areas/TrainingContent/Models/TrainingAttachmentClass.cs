using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.TrainingContent.Models
{
    public class TrainingAttachmentClass
    {
        public int Id { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }

        public string AttachmentDate { get; set; }
        public int VesselID { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
        public List<TrainingAttachmentClass> TrainingContentList { get; set; }
    }
}