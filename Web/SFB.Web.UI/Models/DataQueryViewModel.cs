using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class DataQueryViewModel
    {
        [Required(ErrorMessage = "Enter a name to continue")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a valid email address to continue")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter a school or trust name to continue")]
        [StringLength(100, MinimumLength = 3)]
        public string SchoolTrustName { get; set; }

        [Required(ErrorMessage = "Enter a valid school or trust reference number to continue")]
        [RegularExpression("^[0-9]{6}$|^[0-9]{3}(-|/)?[0-9]{4}$|^[0-9]{8}$", ErrorMessage = "Please provide a URN, LAESTAB code or company number")]
        public string SchoolTrustReferenceNumber { get; set; }

        [Required(ErrorMessage = "Enter a data query to continue")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "The field Data query must be a string with a minimum length of 2 and a maximum length of 1000.")]
        public string DataQuery { get; set; }        
    }
}