using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class GetInvolvedViewModel
    {
        [Required(ErrorMessage = "Enter a name to continue")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a valid email address to continue")]
        [EmailAddress(ErrorMessage = "Enter a valid email address to continue")]
        public string Email { get; set; }
    }
}