using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertificationLayer
{
    [Table("Certicationcomment")]
    public class CommentCirtificate
    {
        [Key]
        public int Cid { get; set; }
        public int Nid { get; set; }
        public string Comment { get; set; }
        public string CommentBy { get; set; }
        public DateTime? CDate { get; set; }
    }
}
