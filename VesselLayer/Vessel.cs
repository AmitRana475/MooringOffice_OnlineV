using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System;

namespace VesselLayer
{
    [Table("VesselDetail")]
    [MetadataType(typeof(VesselMetadata))]
    public partial class Vessel
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "Vessel Id")]
        [NotMapped]
        public int VesselID { get; set; }

        [Display(Name = "Vessel Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vessel name is required")]
        public string VesselName { get; set; }

        [Display(Name = "IMO Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "IMO number is required")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "IMO number must be numeric")]
        public int ImoNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Flag is required")]
        public string Flag { get; set; } = string.Empty;

        [Display(Name = "Fleet Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fleet name is required")]
        public int FleetNameID { get; set; }

        [Display(Name = "Fleet Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fleet Type is required")]
        public int FleetTypeID { get; set; }

        [Display(Name = "Trade Area")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Trade Area is required")]
        public int TradeAreaID { get; set; }

        public string FleetName { get; set; }

        public string FleetType { get; set; }

        public string TradeArea { get; set; }

        public DateTime DateBuilt { get; set; }

        [Display(Name = "Minimum Lines")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum Lines is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum Lines must be greater than zero.")]
        //[Range(0d, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public int MinimumRopes { get; set; }

        [Display(Name = "Minimum Rope-Tails")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum Rope-Tails is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum Rope-Tails must be greater than zero.")]
        public int MinimumRopeTails  { get; set; }
    }

    public class VesselMetadata
    {
        [Remote("checkimo", "VesselInfo", "CommonTypes", ErrorMessage = "IMO number already in use", AdditionalFields = "Id")]
        public int ImoNo { get; set; }

        [Remote("checkfnameVesselCreate", "VesselInfo", "CommonTypes", ErrorMessage = "Fleet Name not in database")]
        public string FleetName { get; set; }

        [Remote("checkftypeVesselCreate", "VesselInfo", "CommonTypes", ErrorMessage = "Fleet Type not in database")]
        public string FleetType { get; set; }
    }
}
