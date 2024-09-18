using Diary.Api;
using Diary.Api.Middlewares;
using Diary.Application.DependencyInjection;
using Diary.Consumer.DependencyInjection;
using Diary.DAL.DependencyInjection;
using Diary.Domain.Settings;
using Diary.Producer.DependencyInjection;
using Hangfire;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.UseHttpClientMetrics();

builder.Services.AddSwagger();


builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddProducer();
builder.Services.AddConsumer();
builder.Services.AddHangfire(builder.Configuration);

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
app.UseHangfireDashboard("/dashboard");
app.UseHangfireServer();
app.MapControllers();
Startup.LogListeningUrls(app);

app.Run();