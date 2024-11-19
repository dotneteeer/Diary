using System.Reflection;
using System.Text;
using Asp.Versioning;
using Diary.Application.GraphQl.Mutations;
using Diary.Application.GraphQl.Queries;
using Diary.Application.GraphQl.Types.BaseTypes;
using Diary.Application.GraphQl.Types.ReportTypes;
using Diary.Application.GraphQl.Types.UserTypes;
using Diary.Domain.Entity;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Path = System.IO.Path;

namespace Diary.Api;

public static class Startup
{
    /// <summary>
    /// Sets up authentication and authorization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public static void AddAuthenticationAndAuthorization(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtOptions = builder.Configuration.GetSection(JwtSettings.DefaultSection).Get<JwtSettings>();
            var jwtKey = jwtOptions.JwtKey;
            var issuer = jwtOptions.Issuer;
            var audience = jwtOptions.Audience;
            var authority = jwtOptions.Authority;
            options.Authority = authority;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
    }

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
                    Url = new Uri("https://aka.ms/aspnetcore/swashbuckle")
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
                    Url = new Uri("https://aka.ms/aspnetcore/swashbuckle")
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
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
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }

    /// <summary>
    /// Logs listening urls
    /// </summary>
    /// <param name="app"></param>
    public static void LogListeningUrls(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var addresses = app.Configuration.GetSection("ASPNETCORE_URLS");
            var addressesList = addresses.Value?.Split(';').ToList();
            addressesList?.ForEach(address => Log.Information("Now listening on: " + address));
        });
    }


    /// <summary>
    /// Adds GraphQl server with types
    /// </summary>
    /// <param name="services"></param>
    public static void AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddInMemorySubscriptions()
            .AddQueryType<ReportQuery>()
            .AddType<ReportType>()
            .AddType<UserType>()
            .AddType(new BaseResultType<Report>())
            .AddType(new CollectionResultType<BaseResult<Report>>())
            .AddMutationType<ReportMutation>()
            .AddSorting()
            .AddFiltering();
    }
}