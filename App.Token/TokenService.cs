using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Application;
using App.Application.Contracts.Token;
using App.Application.Features.Users.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace App.Token;

public class TokenService(IConfiguration configuration):ITokenService
{
    public Task<ServiceResult<object>> GenerateJwt(UserResponse user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Secret").Value ?? "");    
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.NameIdentifier, user.UserName!)

            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(ServiceResult<object>.Success(tokenHandler.WriteToken(token)));
    }
    
    


}