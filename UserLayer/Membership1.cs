using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserLayer
{
    [Table("Membership")]
    public class Membership1
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime CreateDate { get; set; }

        public string ConfirmationToken { get; set; }

        public bool IsConfirmed { get; set; }
    }



    public class SwichButton
    {
        public static string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public static string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
    }
}
