using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using SmartTaskManagement.API.Middleware;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Infrastructure.Data;
using SmartTaskManagement.Infrastructure.Repositories;
using SmartTaskManagement.Infrastructure.Services;
using SmartTaskManagement.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using SmartTaskManagement.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using FluentValidation;
using SmartTaskManagement.Application.Validators;


namespace SmartTaskManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/smarttask-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();        

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Configure Swagger with JWT support
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Smart Task Management API",
                Version = "v1",
                Description = "A comprehensive task management system with JWT authentication"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: \"Bearer 12345abcdef\""
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                   [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            // c.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         Array.Empty<string>()
            //     }
            // });
        });


        // Configure Database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection2")));

        // Configure JWT Settings
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        builder.Services.Configure<JwtSettings>(jwtSettings);

        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

        // Configure AI Settings
        var aiSettings = builder.Configuration.GetSection("AiSettings");
        builder.Services.Configure<AiSettings>(aiSettings);

        // Add HttpClient for AI service
        builder.Services.AddHttpClient<IAiService, AiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Add Memory Cache for AI responses
        builder.Services.AddMemoryCache();


        // Configure Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Configure Authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ProjectManagerOrAdmin", policy => 
                policy.RequireRole("Admin", "ProjectManager"));
            options.AddPolicy("AllUsers", policy => 
                policy.RequireRole("Admin", "ProjectManager", "TeamMember"));
        });

        // Add FluentValidation
        builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectDtoValidator>();

        // Configure Password Hasher
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Register Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Services
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        builder.Services.AddScoped<IAiService, AiService>();

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        //Health Checks
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        // Configure Rate Limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("Fixed", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(10);
                opt.PermitLimit = 5;
                opt.QueueLimit = 2;
            });
        });

        // Add AutoMapper (if you want to use it instead of manual mapping)
        builder.Services.AddAutoMapper(typeof(Program));


        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Global Exception Handling Middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Rate Limiting
        app.UseRateLimiter();

        // CORS
        app.UseCors("AllowAngularApp");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Health Checks
        app.MapHealthChecks("/health");

        // Controllers
        app.MapControllers();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        // var summaries = new[]
        // {
        //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        // };

        // app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        // {
        //     var forecast = Enumerable.Range(1, 5).Select(index =>
        //         new WeatherForecast
        //         {
        //             Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //             TemperatureC = Random.Shared.Next(-20, 55),
        //             Summary = summaries[Random.Shared.Next(summaries.Length)]
        //         })
        //         .ToArray();
        //     return forecast;
        // })
        // .WithName("GetWeatherForecast");

        app.Run();
    }
}
