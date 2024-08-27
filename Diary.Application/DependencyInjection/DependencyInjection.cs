using Diary.Application.Mapping;
using Diary.Application.Services;
using Diary.Application.Validation;
using Diary.Application.Validation.FluentValidations.Report;
using Diary.Domain.Dto.Report;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Interfaces.Validations;
using Diary.Domain.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diary.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ReportMapping));
        var options = configuration.GetSection(nameof(RedisSettings));
        var redisUrl = options["Url"];
        var instanceName = options["InstanceName"];

        services.AddStackExchangeRedisCache(redisCacheOptions =>
        {
            redisCacheOptions.Configuration = redisUrl;
            redisCacheOptions.InstanceName = instanceName;
        });
        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IReportValidator, ReportValidator>();
        services.AddScoped<IValidator<CreateReportDto>, CreateReportValidator>();
        services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidator>();
        services.AddScoped<IValidator<PageReportDto>, PageReportDtoValidator>();

        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}