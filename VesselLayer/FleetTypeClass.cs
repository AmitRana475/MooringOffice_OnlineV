using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace VesselLayer
{
    [Table("tblFleetType")]
    [MetadataType(typeof(FleetTypeMetadata))]
    public class FleetTypeClass
    {
        [Key]
        public int Tid { get; set; }
      
        [Display(Name = "Fleet Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fleet Type is Required")]
        public string FleetType { get; set; }
        public string AddedBy { get; set; }

    }

    public class FleetTypeMetadata
    {
       
        [Remote("checkftype", "Vesseldetails", "setting", ErrorMessage = "Fleet Type Already in use", AdditionalFields = "initialProductT")]
        public string FleetType { get; set; }
    }
}
