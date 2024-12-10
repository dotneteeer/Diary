using System.Reflection;
using Asp.Versioning;
using Diary.Application.GraphQl.Mutations;
using Diary.Application.GraphQl.Queries;
using Diary.Application.GraphQl.Types.BaseTypes;
using Diary.Application.GraphQl.Types.ReportTypes;
using Diary.Application.GraphQl.Types.SubscriptionsTypes;
using Diary.Application.GraphQl.Types.UserTypes;
using Diary.Domain.Entity;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

        var expireTimeSpan = builder.Configuration.GetSection(JwtSettings.DefaultSection).GetValue<int>("LifeTime");
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/api/v1/auth/login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(expireTimeSpan);
                options.Cookie.HttpOnly = true;
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
            .AddSubscriptionType<ReportSubscriptionType>()
            .AddSorting()
            .AddFiltering();
    }

    /// <summary>
    /// Adds open telemetry services
    /// </summary>
    /// <param name="builder"></param>
    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService("DiaryService"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:18889"));
            })
            .WithTracing(traces =>
            {
                traces.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                traces.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:18889"))
                    .AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:18889"));
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });
    }
}