﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrewReportLayer
{
    [Table("CrewReport")]
    public class CreReportClass
    {
        [Key]
        public int Wid { get; set; }
        public int Vessel_ID { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public int ID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public decimal TotalHours { get; set; }
        public decimal RestHours { get; set; }
        public string Options { get; set; }
        public string Remarks { get; set; }
        public DateTime Dates { get; set; }
        public string hrs { get; set; }
        public string NonConfirmities { get; set; }
        public string Department { get; set; }
        public string DaysOfMonth { get; set; }
        public decimal Overtime { get; set; }
        public bool Opa { get; set; }
        public decimal RestHourAny24 { get; set; }
        public decimal RestHourAny7day { get; set; }
        public decimal NormalWH { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        public string col3 { get; set; }
        public string col4 { get; set; }
        public string col5 { get; set; }
        public string col6 { get; set; }
        public string col7 { get; set; }
        public string col8 { get; set; }
        public string col9 { get; set; }
        public string col10 { get; set; }
        public string col11 { get; set; }
        public string col12 { get; set; }
        public string col13 { get; set; }
        public string col14 { get; set; }
        public string col15 { get; set; }
        public string col16 { get; set; }
        public string col17 { get; set; }
        public string col18 { get; set; }
        public string col19 { get; set; }
        public string col20 { get; set; }
        public string col21 { get; set; }
        public string col22 { get; set; }
        public string col23 { get; set; }
        public string col24 { get; set; }
        public string col25 { get; set; }
        public string col26 { get; set; }
        public string col27 { get; set; }
        public string col28 { get; set; }
        public string col29 { get; set; }
        public string col30 { get; set; }
        public string col31 { get; set; }
        public string col32 { get; set; }
        public string col33 { get; set; }
        public string col34 { get; set; }
        public string col35 { get; set; }
        public string col36 { get; set; }
        public string col37 { get; set; }
        public string col38 { get; set; }
        public string col39 { get; set; }
        public string col40 { get; set; }
        public string col41 { get; set; }
        public string col42 { get; set; }
        public string col43 { get; set; }
        public string col44 { get; set; }
        public string col45 { get; set; }
        public string col46 { get; set; }
        public string col47 { get; set; }
        public string col48 { get; set; }
        public string col49 { get; set; }
        public string col50 { get; set; }
        public string col51 { get; set; }
        public string col52 { get; set; }
        public string col53 { get; set; }
        public string col54 { get; set; }
        public string col55 { get; set; }
        public string col56 { get; set; }
        public string col57 { get; set; }
        public string col58 { get; set; }
        public string col59 { get; set; }
        public string col60 { get; set; }
        public string col61 { get; set; }
        public string col62 { get; set; }
        public string col63 { get; set; }
        public string col64 { get; set; }
        public string col65 { get; set; }
        public string col66 { get; set; }
        public string col67 { get; set; }
        public string col68 { get; set; }
        public string col69 { get; set; }
        public string col70 { get; set; }
        public string col71 { get; set; }
        public string col72 { get; set; }
        public string col73 { get; set; }
        public string col74 { get; set; }
        public string col75 { get; set; }
        public string col76 { get; set; }
        public string col77 { get; set; }
        public string col78 { get; set; }
        public string col79 { get; set; }
        public string col80 { get; set; }
        public string col81 { get; set; }
        public string col82 { get; set; }
        public string col83 { get; set; }
        public string col84 { get; set; }
        public string col85 { get; set; }
        public string col86 { get; set; }
        public string col87 { get; set; }
        public string col88 { get; set; }
        public string col89 { get; set; }
        public string col90 { get; set; }
        public string col91 { get; set; }
        public string col92 { get; set; }
        public string col93 { get; set; }
        public string col94 { get; set; }
        public string col95 { get; set; }
        public string col96 { get; set; }
        public bool youngseafearer { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImportedBy { get; set; }
        public string FileNames { get; set; }
        



    }
}
