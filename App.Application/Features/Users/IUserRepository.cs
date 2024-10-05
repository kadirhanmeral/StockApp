using App.Application.Features.Users.Create;
using App.Application.Features.Users.Dto;

namespace App.Application.Features.Users;

public interface IUserRepository
{
    Task<ServiceResult<UserResponse>> FindByEmailAsync(string email);
    Task<ServiceResult<UserResponse>> FindByNameAsync(string userName);
    
    Task<ServiceResult> CreateAsync(CreateNewUserRequest user, string password);
    
    Task<ServiceResult> CheckPasswordSignInAsync(string email, string password, bool lockOutFailure);

    
}