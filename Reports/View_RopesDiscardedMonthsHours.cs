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
    
    public partial class View_RopesDiscardedMonthsHours
    {
        public int id { get; set; }
        public int RopeId { get; set; }
        public int VesselID { get; set; }
        public int MooringRopeType { get; set; }
        public int ManufacturerType { get; set; }
        public Nullable<int> MaximumRunningHours { get; set; }
        public Nullable<int> CurrentRunningHours { get; set; }
        public Nullable<int> BalanceHours { get; set; }
        public Nullable<int> MaximumMonthsAllowed { get; set; }
        public Nullable<int> CurrentRunningMonths { get; set; }
        public Nullable<int> BalanceMonths { get; set; }
    }
}
