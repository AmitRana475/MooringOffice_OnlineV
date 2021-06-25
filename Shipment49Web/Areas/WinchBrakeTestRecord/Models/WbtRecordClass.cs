using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.WinchBrakeTestRecord.Models
{
    public class WbtRecordClass
    {
        public int Id { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }

        public string AttachmentDate { get; set; }
        public int VesselID { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
        public List<WbtRecordClass> WbtList { get; set; }
    }
}