using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "Enter a name to continue")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a valid email address to continue")]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 0)]
        public string SchoolTrustName { get; set; }

        [Required(ErrorMessage = "Enter a message to continue")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "The field Message must be a string with a minimum length of 2 and a maximum length of 1000.")]
        public string Message { get; set; }
    }
}