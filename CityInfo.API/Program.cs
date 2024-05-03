using CityInfo.API.DbContext;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

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
        builder.Services.AddTransient<IMailService , LocalMailService>();
        builder.Services.AddSingleton<CityDataStore>();
        builder.Services.AddDbContext<CityInfoContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:DefaultConnection"]
                );
        });
        builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Authentication:Audiance"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBeFromAntwerp", policy =>
            {
                // user must be authenticated first
                policy.RequireAuthenticatedUser();

                // then the city claim must be Antwerp
                policy.RequireClaim("city", "Antwerp");
            });
        });

        // policy is made of set of requirements 
        // when all requirements evaluate to true , the policy is met 

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

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}