using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace VesselLayer
{
    [Table("tblFleetName")]
    [MetadataType(typeof(FleetNameMetadata))]
    public class FleetNameClass
    {
        [Key]
        public int Fid { get; set; }

        [Display(Name ="Fleet Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fleet Name is Required")]
        public string FleetName { get; set; }

        public string AddedBy { get; set; }
    }
    public class FleetNameMetadata
    {
       

        [Remote("checkfname", "Vesseldetails", "setting", ErrorMessage = "Fleet Name Already in use", AdditionalFields = "initialProductF")]
        public string FleetName { get; set; }

       
    }
}
