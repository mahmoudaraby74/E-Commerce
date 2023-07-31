
using System.ComponentModel.DataAnnotations;

namespace StoreAPI.Dtos
{
    public class SignUpDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W)(?!.*\\s).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one non-alphanumeric character, and at least six characters in total.")]
        public string Password { get; set; }
    }
}
