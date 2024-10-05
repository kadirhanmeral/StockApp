using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace App.Web.DTO.Login
{
    public class LoginUserRequest
    {
        [Required]
        [JsonProperty("email")]
        public string? Email { get; set; } = null!;

        [Required]
        [JsonProperty("password")]
        public string Password { get; set; } = null!;
    }
}