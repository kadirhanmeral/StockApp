namespace App.Application.Contracts.Authorization;

public interface IAuthorizationService
{
    Task<ServiceResult<string>> FindFirstValue(string claimType);
}