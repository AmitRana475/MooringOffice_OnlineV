//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shipment49Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vRopesEndToEnd
    {
        public int RopeId { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<System.DateTime> EndtoEndDoneDate { get; set; }
        public Nullable<bool> CurrentOutboadEndinUse { get; set; }
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public Nullable<int> RopeTail { get; set; }
    }
}