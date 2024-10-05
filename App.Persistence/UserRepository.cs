using App.Application;
using App.Application.Features.Users;
using App.Application.Features.Users.Create;
using App.Application.Features.Users.Dto;
using App.Domain.Entities;
using App.Persistence.Models;
using Microsoft.AspNetCore.Identity;

namespace App.Persistence;


public class UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    : IUserRepository
{
    

    public async Task<ServiceResult<UserResponse>> FindByEmailAsync(string email)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            return ServiceResult<UserResponse>.Fail("Email not found");
        }
        var user = new UserResponse(
            Id:appUser.Id,
            UserName: appUser.UserName!,
            Email: appUser.Email!,
            FullName: appUser.FullName
        );
        
        return ServiceResult<UserResponse>.Success(user);
    }

    public async Task<ServiceResult<UserResponse>> FindByNameAsync(string userName)
    {
        var appUser = await userManager.FindByNameAsync(userName);
        if (appUser == null)
        {
            return ServiceResult<UserResponse>.Fail("user not found");
        }
        var user = new UserResponse(
            Id:appUser.Id,
            UserName: appUser.UserName!,
            Email: appUser.Email!,
            FullName: appUser.FullName
        );
        
        return ServiceResult<UserResponse>.Success(user);
    }



    public async Task<ServiceResult> CreateAsync(CreateNewUserRequest user, string password)
    {
        var appUser = new AppUser
        {
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            DateAdded = DateTime.Now
        };
        var identityResult = await userManager.CreateAsync(appUser, password);
        var result = new ServiceResult
        {
            ErrorMessages = identityResult.Errors.Select(e => e.Description).ToList()
        };
        return result;
    }

    public async Task<ServiceResult> CheckPasswordSignInAsync(string email, string password, bool lockOutFailure)
    {
        var user = await userManager.FindByEmailAsync(email);
        var signInResult = await signInManager.CheckPasswordSignInAsync(user!, password, lockOutFailure);
        var result = new ServiceResult
        {
            ErrorMessages = new List<string>()
        };
        
        if (signInResult.Succeeded)
        {
            return result;
        }
        else
        {
            result.ErrorMessages.Add("Password is not correct");
        }
        
        

        return result;

    }
}

