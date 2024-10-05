using App.Application.Features.Users;
using App.Application.Features.Users.Login;
using App.Application.Features.Users.Logout;
using App.Application.Features.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserResponse user)
    {
        var result =await userService.CreateUser(user);
        return Ok(result);
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserRequest user)
    {
        var result =await userService.Login(user);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout(TokenRequest token)
    {
        var result =await userService.Logout(token);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("GetProfile")]
    public async Task<IActionResult> GetProfile()
    {
        var result =await userService.GetProfile();
        return Ok(result);
    }
}