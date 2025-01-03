using System.Reflection;
using System.Text;
using Asp.Versioning;
using Diary.Application.GraphQl.Mutations;
using Diary.Application.GraphQl.Queries;
using Diary.Application.GraphQl.Types.BaseTypes;
using Diary.Application.GraphQl.Types.ReportTypes;
using Diary.Application.GraphQl.Types.SubscriptionsTypes;
using Diary.Application.GraphQl.Types.UserTypes;
using Diary.DAL;
using Diary.Domain.Entity;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Path = System.IO.Path;

namespace Diary.Api;

/// <summary>
///     Class with methods for app stratup
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";

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
    /// <param name="builder"></param>
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var termsOfServiceUrl = new Uri(configuration.GetSection(AppStartupSectionName)
            .GetValue<string>("TermsOfServiceUrl"));
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
                TermsOfService = termsOfServiceUrl,
                Contact = new OpenApiContact
                {
                    Name = "Diary api contact",
                    Url = termsOfServiceUrl
                }
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "Diary.Api",
                Description = "Diary api v2",
                TermsOfService = termsOfServiceUrl,
                Contact = new OpenApiContact
                {
                    Name = "Diary api contact",
                    Url = termsOfServiceUrl
                }
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please write valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Name = JwtBearerDefaults.AuthenticationScheme,
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
    public static void LogListeningUrls(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var addresses = app.Configuration.GetSection("ASPNETCORE_URLS");
            var addressesList = addresses.Value?.Split(';').ToList();
            var appStartupUrlLog =
                app.Configuration.GetSection(AppStartupSectionName).GetValue<string>("AppStartupUrlLog");
            addressesList?.ForEach(address =>
            {
                var fullUrlLog = appStartupUrlLog + address;
                Log.Information(fullUrlLog);
            });
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
        var openTelemetryConfiguration =
            builder.Configuration.GetSection(AppStartupSectionName).GetSection("OpenTelemetrySettings");
        var aspireDashboardUri = new Uri(openTelemetryConfiguration.GetValue<string>("AspireDashboardUrl"));
        var jaegerUri = new Uri(openTelemetryConfiguration.GetValue<string>("JaegerUrl"));

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService("DiaryService"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri);
            })
            .WithTracing(traces =>
            {
                traces.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                traces.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri)
                    .AddOtlpExporter(options => options.Endpoint = jaegerUri);
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri);
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });
    }

    /// <summary>
    ///     Add health checks
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = new RabbitMqSettings();
        var redisSettings = new RedisSettings();

        configuration.GetSection(nameof(RabbitMqSettings)).Bind(rabbitMqSettings);
        configuration.GetSection(nameof(RedisSettings)).Bind(redisSettings);

        services.AddHealthChecks()
            .AddRabbitMQ(rabbitMqSettings.GetConnectionString(), name: default)
            .AddRedis(redisSettings.GetConnectionString())
            .AddElasticsearch(configuration.GetSection(AppStartupSectionName).GetValue<string>("ElasticSearchUrl"))
            .AddDbContextCheck<ApplicationDbContext>();
    }
}