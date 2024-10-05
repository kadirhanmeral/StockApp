using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace App.Application.Extensions;

public  static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {

        
        //TODO: api katmanina tasinacak
        //services.AddScoped(typeof(NotFoundFilter<,>));
        //services.AddExceptionHandler<CriticalExteptionHandler>();
        //services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        
        
        return services;
    }
}