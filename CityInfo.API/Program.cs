using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()          
            .WriteTo.Console()             
            .WriteTo.File("logs/cityinfo.txt" , rollingInterval: RollingInterval.Day)
            .CreateLogger(); 

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        // Add services to the container.

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        })
        .AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters();

        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

        builder.Services.AddTransient<LocalMailService>();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}