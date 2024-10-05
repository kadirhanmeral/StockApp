using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Persistence.Models
{
    public class AppUser:IdentityUser<int>,UserEntity
    {
        public string FullName { get; set; } = null!;
        public DateTime DateAdded { get; set; } 
    }
}