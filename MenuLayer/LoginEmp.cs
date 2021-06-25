using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuLayer
{
    [Table("login")]
    public class LoginEmp
    {
        //[Key]
        //public int Id { get; set; }

        //[Display(Name = "Email")]
        //public string UserName { get; set; }

        //[DataType(DataType.Password)]
        //public string Password { get; set; }
        //public string Role { get; set; }
        //public int UserId { get; set; }
        [Key]
        public System.Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Designation { get; set; }
        public string UserRole { get; set; }
        public string Vessels { get; set; }
        public string FleetNames { get; set; }
        public string FleetTypes { get; set; }
        public System.DateTime CreatedDate { get; set; }

        [NotMapped]
        public string IpAddress { get; set; }

        [NotMapped]        public string HId { get; set; }        [NotMapped]        public string HPass { get; set; }
    }

}

    