using App.Application;
using App.Application.Contracts.Authorization;
using Microsoft.AspNetCore.Http;

namespace App.Authorization;

public class AuthorizationService:IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ServiceResult<string>> FindFirstValue(string claimType)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var claimValue = user?.FindFirst(claimType)?.Value;

        if (string.IsNullOrEmpty(claimValue))
        {
            return Task.FromResult(ServiceResult<string>.Fail("Claim not found or value is empty"));
        }

        return Task.FromResult(ServiceResult<string>.Success(claimValue));
    }
}