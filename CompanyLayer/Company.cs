using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CompanyLayer
{
    [Table("CompanyInfo")]
    public partial class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Display(Name ="Company Name")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Company Name is Required")]
        [StringLength(150, ErrorMessage = "Company Name Should Have Maximum Length 150 Characters")]
        public string CompanyName { get; set; }

        [Display(Name = "Established Year")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Established Year Should Have  4 Digits")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Established Year Should Not Contain Characters")]
        public string EstablishYear { get; set; }

        [Display(Name = "Total Employees ")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Total Employees Must be Digits")]
        [StringLength(50, ErrorMessage = "Total Employees Should Have Maximum Length 50 Digits")]
        public string TotalEmployee { get; set; }

        [Display(Name = "Contact Number ")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Contact Number Must be Digits")]
        [Required(AllowEmptyStrings =false, ErrorMessage = "Contact Number is Required")]
        [StringLength(50, ErrorMessage = "Contact Number Should Have Maximum Length 50 Digits")]
        public string contectNo { get; set; }

        [Display(Name ="Fax No")]
        [StringLength(50, ErrorMessage = "Fax No Should Have Maximum Length 50 Digits")]
        public string FaxNo { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Email is Required")]
        [StringLength(100, ErrorMessage = "Email Should Have Haximum Length 100 Characters")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}",ErrorMessage ="Please Enter The Correct Email")]
        public string EmailID { get; set; }

        [Display(Name = "Website")]
        [StringLength(100, ErrorMessage = "Website Should Have Maximum Length 100 Characters")]
        [RegularExpression(@"(http(s)?:\\)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?", ErrorMessage = "URL Format is Wrong!")]
        public string WebSite { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(AllowEmptyStrings =false, ErrorMessage = "Address is Required")]
        [StringLength(500, ErrorMessage = "Address Should Have Maximum Length 500 Characters")]
        public string Address { get; set; }

    }

    public class CompanyMetaData
    {
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [DataType(DataType.Url)]
        public string WebSite { get; set; }


    }
}
