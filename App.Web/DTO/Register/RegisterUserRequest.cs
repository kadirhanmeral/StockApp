using System.ComponentModel.DataAnnotations;

namespace App.Web.DTO.Register
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Password Check is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordCheck { get; set; } = default!;
    }
}