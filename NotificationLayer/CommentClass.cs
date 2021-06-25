using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationLayer
{
    [Table("Comments")]
    public class CommentClass
    {
        [Key]
        public int Cid { get; set; }
        public int Nid { get; set; }
        public string Comment { get; set; }
        public string CommentBy { get; set; }
        public DateTime? CDate { get; set; }
    }
}
