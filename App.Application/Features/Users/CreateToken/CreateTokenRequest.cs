namespace App.Application.Features.Users.CreateToken;

public record CreateTokenRequest(int UserId, string Email, string FullName, string UserName);