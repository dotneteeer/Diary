using System.Collections.ObjectModel;
using System.Data;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Diary.Api;

public static class Startup
{
    /// <summary>
    /// Swagger set up
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Diary.Api",
                Description = "Diary api v1",
                TermsOfService = new Uri("https://aka.ms/aspnetcore/swashbuckle"),
                Contact = new OpenApiContact
                {
                    Name = "Diary api contact",
                    Url=new Uri("https://aka.ms/aspnetcore/swashbuckle")
                }
            });
            
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "Diary.Api",
                Description = "Diary api v2",
                TermsOfService = new Uri("https://aka.ms/aspnetcore/swashbuckle"),
                Contact = new OpenApiContact
                {
                    Name = "Diary api contact",
                    Url=new Uri("https://aka.ms/aspnetcore/swashbuckle")
                }
            });
             
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In=ParameterLocation.Header,
                Description = "Please write valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    }, 
                    Array.Empty<string>()
                }
            });
        });
        
        
    }
}