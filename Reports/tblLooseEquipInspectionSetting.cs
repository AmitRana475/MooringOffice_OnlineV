//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reports
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblLooseEquipInspectionSetting
    {
        public int Id { get; set; }
        public int EquipmentType { get; set; }
        public int InspectionFrequency { get; set; }
        public Nullable<int> MaximumRunningHours { get; set; }
        public Nullable<int> MaximumMonthsAllowed { get; set; }
    }
}