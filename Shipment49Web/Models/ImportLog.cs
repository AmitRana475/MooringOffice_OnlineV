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
    
    public partial class ImportLog
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DateImported { get; set; }
        public Nullable<System.DateTime> DateImportFrom { get; set; }
        public Nullable<System.DateTime> DateImportTo { get; set; }
        public string VesselName { get; set; }
        public string ImportedBy { get; set; }
        public string Filenames { get; set; }
    }
}