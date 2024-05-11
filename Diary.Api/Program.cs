using Diary.Api;
using Diary.Application.DependencyInjection;
using Diary.DAL.DependencyInjection;
using Diary.Domain.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);

//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();


builder.Host.UseSerilog((context, configuration)=>configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json","Diary Swagger v1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json","Diary Swagger v2.0");
        //options.RoutePrefix=string.Empty;//https://localhost:3306/index.html
    });
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();
