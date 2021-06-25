using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Compare = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace UserLayer
{
    [Table("UserDetail")]
    [MetadataType(typeof(UserMetadata))]
    public partial class UserClass
    {
        
        
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassowrd { get; set; }
        public string ContactNo { get; set; }
        public string Designation { get; set; }
        public string EmailId { get; set; }
        public string Role { get; set; }
        public string VesselName { get; set; }
        public string VesselID { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string AssignVal { get; set; }
        [NotMapped]
        public string FullName1
        {
            get { return FullName + "(" + EmailId + ")"; }
        }

      

    }

    public class UserMetadata
    {
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is Required")]
        [StringLength(150, ErrorMessage = "Name Should Have Maximum Length 150 Characters")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email  is Required")]
        [StringLength(100, ErrorMessage = "Email Should Have Maximum Length 100 Characters")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Remote("checkuser", "userdetails", "setting", ErrorMessage = "Email already in use", AdditionalFields = "initialProductCode")]
        public string EmailId { get; set; }

        [Display(Name = "Phone")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contact Number is Required")]
        [StringLength(50, ErrorMessage = "Contact Number Should Have Maximum Length 50 Digits")]
        public string ContactNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Designation is Required")]
        [StringLength(100, ErrorMessage = "Designation Should Have Maximum Length 100 Characters")]
        public string Designation { get; set; }
        
        [Display(Name = "Assigning Vessel")]
        public string VesselName { get; set; }
        
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(100,MinimumLength = 6, ErrorMessage = "Password Should have Minimumlength 6 Characters")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is Required")]
        [Compare("Password", ErrorMessage = "Password Doesn't Match")]
        public string ConfirmPassowrd { get; set; }
    }
}
