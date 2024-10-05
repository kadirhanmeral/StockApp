using App.Application.Features.Users.Dto;
using App.Application.Features.Users.Login;
using App.Application.Features.Users.Logout;
using App.Application.Features.Users.Profile;
using App.Application.Features.Users.Register;

namespace App.Application.Features.Users;

public interface IUserService
{
    Task<ServiceResult> CreateUser(RegisterUserResponse user);

    Task<ServiceResult<object>> Login(LoginUserRequest model);

    Task<ServiceResult> Logout(TokenRequest token);

    Task<ServiceResult<GetProfileResponse>> GetProfile();
}