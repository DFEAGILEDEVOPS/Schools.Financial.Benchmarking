using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class DataQueryViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Reference { get; set; }

        [Required(ErrorMessage = "Data query field is required.")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "The field Data query must be a string with a minimum length of 2 and a maximum length of 1000.")]
        public string DataQuery { get; set; }        
    }
}