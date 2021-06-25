using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserLayer
{
    [Table("ImportLog")]
   
    public class ImportLogClass
    {
        [Key]
        public int Id { get; set; }
        public DateTime? DateImported { get; set; }
        public DateTime? DateImportFrom { get; set; }
        public DateTime? DateImportTo { get; set; }
        public string VesselName { get; set; }
        public string ImportedBy { get; set; }
        public string Filenames { get; set; }
    }
}
