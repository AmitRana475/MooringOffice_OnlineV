using System.ComponentModel.DataAnnotations;

namespace Shipment49Web.Models
{
    public class Passwords
    {
        [Key]
        public int UserId { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please enter correct email address")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [StringLength(255, MinimumLength = 6, ErrorMessage = "password must be minimum 6 characters.")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [StringLength(255, MinimumLength = 6, ErrorMessage = "password must be minimum 6 characters.")]
        [Required(ErrorMessage = "ConfirmPassword is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}