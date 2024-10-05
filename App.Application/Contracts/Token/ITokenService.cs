using App.Application.Features.Users.Dto;

namespace App.Application.Contracts.Token;

public interface ITokenService
{
    Task<ServiceResult<object>> GenerateJwt(UserResponse user);
    
}