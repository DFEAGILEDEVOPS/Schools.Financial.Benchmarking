using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class GetInvolvedViewModel
    {
        [Required(ErrorMessage = "The Name field is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a valid email address to continue")]
        [EmailAddress]
        public string Email { get; set; }
    }
}