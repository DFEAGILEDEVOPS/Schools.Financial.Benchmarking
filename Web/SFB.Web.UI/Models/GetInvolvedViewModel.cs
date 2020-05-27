using System.ComponentModel.DataAnnotations;

namespace SFB.Web.UI.Models
{
    public class GetInvolvedViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}