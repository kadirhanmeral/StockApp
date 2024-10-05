using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Persistence.Models
{
    public class AppRole : IdentityRole<int>,UserEntity
    {
        
    }
}