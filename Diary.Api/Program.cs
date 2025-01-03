using Diary.Api;
using Diary.Api.Middlewares;
using Diary.Application.DependencyInjection;
using Diary.Consumer.DependencyInjection;
using Diary.DAL.DependencyInjection;
using Diary.Domain.Settings;
using Diary.Producer.DependencyInjection;
using GraphQL.Server.Ui.Voyager;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.UseHttpClientMetrics();

builder.Services.AddSwagger(builder.Configuration);


builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
        .WriteTo.OpenTelemetry(options =>
        {
            options.Endpoint = builder.Configuration.GetSection("AppStartupSettings")
                .GetSection("OpenTelemetrySettings").GetValue<string>("AspireDashboardUrl");
            options.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = "DiaryService",
                ["service.instance.id"] = Guid.NewGuid().ToString()
            };
        })
//.Enrich.FromLogContext()
);

builder.AddOpenTelemetry();
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddProducer();
builder.Services.AddConsumer();
builder.Services.AddGraphQl();
builder.Services.AddHealthCheck(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Diary Swagger v1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Diary Swagger v2.0");
        //options.RoutePrefix=string.Empty;//https://localhost:3306/index.html
    });
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("/random-number-game", () => Results.Ok(Random.Shared.Next(0, 10)));

app.MapMetrics();
app.UseRouting();
app.UseAuthorization(); //added by me because Authorization didn't work
app.MapControllers();
app.MapGraphQL();
app.UseGraphQLVoyager("/graphql-voyager", new VoyagerOptions { GraphQLEndPoint = "/graphql" });
app.UseWebSockets();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.LogListeningUrls();

await app.RunAsync();

public partial class Program
{
}