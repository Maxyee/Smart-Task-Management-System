# Part 1: Architecture Design
## Step 1: Create the Solution and Project Structure

First, create a new directory and solution:

```bash
# Create solution directory
mkdir Smart-Task-Management-System
cd Smart-Task-Management-System

# Create solution file
dotnet new sln -n SmartTaskManagement

```

## Step 2: Create the Source Folder

```bash
# Create src directory
mkdir src

# Navigate to the directory
cd src
```

## Step 3: Create Domain Project (Class Library)

```bash
dotnet new classlib -n SmartTaskManagement.Domain -f net10.0
```

## Step 4: Create Application Project (Class Library)

```bash
dotnet new classlib -n SmartTaskManagement.Application -f net10.0
```

## Step 5: Create Infrastructure Project (Class Library)

```bash
dotnet new classlib -n SmartTaskManagement.Infrastructure -f net10.0
```

## Step 6: Create API Project (Web API with Program.cs Main method)

```bash
dotnet new webapi -n SmartTaskManagement.API -f net10.0 --use-program-main
```

## Step 7: Add Projects to Solution

```bash

# navigate to the root folder ./Smart-Task-Management-System
cd ..

# Add all projects to the solution
dotnet sln add src/SmartTaskManagement.Domain/SmartTaskManagement.Domain.csproj
dotnet sln add src/SmartTaskManagement.Application/SmartTaskManagement.Application.csproj
dotnet sln add src/SmartTaskManagement.Infrastructure/SmartTaskManagement.Infrastructure.csproj
dotnet sln add src/SmartTaskManagement.API/SmartTaskManagement.API.csproj

```
## Step 8: Set Up Project References

```bash
# Application references Domain
dotnet add src/SmartTaskManagement.Application/SmartTaskManagement.Application.csproj reference src/SmartTaskManagement.Domain/SmartTaskManagement.Domain.csproj

# Infrastructure references Domain and Application
dotnet add src/SmartTaskManagement.Infrastructure/SmartTaskManagement.Infrastructure.csproj reference src/SmartTaskManagement.Domain/SmartTaskManagement.Domain.csproj
dotnet add src/SmartTaskManagement.Infrastructure/SmartTaskManagement.Infrastructure.csproj reference src/SmartTaskManagement.Application/SmartTaskManagement.Application.csproj

# API references Domain, Application, and Infrastructure
dotnet add src/SmartTaskManagement.API/SmartTaskManagement.API.csproj reference src/SmartTaskManagement.Domain/SmartTaskManagement.Domain.csproj
dotnet add src/SmartTaskManagement.API/SmartTaskManagement.API.csproj reference src/SmartTaskManagement.Application/SmartTaskManagement.Application.csproj
dotnet add src/SmartTaskManagement.API/SmartTaskManagement.API.csproj reference src/SmartTaskManagement.Infrastructure/SmartTaskManagement.Infrastructure.csproj
```

## Step 9: Add Essential NuGet Packages
### Infrastructure Project (for Entity Framework, Identity)

```bash
cd src/SmartTaskManagement.Infrastructure

# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Identity
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# For JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

```

### API Project

```bash
cd ../SmartTaskManagement.API

# Swagger/OpenAPI
dotnet add package Swashbuckle.AspNetCore

# Entity Framework Design (for migrations)
dotnet add package Microsoft.EntityFrameworkCore.Design

```


### Application Project (FluentValidation)

```bash
cd ../SmartTaskManagement.Application

# For validation
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

# AutoMapper
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

## Step 10: Create the Folder Structure

```bash

# Navigate back to root
cd ../../

# Create folder structure in each project

# Domain folders
mkdir src/SmartTaskManagement.Domain/Entities
mkdir src/SmartTaskManagement.Domain/Enums
mkdir src/SmartTaskManagement.Domain/Interfaces
mkdir src/SmartTaskManagement.Domain/Common

# Application folders
mkdir src/SmartTaskManagement.Application/Common
mkdir src/SmartTaskManagement.Application/DTOs
mkdir src/SmartTaskManagement.Application/Features
mkdir src/SmartTaskManagement.Application/Features/Auth
mkdir src/SmartTaskManagement.Application/Features/Projects
mkdir src/SmartTaskManagement.Application/Features/Tasks
mkdir src/SmartTaskManagement.Application/Interfaces
mkdir src/SmartTaskManagement.Application/Services

# Infrastructure folders
mkdir src/SmartTaskManagement.Infrastructure/Data
mkdir src/SmartTaskManagement.Infrastructure/Identity
mkdir src/SmartTaskManagement.Infrastructure/Services
mkdir src/SmartTaskManagement.Infrastructure/Extensions

# API folders
mkdir src/SmartTaskManagement.API/Controllers
mkdir src/SmartTaskManagement.API/Middleware
mkdir src/SmartTaskManagement.API/Filters

# Tests folder
mkdir tests/SmartTaskManagement.Tests

```

## Step 11: Create Test Project (Optional)

```bash

cd tests/SmartTaskManagement.Tests
# Create test project
dotnet new xunit -n SmartTaskManagement.Tests -f net10.0


# Navigate to the root 
cd ../../


# Add references to test project
dotnet add tests/SmartTaskManagement.Tests/SmartTaskManagement.Tests.csproj reference src/SmartTaskManagement.Domain/SmartTaskManagement.Domain.csproj
dotnet add tests/SmartTaskManagement.Tests/SmartTaskManagement.Tests.csproj reference src/SmartTaskManagement.Application/SmartTaskManagement.Application.csproj

# Add test NuGet packages
cd tests/SmartTaskManagement.Tests
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Moq
dotnet add package FluentAssertions

```

## Step 12: Build and Restore the project

```bash
# Go back to solution root
cd ../..

# Restore all packages
dotnet restore

# Build the solution
dotnet build

```
This is the end of Part 1.


# Part 2 : Domain Layer Implementation
## Base Entity

```cs
// SmartTaskManagement.Domain/Common/BaseEntity.cs
namespace SmartTaskManagement.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
```

## Enums
```cs
// SmartTaskManagement.Domain/Enums/TaskItemStatus.cs

namespace SmartTaskManagement.Domain.Enums
{
    public enum TaskItemStatus
    {
        ToDo = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}

// SmartTaskManagement.Domain/Enums/TaskItemPriority.cs
namespace SmartTaskManagement.Domain.Enums
{
    public enum TaskItemPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}

// SmartTaskManagement.Domain/Enums/Role.cs
namespace SmartTaskManagement.Domain.Enums
{
    public enum UserRole
    {
        Admin = 0,
        ProjectManager = 1,
        TeamMember = 2
    }
}
```

## Domain Entities

```cs
// SmartTaskManagement.Domain/Entities/User.cs
using SmartTaskManagement.Domain.Common;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}


// SmartTaskManagement.Domain/Entities/Project.cs
using SmartTaskManagement.Domain.Common;

namespace SmartTaskManagement.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        // Foreign Keys
        public Guid CreatedByUserId { get; set; }

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

// SmartTaskManagement.Domain/Entities/TaskItem.cs
using SmartTaskManagement.Domain.Common;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskItemPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }

        // Foreign Keys
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid CreatedByUserId { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual User AssignedToUser { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
    }
}
```
## Bulil The project
```bash
# from the root folder run build
dotnet build
```
This is the end of Part 2

# Part 3 : Application Layer - DTOs and Interfaces
## DTOs for Authentication

```cs
// SmartTaskManagement.Application/DTOs/Auth/RegisterDto.cs
namespace SmartTaskManagement.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

// SmartTaskManagement.Application/DTOs/Auth/LoginDto.cs
namespace SmartTaskManagement.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

// SmartTaskManagement.Application/DTOs/Auth/AuthResponseDto.cs
namespace SmartTaskManagement.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}

// SmartTaskManagement.Application/DTOs/Auth/RefreshTokenDto.cs
namespace SmartTaskManagement.Application.DTOs.Auth
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
```

## Common Response Wrapper

```cs
// SmartTaskManagement.Application/Common/Response.cs
namespace SmartTaskManagement.Application.Common
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Response<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new Response<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static Response<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Response<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}

```

## Application Interfaces

```cs
// SmartTaskManagement.Application/Interfaces/Services/IAuthService.cs
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<Response<bool>> LogoutAsync(string refreshToken);
        Task<Response<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }

}

// SmartTaskManagement.Application/Interfaces/Services/ITokenService.cs
using System.Security.Claims;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        (string AccessToken, string RefreshToken) GenerateTokens(IEnumerable<Claim> claims);
        int GetRefreshTokenExpiryDays();
    }

}

// SmartTaskManagement.Application/Interfaces/Repositories/IUnitOfWork.cs
namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

// SmartTaskManagement.Application/Interfaces/Repositories/IUserRepository.cs
using SmartTaskManagement.Domain.Entities;

namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null);
    }
}


// SmartTaskManagement.Application/Interfaces/Repositories/IProjectRepository.cs
using SmartTaskManagement.Domain.Entities;


namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByUserAsync(Guid userId);
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
    Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm);
    Task<Project?> GetProjectWithTasksAsync(Guid projectId);
    Task<int> GetProjectCountByUserAsync(Guid userId);
}
}

// SmartTaskManagement.Application/Interfaces/Repositories/ITaskRepository.cs
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId);
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskItemPriority priority);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> SearchTasksAsync(string searchTerm);
        Task<Dictionary<TaskItemStatus, int>> GetTaskStatusCountsAsync(Guid? projectId = null);
        Task<Dictionary<TaskItemPriority, int>> GetTaskPriorityCountsAsync(Guid? projectId = null);
    }
}

// SmartTaskManagement.Application/Interfaces/Repositories/IGenericRepository.cs
using System.Linq.Expressions;

namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    }
}

```

This is the end of Part 3


# Part 4 :  Infrastructure Layer - JWT Implementation
## JWT Settings

```cs
// SmartTaskManagement.Infrastructure/Settings/JwtSettings.cs

namespace SmartTaskManagement.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}


```

## Token Service Implementation

```cs
// SmartTaskManagement.Infrastructure/Services/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Infrastructure.Settings;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public (string AccessToken, string RefreshToken) GenerateTokens(IEnumerable<Claim> claims)
        {
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();
            return (accessToken, refreshToken);
        }

        public int GetRefreshTokenExpiryDays()
        {
            return _jwtSettings.RefreshTokenExpiryDays;
        }
    }
}
```

## Auth Service Implementation

```cs
// SmartTaskManagement.Infrastructure/Services/AuthService.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user exists
                var existingUser = await _unitOfWork.Users
                    .FindAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);

                if (existingUser.Any())
                {
                    return Response<AuthResponseDto>.FailureResponse(
                        "User with this email or username already exists",
                        409);
                }

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = registerDto.Email,
                    Username = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = UserRole.TeamMember, // Default role
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Hash password
                user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Generate tokens
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                _logger.LogInformation("User {Username} registered successfully", user.Username);
                return Response<AuthResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during registration",
                    500);
            }
        }

        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FindAsync(u => u.Email == loginDto.Email)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (user == null)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid credentials", 401);
                }

                var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, loginDto.Password);

                if (passwordVerification == PasswordVerificationResult.Failed)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid credentials", 401);
                }

                if (!user.IsActive)
                {
                    return Response<AuthResponseDto>.FailureResponse("Account is deactivated", 403);
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                // Update refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                _logger.LogInformation("User {Username} logged in successfully", user.Username);
                return Response<AuthResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during login",
                    500);
            }
        }

        public async Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid token", 401);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));

                if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken ||
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid refresh token", 401);
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (newAccessToken, newRefreshToken) = _tokenService.GenerateTokens(claims);

                // Update refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                return Response<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during token refresh",
                    500);
            }
        }

        public async Task<Response<bool>> LogoutAsync(string refreshToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FindAsync(u => u.RefreshToken == refreshToken)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    user.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.CompleteAsync();
                }

                return Response<bool>.SuccessResponse(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Response<bool>.FailureResponse(
                    "An error occurred during logout",
                    500);
            }
        }

        public async Task<Response<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, currentPassword);

                if (passwordVerification == PasswordVerificationResult.Failed)
                {
                    return Response<bool>.FailureResponse("Current password is incorrect", 401);
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return Response<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return Response<bool>.FailureResponse(
                    "An error occurred while changing password",
                    500);
            }
        }
    }
}

```

this is the end of Part 4

# Part 5: Database Configuration
## DbContext

```cs
// SmartTaskManagement.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasConversion<int>();

                // Seed admin user
                entity.HasData(new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Email = "admin@smarttask.com",
                    Username = "admin",
                    FirstName = "System",
                    LastName = "Admin",
                    PasswordHash = "AQAAAAIAAYagAAAAEMy1zqjMv7g9Xh6sHv2nwS0jN1CkOxLzRqT3vW5bY2aG8fHg9JkLmNoPqRsTuVwXyZ", // Hashed "Admin@123"
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            });

            // Project Configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.StartDate).IsRequired();

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.Projects)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Task Configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.Priority).IsRequired().HasConversion<int>();
                entity.Property(e => e.DueDate).IsRequired();

                entity.HasOne(e => e.Project)
                    .WithMany(e => e.Tasks)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssignedToUser)
                    .WithMany(e => e.AssignedTasks)
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.AssignedToUserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.DueDate);
            });
        }
    }
}
```


## Repository Implementations

### Generic Repository Implementation
```cs
// SmartTaskManagement.Infrastructure/Repositories/GenericRepository.cs
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Infrastructure.Data;
using System.Linq.Expressions;


namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
```

### User Repository Implementation

```cs
// SmartTaskManagement.Infrastructure/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;


namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.RefreshToken == refreshToken &&
                !u.IsDeleted &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role.ToString() == role && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Email == email && !u.IsDeleted);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Username == username && !u.IsDeleted);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
```

###  Project Repository Implementation

```cs
// SmartTaskManagement.Infrastructure/Repositories/ProjectRepository.cs
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.CreatedByUserId == userId && !p.IsDeleted)
                .Include(p => p.CreatedByUser)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.CreatedByUser)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted &&
                    (p.Name.Contains(searchTerm) ||
                     p.Description.Contains(searchTerm)))
                .Include(p => p.CreatedByUser)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectWithTasksAsync(Guid projectId)
        {
            return await _dbSet
                .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.AssignedToUser)
                .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.CreatedByUser)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);
        }

        public async Task<int> GetProjectCountByUserAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(p => p.CreatedByUserId == userId && !p.IsDeleted);
        }
    }
}
```

### Task Repository Implementation

```cs
// SmartTaskManagement.Infrastructure/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId)
        {
            return await _dbSet
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Include(t => t.Project)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(t => t.AssignedToUserId == userId && !t.IsDeleted)
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status)
        {
            return await _dbSet
                .Where(t => t.Status == status && !t.IsDeleted)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskItemPriority priority)
        {
            return await _dbSet
                .Where(t => t.Priority == priority && !t.IsDeleted)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            return await _dbSet
                .Where(t => t.DueDate < DateTime.UtcNow &&
                            t.Status != TaskItemStatus.Completed &&
                            t.Status != TaskItemStatus.Cancelled &&
                            !t.IsDeleted)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string searchTerm)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted &&
                    (t.Title.Contains(searchTerm) ||
                     t.Description.Contains(searchTerm)))
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task<Dictionary<TaskItemStatus, int>> GetTaskStatusCountsAsync(Guid? projectId = null)
        {
            var query = _dbSet.Where(t => !t.IsDeleted);
            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var counts = await query
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            // Ensure all statuses are present
            foreach (TaskItemStatus status in Enum.GetValues(typeof(TaskItemStatus)))
            {
                if (!counts.ContainsKey(status))
                {
                    counts[status] = 0;
                }
            }

            return counts;
        }

        public async Task<Dictionary<TaskItemPriority, int>> GetTaskPriorityCountsAsync(Guid? projectId = null)
        {
            var query = _dbSet.Where(t => !t.IsDeleted);
            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var counts = await query
                .GroupBy(t => t.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Priority, x => x.Count);

            // Ensure all priorities are present
            foreach (TaskItemPriority priority in Enum.GetValues(typeof(TaskItemPriority)))
            {
                if (!counts.ContainsKey(priority))
                {
                    counts[priority] = 0;
                }
            }

            return counts;
        }
    }
}
```
### Unit of Work Implementation

```cs
// SmartTaskManagement.Infrastructure/Repositories/UnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository)
        {
            _context = context;
            Users = userRepository;
            Projects = projectRepository;
            Tasks = taskRepository;
        }

        public IUserRepository Users { get; }
        public IProjectRepository Projects { get; }
        public ITaskRepository Tasks { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}
```

This is the end of Part 5

# Part 6 : API Layer - Controllers
## Auth Controller

```cs
// SmartTaskManagement.API/Controllers/AuthController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;
using SmartTaskManagement.Application.Interfaces.Services;


namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Login user and get tokens
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var result = await _authService.LogoutAsync(refreshToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(
                Guid.Parse(userId),
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }

    // DTO for change password
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
```

## Program.cs Configuration

```bash
# go to the ./SmartTaskManagement.API
cd ./SmartTaskManagement.API

# then install the below packages
dotnet add package Serilog.AspNetCore
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
```

```cs
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
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure JWT Settings
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        builder.Services.Configure<JwtSettings>(jwtSettings);

        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

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


```

##  Global Exception Handling Middleware
```cs
// SmartTaskManagement.API/Middleware/ExceptionHandlingMiddleware.cs
using System.Text.Json;
using SmartTaskManagement.Application.Common;

namespace SmartTaskManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new Response<object>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                StatusCode = 500
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = 401;
                    response.Message = "Unauthorized access";
                    break;
                case KeyNotFoundException:
                    response.StatusCode = 404;
                    response.Message = "Resource not found";
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    response.StatusCode = 400;
                    response.Message = exception.Message;
                    break;
                default:
                    response.StatusCode = 500;
                    response.Message = "Internal server error";
                    break;
            }

            context.Response.StatusCode = response.StatusCode;
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
```

This is the end of Part 6

# Part 7 : Configuration Files
## appsettings.json (SmartTaskManagement.API)
```json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartTaskManagement;Trusted_Connection=True;MultipleActiveResultSets=true",
    "DefaultConnection2": "Server=localhost\\SQLEXPRESS;Database=JwtAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YOUR-VERY-LONG-SECRET-KEY-HERE-MINIMUM-32-CHARACTERS",
    "Issuer": "SmartTaskManagement",
    "Audience": "SmartTaskManagementAPI",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "AllowedHosts": "*"
}

```

## appsettings.Development.json (SmartTaskManagement.API)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartTaskManagement;Trusted_Connection=True;MultipleActiveResultSets=true",
    "DefaultConnection2": "Server=localhost\\SQLEXPRESS;Database=JwtAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyForDevelopmentOnly12345!",
    "Issuer": "SmartTaskManagement",
    "Audience": "SmartTaskManagementAPI",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## build and run the API project
```bash
# first navigate to the API project
cd ./SmartTaskManagement.API

# build the project
dotnet build

# run the project
dotnet run

# is project is working or not visit the url below:
http://localhost:5027/swagger/index.html

```

This is the end of Part 7

# Part 8 : Project Management Module

## DTOs

### Project DTOs

```cs
// SmartTaskManagement.Application/DTOs/Projects/ProjectDto.cs
using SmartTaskManagement.Application.DTOs.Shared;

namespace SmartTaskManagement.Application.DTOs.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTasks { get; set; }
        public UserDto CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProjectFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }

    public class ProjectTaskSummaryDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int ToDoTasks { get; set; }
        public int CancelledTasks { get; set; }
        public double CompletionPercentage { get; set; }
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
```

### User DTOs

```cs
namespace SmartTaskManagement.Application.DTOs.Shared
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

}

```

## Services

### Project Service Interface

```cs

// SmartTaskManagement.Application/Interfaces/Services/IProjectService.cs

using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<Response<ProjectDto>> CreateProjectAsync(CreateProjectDto createDto, Guid userId);
        Task<Response<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateDto, Guid userId);
        Task<Response<bool>> DeleteProjectAsync(Guid projectId, Guid userId);
        Task<Response<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid userId);
        Task<Response<PagedResponse<ProjectDto>>> GetProjectsAsync(ProjectFilterDto filter, Guid userId);
        Task<Response<ProjectTaskSummaryDto>> GetProjectTaskSummaryAsync(Guid projectId, Guid userId);
        Task<Response<bool>> ToggleProjectStatusAsync(Guid projectId, Guid userId);
    }
}
```

### Project Service Implementation

```cs
// SmartTaskManagement.Infrastructure/Services/ProjectService.cs
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<ProjectDto>> CreateProjectAsync(CreateProjectDto createDto, Guid userId)
        {
            try
            {
                // Validate user exists
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                // Validate dates
                if (createDto.EndDate.HasValue && createDto.EndDate < createDto.StartDate)
                {
                    return Response<ProjectDto>.FailureResponse("End date must be after start date", 400);
                }

                // Create project
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = createDto.Name,
                    Description = createDto.Description,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    IsActive = true,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Projects.AddAsync(project);
                await _unitOfWork.CompleteAsync();

                var projectDto = await MapToProjectDto(project);

                _logger.LogInformation("Project {ProjectName} created by user {UserId}", project.Name, userId);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project for user {UserId}", userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while creating the project", 500);
            }
        }

        public async Task<Response<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateDto, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission (Admin or Project Owner)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<ProjectDto>.FailureResponse("You don't have permission to update this project", 403);
                }

                // Validate dates
                if (updateDto.EndDate.HasValue && updateDto.EndDate < updateDto.StartDate)
                {
                    return Response<ProjectDto>.FailureResponse("End date must be after start date", 400);
                }

                // Update project
                project.Name = updateDto.Name;
                project.Description = updateDto.Description;
                project.StartDate = updateDto.StartDate;
                project.EndDate = updateDto.EndDate;
                project.IsActive = updateDto.IsActive;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                var projectDto = await MapToProjectDto(project);

                _logger.LogInformation("Project {ProjectName} updated by user {UserId}", project.Name, userId);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId} for user {UserId}", projectId, userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while updating the project", 500);
            }
        }

        public async Task<Response<bool>> DeleteProjectAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
                if (project == null)
                {
                    return Response<bool>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission (Admin or Project Owner)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to delete this project", 403);
                }

                // Soft delete
                project.IsDeleted = true;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Project {ProjectName} deleted by user {UserId}", project.Name, userId);
                return Response<bool>.SuccessResponse(true, "Project deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId} for user {UserId}", projectId, userId);
                return Response<bool>.FailureResponse("An error occurred while deleting the project", 500);
            }
        }

        public async Task<Response<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission to view
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<ProjectDto>.FailureResponse("You don't have permission to view this project", 403);
                }

                var projectDto = await MapToProjectDto(project);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId} for user {UserId}", projectId, userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while retrieving the project", 500);
            }
        }

        public async Task<Response<PagedResponse<ProjectDto>>> GetProjectsAsync(ProjectFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PagedResponse<ProjectDto>>.FailureResponse("User not found", 404);
                }

                // Build query
                var query = _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);

                // Apply filters
                var projects = await query;
                var filteredProjects = projects.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    filteredProjects = filteredProjects.Where(p =>
                        p.Name.Contains(filter.SearchTerm) ||
                        p.Description.Contains(filter.SearchTerm));
                }

                if (filter.IsActive.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.IsActive == filter.IsActive.Value);
                }

                if (filter.StartDateFrom.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.StartDate >= filter.StartDateFrom.Value);
                }

                if (filter.StartDateTo.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.StartDate <= filter.StartDateTo.Value);
                }

                // Apply user filter for non-admins
                if (user.Role != UserRole.Admin)
                {
                    filteredProjects = filteredProjects.Where(p => p.CreatedByUserId == userId);
                }
                else if (filter.CreatedByUserId.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.CreatedByUserId == filter.CreatedByUserId.Value);
                }

                // Apply sorting
                filteredProjects = ApplySorting(filteredProjects, filter.SortBy, filter.SortDescending);

                // Get total count
                var totalCount = filteredProjects.Count();

                // Apply pagination
                var pagedProjects = filteredProjects
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var projectDtos = new List<ProjectDto>();
                foreach (var project in pagedProjects)
                {
                    var dto = await MapToProjectDto(project);
                    projectDtos.Add(dto);
                }

                var response = new PagedResponse<ProjectDto>
                {
                    Items = projectDtos,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                return Response<PagedResponse<ProjectDto>>.SuccessResponse(response, "Projects retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
                return Response<PagedResponse<ProjectDto>>.FailureResponse(
                    "An error occurred while retrieving projects", 500);
            }
        }

        public async Task<Response<ProjectTaskSummaryDto>> GetProjectTaskSummaryAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectTaskSummaryDto>.FailureResponse("Project not found", 404);
                }

                var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
                var totalTasks = activeTasks.Count;
                var completedTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Completed);
                var inProgressTasks = activeTasks.Count(t => t.Status == TaskItemStatus.InProgress);
                var toDoTasks = activeTasks.Count(t => t.Status == TaskItemStatus.ToDo);
                var cancelledTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Cancelled);

                var summary = new ProjectTaskSummaryDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    InProgressTasks = inProgressTasks,
                    ToDoTasks = toDoTasks,
                    CancelledTasks = cancelledTasks,
                    CompletionPercentage = totalTasks > 0
                        ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                        : 0
                };

                return Response<ProjectTaskSummaryDto>.SuccessResponse(summary, "Project summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project summary for project {ProjectId}", projectId);
                return Response<ProjectTaskSummaryDto>.FailureResponse(
                    "An error occurred while retrieving project summary", 500);
            }
        }

        public async Task<Response<bool>> ToggleProjectStatusAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
                if (project == null)
                {
                    return Response<bool>.FailureResponse("Project not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to modify this project", 403);
                }

                project.IsActive = !project.IsActive;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                var status = project.IsActive ? "activated" : "deactivated";
                _logger.LogInformation("Project {ProjectName} {Status} by user {UserId}", project.Name, status, userId);
                return Response<bool>.SuccessResponse(true, $"Project {status} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling project status for project {ProjectId}", projectId);
                return Response<bool>.FailureResponse("An error occurred while toggling project status", 500);
            }
        }

        #region Private Methods

        private async Task<ProjectDto> MapToProjectDto(Project project)
        {
            var activeTasks = project.Tasks?.Where(t => !t.IsDeleted).ToList() ?? new List<TaskItem>();
            var completedTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Completed);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                IsActive = project.IsActive,
                TaskCount = activeTasks.Count,
                CompletedTasks = completedTasks,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                CreatedByUser = new Application.DTOs.Shared.UserDto
                {
                    Id = project.CreatedByUser?.Id ?? Guid.Empty,
                    Email = project.CreatedByUser?.Email ?? string.Empty,
                    Username = project.CreatedByUser?.Username ?? string.Empty,
                    FirstName = project.CreatedByUser?.FirstName ?? string.Empty,
                    LastName = project.CreatedByUser?.LastName ?? string.Empty,
                    Role = project.CreatedByUser?.Role.ToString() ?? string.Empty
                }
            };
        }

        private IQueryable<Project> ApplySorting(IQueryable<Project> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "startdate" => sortDescending
                    ? query.OrderByDescending(p => p.StartDate)
                    : query.OrderBy(p => p.StartDate),
                "enddate" => sortDescending
                    ? query.OrderByDescending(p => p.EndDate)
                    : query.OrderBy(p => p.EndDate),
                "isactive" => sortDescending
                    ? query.OrderByDescending(p => p.IsActive)
                    : query.OrderBy(p => p.IsActive),
                "taskcount" => sortDescending
                    ? query.OrderByDescending(p => p.Tasks.Count(t => !t.IsDeleted))
                    : query.OrderBy(p => p.Tasks.Count(t => !t.IsDeleted)),
                _ => sortDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt)
            };
        }

        #endregion
    }
}

```

## Project Controller

```cs
// SmartTaskManagement.API/Controllers/ProjectsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;


namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            var userId = GetUserId();
            var result = await _projectService.CreateProjectAsync(createDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto updateDto)
        {
            var userId = GetUserId();
            var result = await _projectService.UpdateProjectAsync(id, updateDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a project (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.DeleteProjectAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectByIdAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all projects with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectsAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get project task summary
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetProjectSummary(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectTaskSummaryAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Toggle project active status
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleProjectStatus(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.ToggleProjectStatusAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }
}
```

## FluentValidation for Project DTOs
```cs
// SmartTaskManagement.Application/Validators/ProjectValidators.cs

using FluentValidation;
using SmartTaskManagement.Application.DTOs.Projects;

namespace SmartTaskManagement.Application.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1))
                .WithMessage("Start date cannot be more than 1 year in the future");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate > x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate > x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class ProjectFilterDtoValidator : AbstractValidator<ProjectFilterDto>
    {
        public ProjectFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) ||
                    new[] { "name", "startdate", "enddate", "isactive", "taskcount", "createdat" }
                        .Contains(x.ToLower()))
                .WithMessage("Invalid sort field");
        }
    }
}
```

## Register Services in Program.cs

Add these services to your Program.cs:
```cs

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectDtoValidator>();

// Register Services
builder.Services.AddScoped<IProjectService, ProjectService>();

// Add AutoMapper (if you want to use it instead of manual mapping)
builder.Services.AddAutoMapper(typeof(Program));

```

## AutoMapper Profile (Optional)

```cs
// SmartTaskManagement.Application/Mappings/MappingProfile.cs
using AutoMapper;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Domain.Entities;

namespace SmartTaskManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.TaskCount,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted)))
                .ForMember(dest => dest.CompletedTasks,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted && t.Status == Domain.Enums.TaskItemStatus.Completed)))
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.MapFrom(src => src.CreatedByUser));

            CreateMap<User, UserDto>();

            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());
        }
    }
}
```
This is the end of Part 8

# Part 9 : Database migration, build and run the project

```bash
# navigate to the Infrastracture project ./SmartTaskManagement.Infrastracture
cd ./SmartTaskManagement.Infrastracture

# Create and apply migrations
dotnet ef migrations add InitialCreate --startup-project ../SmartTaskManagement.API

# remove migrations (if needed use it, otherwise ignore)
dotnet ef migrations remove --startup-project ../SmartTaskManagement.API

# update database
dotnet ef database update --startup-project ../SmartTaskManagement.API

# for dropping database (if you need, otherwise ignore)
dotnet ef database drop

# For build the project navigate to the API project
cd ../SmartTaskManagement.API

# then build
dotnet build

# then run
dotnet run

# then visit the swagger url
http://localhost:5027/swagger/index.html


```

This is the end of Part 9

# Part 10 : Task Management Module
## DTOs

```cs
// SmartTaskManagement.Application/DTOs/Tasks/TaskDto.cs
using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.DTOs.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskItemPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? AssignedToUserId { get; set; }
        public UserDto? AssignedToUser { get; set; }
        public UserDto? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public TaskItemStatus Status { get; set; }
        public string? Comment { get; set; }
    }

    public class AssignTaskDto
    {
        public Guid UserId { get; set; }
    }

    public class TaskFilterDto
    {
        public string? SearchTerm { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public TaskItemStatus? Status { get; set; }
        public TaskItemPriority? Priority { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public bool? IsOverdue { get; set; }
        public bool? ShowOnlyAssignedToMe { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "DueDate";
        public bool SortDescending { get; set; } = false;
    }

    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int ToDoTasks { get; set; }
        public int CancelledTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TasksDueThisWeek { get; set; }
        public Dictionary<TaskItemPriority, int> TasksByPriority { get; set; } = new();
        public Dictionary<TaskItemStatus, int> TasksByStatus { get; set; } = new();
        public double CompletionRate { get; set; }
    }

    public class TaskActivityDto
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? Details { get; set; }
    }
}
```

## Task Service Interface

```cs
// SmartTaskManagement.Application/Interfaces/Services/ITaskService.cs
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.DTOs.Projects;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<Response<TaskDto>> CreateTaskAsync(CreateTaskDto createDto, Guid userId);
        Task<Response<TaskDto>> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateDto, Guid userId);
        Task<Response<bool>> DeleteTaskAsync(Guid taskId, Guid userId);
        Task<Response<TaskDto>> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task<Response<PagedResponse<TaskDto>>> GetTasksAsync(TaskFilterDto filter, Guid userId);
        Task<Response<TaskDto>> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto statusDto, Guid userId);
        Task<Response<TaskDto>> AssignTaskAsync(Guid taskId, AssignTaskDto assignDto, Guid userId);
        Task<Response<TaskStatisticsDto>> GetTaskStatisticsAsync(Guid? projectId = null, Guid? userId = null);
        Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync(Guid userId);
        Task<Response<IEnumerable<TaskDto>>> GetTasksDueSoonAsync(Guid userId, int daysThreshold = 7);
        Task<Response<bool>> BulkUpdateStatusAsync(List<Guid> taskIds, TaskStatus newStatus, Guid userId);
        Task<Response<IEnumerable<TaskActivityDto>>> GetTaskActivityLogAsync(Guid taskId, Guid userId);
    }
}
```

## Task Service Implementation

```cs
// SmartTaskManagement.Infrastructure/Services/TaskService.cs
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IUnitOfWork unitOfWork, ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<TaskDto>> CreateTaskAsync(CreateTaskDto createDto, Guid userId)
        {
            try
            {
                // Validate project exists
                var project = await _unitOfWork.Projects.GetByIdAsync(createDto.ProjectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission to create tasks in this project
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to create tasks in this project", 403);
                }

                // Validate assigned user if provided
                if (createDto.AssignedToUserId.HasValue)
                {
                    var assignedUser = await _unitOfWork.Users.GetByIdAsync(createDto.AssignedToUserId.Value);
                    if (assignedUser == null || assignedUser.IsDeleted)
                    {
                        return Response<TaskDto>.FailureResponse("Assigned user not found", 404);
                    }
                }

                // Validate due date
                if (createDto.DueDate < DateTime.UtcNow.Date)
                {
                    return Response<TaskDto>.FailureResponse("Due date cannot be in the past", 400);
                }

                // Create task
                var task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = createDto.Title,
                    Description = createDto.Description,
                    Status = TaskItemStatus.ToDo,
                    Priority = createDto.Priority,
                    DueDate = createDto.DueDate,
                    EstimatedHours = createDto.EstimatedHours,
                    ActualHours = 0,
                    ProjectId = createDto.ProjectId,
                    AssignedToUserId = createDto.AssignedToUserId,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.CompleteAsync();

                // Get the complete task with navigation properties
                var createdTask = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
                var taskDto = await MapToTaskDto(createdTask!);

                _logger.LogInformation("Task {TaskTitle} created in project {ProjectName} by user {UserId}",
                    task.Title, project.Name, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task by user {UserId}", userId);
                return Response<TaskDto>.FailureResponse("An error occurred while creating the task", 500);
            }
        }

        public async Task<Response<TaskDto>> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission
                if (!await HasTaskPermission(task, userId, user.Role))
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to update this task", 403);
                }

                // Validate assigned user if provided
                if (updateDto.AssignedToUserId.HasValue)
                {
                    var assignedUser = await _unitOfWork.Users.GetByIdAsync(updateDto.AssignedToUserId.Value);
                    if (assignedUser == null || assignedUser.IsDeleted)
                    {
                        return Response<TaskDto>.FailureResponse("Assigned user not found", 404);
                    }
                }

                // Validate due date
                if (updateDto.DueDate < DateTime.UtcNow.Date &&
                    task.Status != TaskItemStatus.Completed &&
                    task.Status != TaskItemStatus.Cancelled)
                {
                    return Response<TaskDto>.FailureResponse("Due date cannot be in the past for active tasks", 400);
                }

                // Update task
                task.Title = updateDto.Title;
                task.Description = updateDto.Description;
                task.Priority = updateDto.Priority;
                task.DueDate = updateDto.DueDate;
                task.EstimatedHours = updateDto.EstimatedHours;
                task.ActualHours = updateDto.ActualHours;
                task.AssignedToUserId = updateDto.AssignedToUserId;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} updated by user {UserId}", task.Title, userId);
                return Response<TaskDto>.SuccessResponse(taskDto, "Task updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while updating the task", 500);
            }
        }

        public async Task<Response<bool>> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                // Check permission
                if (!await HasTaskPermission(task, userId, user.Role))
                {
                    return Response<bool>.FailureResponse(
                        "You don't have permission to delete this task", 403);
                }

                // Soft delete
                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Task {TaskTitle} deleted by user {UserId}", task.Title, userId);
                return Response<bool>.SuccessResponse(true, "Task deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId} by user {UserId}", taskId, userId);
                return Response<bool>.FailureResponse("An error occurred while deleting the task", 500);
            }
        }

        public async Task<Response<TaskDto>> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission - anyone can view if they're part of the project
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    task.AssignedToUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to view this task", 403);
                }

                var taskDto = await MapToTaskDto(task);
                return Response<TaskDto>.SuccessResponse(taskDto, "Task retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while retrieving the task", 500);
            }
        }

        public async Task<Response<PagedResponse<TaskDto>>> GetTasksAsync(TaskFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PagedResponse<TaskDto>>.FailureResponse("User not found", 404);
                }

                // Get all tasks
                var allTasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
                var query = allTasks.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(t =>
                        t.Title.Contains(filter.SearchTerm) ||
                        t.Description.Contains(filter.SearchTerm));
                }

                if (filter.ProjectId.HasValue)
                {
                    query = query.Where(t => t.ProjectId == filter.ProjectId.Value);
                }

                if (filter.AssignedToUserId.HasValue)
                {
                    query = query.Where(t => t.AssignedToUserId == filter.AssignedToUserId.Value);
                }

                if (filter.Status.HasValue)
                {
                    query = query.Where(t => t.Status == filter.Status.Value);
                }

                if (filter.Priority.HasValue)
                {
                    query = query.Where(t => t.Priority == filter.Priority.Value);
                }

                if (filter.DueDateFrom.HasValue)
                {
                    query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);
                }

                if (filter.DueDateTo.HasValue)
                {
                    query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);
                }

                if (filter.IsOverdue.HasValue && filter.IsOverdue.Value)
                {
                    query = query.Where(t =>
                        t.DueDate < DateTime.UtcNow &&
                        t.Status != TaskItemStatus.Completed &&
                        t.Status != TaskItemStatus.Cancelled);
                }

                // Apply user filter for non-admins
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    // Team members can only see tasks assigned to them or tasks from projects they created
                    query = query.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                if (filter.ShowOnlyAssignedToMe.HasValue && filter.ShowOnlyAssignedToMe.Value)
                {
                    query = query.Where(t => t.AssignedToUserId == userId);
                }

                // Get total count before pagination
                var totalCount = query.Count();

                // Apply sorting
                query = ApplyTaskSorting(query, filter.SortBy, filter.SortDescending);

                // Apply pagination
                var pagedTasks = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var taskDtos = new List<TaskDto>();
                foreach (var task in pagedTasks)
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                var response = new PagedResponse<TaskDto>
                {
                    Items = taskDtos,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                return Response<PagedResponse<TaskDto>>.SuccessResponse(response, "Tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
                return Response<PagedResponse<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving tasks", 500);
            }
        }

        public async Task<Response<TaskDto>> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto statusDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check if user has permission to update status
                // Assigned user, creator, project manager, or admin can update status
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (task.AssignedToUserId != userId &&
                    task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to update this task's status", 403);
                }

                // Validate status transition
                if (!IsValidStatusTransition(task.Status, statusDto.Status))
                {
                    return Response<TaskDto>.FailureResponse(
                        $"Invalid status transition from {task.Status} to {statusDto.Status}", 400);
                }

                // Update status
                var oldStatus = task.Status;
                task.Status = statusDto.Status;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                // If task is completed, set actual hours if not set
                if (statusDto.Status == TaskItemStatus.Completed && task.ActualHours == 0)
                {
                    task.ActualHours = task.EstimatedHours;
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} status changed from {OldStatus} to {NewStatus} by user {UserId}",
                    task.Title, oldStatus, statusDto.Status, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId} status by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while updating task status", 500);
            }
        }

        public async Task<Response<TaskDto>> AssignTaskAsync(Guid taskId, AssignTaskDto assignDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission (Admin, Project Manager, or Task Creator can assign)
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to assign this task", 403);
                }

                // Validate user to assign
                var assignUser = await _unitOfWork.Users.GetByIdAsync(assignDto.UserId);
                if (assignUser == null || assignUser.IsDeleted || !assignUser.IsActive)
                {
                    return Response<TaskDto>.FailureResponse("User to assign not found or inactive", 404);
                }

                // Assign task
                task.AssignedToUserId = assignDto.UserId;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                // If task was in ToDo, move to InProgress
                if (task.Status == TaskItemStatus.ToDo)
                {
                    task.Status = TaskItemStatus.InProgress;
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} assigned to user {AssignUser} by {UserId}",
                    task.Title, assignUser.Username, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while assigning the task", 500);
            }
        }

        public async Task<Response<TaskStatisticsDto>> GetTaskStatisticsAsync(Guid? projectId = null, Guid? userId = null)
        {
            try
            {
                var statistics = new TaskStatisticsDto();

                // Get tasks based on filters
                var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
                var filteredTasks = tasks.AsQueryable();

                if (projectId.HasValue)
                {
                    filteredTasks = filteredTasks.Where(t => t.ProjectId == projectId.Value);
                }

                if (userId.HasValue)
                {
                    filteredTasks = filteredTasks.Where(t => t.AssignedToUserId == userId.Value);
                }

                var taskList = filteredTasks.ToList();

                // Calculate statistics
                statistics.TotalTasks = taskList.Count;
                statistics.CompletedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
                statistics.InProgressTasks = taskList.Count(t => t.Status == TaskItemStatus.InProgress);
                statistics.ToDoTasks = taskList.Count(t => t.Status == TaskItemStatus.ToDo);
                statistics.CancelledTasks = taskList.Count(t => t.Status == TaskItemStatus.Cancelled);
                statistics.OverdueTasks = taskList.Count(t =>
                    t.DueDate < DateTime.UtcNow &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);
                statistics.TasksDueThisWeek = taskList.Count(t =>
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(7) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);

                statistics.CompletionRate = statistics.TotalTasks > 0
                    ? Math.Round((double)statistics.CompletedTasks / statistics.TotalTasks * 100, 2)
                    : 0;

                // Group by priority
                statistics.TasksByPriority = taskList
                    .GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all priorities are present
                foreach (TaskItemPriority priority in Enum.GetValues(typeof(TaskItemPriority)))
                {
                    if (!statistics.TasksByPriority.ContainsKey(priority))
                    {
                        statistics.TasksByPriority[priority] = 0;
                    }
                }

                // Group by status
                statistics.TasksByStatus = taskList
                    .GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all statuses are present
                foreach (TaskItemStatus status in Enum.GetValues(typeof(TaskItemStatus)))
                {
                    if (!statistics.TasksByStatus.ContainsKey(status))
                    {
                        statistics.TasksByStatus[status] = 0;
                    }
                }

                return Response<TaskStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task statistics");
                return Response<TaskStatisticsDto>.FailureResponse(
                    "An error occurred while retrieving statistics", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync(Guid userId)
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();

                // Filter for user if not admin
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskDto>>.FailureResponse("User not found", 404);
                }

                var filteredTasks = tasks.AsEnumerable();
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    filteredTasks = filteredTasks.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                var taskDtos = new List<TaskDto>();
                foreach (var task in filteredTasks)
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                return Response<IEnumerable<TaskDto>>.SuccessResponse(
                    taskDtos, "Overdue tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue tasks for user {UserId}", userId);
                return Response<IEnumerable<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving overdue tasks", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetTasksDueSoonAsync(Guid userId, int daysThreshold = 7)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskDto>>.FailureResponse("User not found", 404);
                }

                var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                    !t.IsDeleted &&
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(daysThreshold) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);

                // Filter for user if not admin
                var filteredTasks = tasks.AsEnumerable();
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    filteredTasks = filteredTasks.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                var taskDtos = new List<TaskDto>();
                foreach (var task in filteredTasks.OrderBy(t => t.DueDate))
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                return Response<IEnumerable<TaskDto>>.SuccessResponse(
                    taskDtos, $"Tasks due in next {daysThreshold} days retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks due soon for user {UserId}", userId);
                return Response<IEnumerable<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving tasks due soon", 500);
            }
        }

        public async Task<Response<bool>> BulkUpdateStatusAsync(List<Guid> taskIds, TaskItemStatus newStatus, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                var updatedCount = 0;
                var errors = new List<string>();

                foreach (var taskId in taskIds)
                {
                    try
                    {
                        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                        if (task == null || task.IsDeleted)
                        {
                            errors.Add($"Task {taskId} not found");
                            continue;
                        }

                        // Check permission
                        if (!await HasTaskPermission(task, userId, user.Role))
                        {
                            errors.Add($"Task {taskId} - permission denied");
                            continue;
                        }

                        // Validate status transition
                        if (!IsValidStatusTransition(task.Status, newStatus))
                        {
                            errors.Add($"Task {taskId} - invalid status transition from {task.Status} to {newStatus}");
                            continue;
                        }

                        task.Status = newStatus;
                        task.UpdatedAt = DateTime.UtcNow;
                        task.UpdatedBy = user.Username;

                        _unitOfWork.Tasks.Update(task);
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Task {taskId} - {ex.Message}");
                    }
                }

                await _unitOfWork.CompleteAsync();

                var message = $"Updated {updatedCount} out of {taskIds.Count} tasks";
                if (errors.Any())
                {
                    message += $". Errors: {string.Join("; ", errors)}";
                }

                _logger.LogInformation("Bulk status update completed: {Message}", message);
                return Response<bool>.SuccessResponse(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk status update by user {UserId}", userId);
                return Response<bool>.FailureResponse("An error occurred during bulk update", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskActivityDto>>> GetTaskActivityLogAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("User not found", 404);
                }

                // Check permission
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("Project not found", 404);
                }

                if (task.AssignedToUserId != userId &&
                    task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse(
                        "You don't have permission to view this task's activity", 403);
                }

                // In a real implementation, you would have a separate activity log table
                // For now, we'll return a simulated activity log based on task history
                var activities = new List<TaskActivityDto>
            {
                new TaskActivityDto
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    Action = "Created",
                    PerformedBy = task.CreatedByUser?.Username ?? "Unknown",
                    PerformedAt = task.CreatedAt,
                    Details = $"Task created with priority {task.Priority}"
                }
            };

                // Add status change activities if we had tracking
                // This is a placeholder - you would need to implement proper activity tracking

                return Response<IEnumerable<TaskActivityDto>>.SuccessResponse(
                    activities, "Task activity log retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity log for task {TaskId}", taskId);
                return Response<IEnumerable<TaskActivityDto>>.FailureResponse(
                    "An error occurred while retrieving activity log", 500);
            }
        }

        #region Private Methods

        private async Task<TaskDto> MapToTaskDto(TaskItem task)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            var assignedUser = task.AssignedToUserId.HasValue
                ? await _unitOfWork.Users.GetByIdAsync(task.AssignedToUserId.Value)
                : null;
            var createdByUser = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);

            var isOverdue = task.DueDate < DateTime.UtcNow &&
                            task.Status != TaskItemStatus.Completed &&
                            task.Status != TaskItemStatus.Cancelled;

            var daysUntilDue = (int)(task.DueDate - DateTime.UtcNow.Date).TotalDays;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                ProjectId = task.ProjectId,
                ProjectName = project?.Name ?? string.Empty,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUser = assignedUser != null ? new UserDto
                {
                    Id = assignedUser.Id,
                    Email = assignedUser.Email,
                    Username = assignedUser.Username,
                    FirstName = assignedUser.FirstName,
                    LastName = assignedUser.LastName,
                    Role = assignedUser.Role.ToString()
                } : null,
                CreatedByUser = createdByUser != null ? new UserDto
                {
                    Id = createdByUser.Id,
                    Email = createdByUser.Email,
                    Username = createdByUser.Username,
                    FirstName = createdByUser.FirstName,
                    LastName = createdByUser.LastName,
                    Role = createdByUser.Role.ToString()
                } : null,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsOverdue = isOverdue,
                DaysUntilDue = daysUntilDue
            };
        }

        private async Task<bool> HasTaskPermission(TaskItem task, Guid userId, UserRole userRole)
        {
            if (userRole == UserRole.Admin)
                return true;

            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            if (project == null)
                return false;

            // Project Manager can manage all tasks in their projects
            if (userRole == UserRole.ProjectManager && project.CreatedByUserId == userId)
                return true;

            // Task creator can manage their tasks
            if (task.CreatedByUserId == userId)
                return true;

            // Assigned user can update their tasks (except delete)
            if (task.AssignedToUserId == userId)
                return true;

            return false;
        }

        private bool IsValidStatusTransition(TaskItemStatus currentStatus, TaskItemStatus newStatus)
        {
            // Allow same status
            if (currentStatus == newStatus)
                return true;

            // Define valid transitions
            return (currentStatus, newStatus) switch
            {
                // From ToDo
                (TaskItemStatus.ToDo, TaskItemStatus.InProgress) => true,
                (TaskItemStatus.ToDo, TaskItemStatus.Completed) => true,
                (TaskItemStatus.ToDo, TaskItemStatus.Cancelled) => true,

                // From InProgress
                (TaskItemStatus.InProgress, TaskItemStatus.Completed) => true,
                (TaskItemStatus.InProgress, TaskItemStatus.Cancelled) => true,

                // From Completed
                (TaskItemStatus.Completed, TaskItemStatus.InProgress) => true, // Can reopen
                (TaskItemStatus.Completed, TaskItemStatus.Cancelled) => true,

                // From Cancelled
                (TaskItemStatus.Cancelled, TaskItemStatus.ToDo) => true, // Can revive
                (TaskItemStatus.Cancelled, TaskItemStatus.InProgress) => true,

                _ => false
            };
        }

        private IQueryable<TaskItem> ApplyTaskSorting(IQueryable<TaskItem> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),
                "status" => sortDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),
                "priority" => sortDescending
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority),
                "duedate" => sortDescending
                    ? query.OrderByDescending(t => t.DueDate)
                    : query.OrderBy(t => t.DueDate),
                "estimatedhours" => sortDescending
                    ? query.OrderByDescending(t => t.EstimatedHours)
                    : query.OrderBy(t => t.EstimatedHours),
                "actualhours" => sortDescending
                    ? query.OrderByDescending(t => t.ActualHours)
                    : query.OrderBy(t => t.ActualHours),
                "projectname" => sortDescending
                    ? query.OrderByDescending(t => t.Project.Name)
                    : query.OrderBy(t => t.Project.Name),
                "assignedto" => sortDescending
                    ? query.OrderByDescending(t => t.AssignedToUser.Username)
                    : query.OrderBy(t => t.AssignedToUser.Username),
                _ => sortDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
            };
        }

        #endregion
    }
}

```

## Task Controller

```cs
// SmartTaskManagement.API/Controllers/TasksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createDto)
        {
            var userId = GetUserId();
            var result = await _taskService.CreateTaskAsync(createDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateDto)
        {
            var userId = GetUserId();
            var result = await _taskService.UpdateTaskAsync(id, updateDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a task (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.DeleteTaskAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTaskByIdAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all tasks with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTasksAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusDto statusDto)
        {
            var userId = GetUserId();
            var result = await _taskService.UpdateTaskStatusAsync(id, statusDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Assign task to a user
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskDto assignDto)
        {
            var userId = GetUserId();
            var result = await _taskService.AssignTaskAsync(id, assignDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetTaskStatistics([FromQuery] Guid? projectId = null, [FromQuery] Guid? userId = null)
        {
            var result = await _taskService.GetTaskStatisticsAsync(projectId, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var userId = GetUserId();
            var result = await _taskService.GetOverdueTasksAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get tasks due soon
        /// </summary>
        [HttpGet("due-soon")]
        public async Task<IActionResult> GetTasksDueSoon([FromQuery] int daysThreshold = 7)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTasksDueSoonAsync(userId, daysThreshold);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Bulk update task statuses
        /// </summary>
        [HttpPatch("bulk-status")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto bulkDto)
        {
            var userId = GetUserId();
            var result = await _taskService.BulkUpdateStatusAsync(bulkDto.TaskIds, bulkDto.NewStatus, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task activity log
        /// </summary>
        [HttpGet("{id}/activity")]
        public async Task<IActionResult> GetTaskActivity(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTaskActivityLogAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }

    // DTO for bulk status update
    public class BulkUpdateStatusDto
    {
        public List<Guid> TaskIds { get; set; } = new();
        public TaskItemStatus NewStatus { get; set; }
    }
}
```

## FluentValidation for Task DTOs
```cs
// SmartTaskManagement.Application/Validators/TaskValidators.cs
using FluentValidation;
using SmartTaskManagement.Application.DTOs.Tasks;


namespace SmartTaskManagement.Application.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Estimated hours cannot exceed 999");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");
        }
    }

    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Estimated hours cannot exceed 999");

            RuleFor(x => x.ActualHours)
                .GreaterThanOrEqualTo(0).WithMessage("Actual hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Actual hours cannot exceed 999");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");
        }
    }

    public class UpdateTaskStatusDtoValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        public UpdateTaskStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
        }
    }

    public class TaskFilterDtoValidator : AbstractValidator<TaskFilterDto>
    {
        public TaskFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) ||
                    new[] { "title", "status", "priority", "duedate", "estimatedhours",
                        "actualhours", "projectname", "assignedto", "createdat" }
                        .Contains(x.ToLower()))
                .WithMessage("Invalid sort field");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid status value");

            RuleFor(x => x.Priority)
                .IsInEnum().When(x => x.Priority.HasValue)
                .WithMessage("Invalid priority value");
        }
    }
}

```
## update program.cs file
Add the task service registration:
```cs
// In Program.cs, add:
builder.Services.AddScoped<ITaskService, TaskService>();

```

## Database Migration Build and Run

```bash
# navigate to the Infrastracture project ./SmartTaskManagement.Infrastracture
cd ./SmartTaskManagement.Infrastracture

# Create and apply migrations
dotnet ef migrations add TaskModule --startup-project ../SmartTaskManagement.API

# update database
dotnet ef database update --startup-project ../SmartTaskManagement.API

# For build the project navigate to the API project
cd ../SmartTaskManagement.API

# then build
dotnet build

# then run
dotnet run
```

This is the end of Part 10

# Part 11 : Dashboard Module
## DTOs
```cs
// SmartTaskManagement.Application/DTOs/Dashboard/DashboardDto.cs
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public ProjectStatisticsDto ProjectStatistics { get; set; } = new();
        public TaskStatisticsDto TaskStatistics { get; set; } = new();
        public UserStatisticsDto UserStatistics { get; set; } = new();
        public ActivitySummaryDto RecentActivity { get; set; } = new();
        public PerformanceMetricsDto PerformanceMetrics { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProjectStatisticsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int InactiveProjects { get; set; }
        public int ProjectsCompleted { get; set; }
        public int ProjectsInProgress { get; set; }
        public double ProjectCompletionRate { get; set; }
        public List<ProjectProgressDto> ProjectProgress { get; set; } = new();
        public Dictionary<DateTime, int> ProjectsByMonth { get; set; } = new();
    }

    public class ProjectProgressDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public double Progress { get; set; } // Percentage
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int ToDoTasks { get; set; }
        public int CancelledTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TasksDueThisWeek { get; set; }
        public int TasksDueNextWeek { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; } // In days
        public Dictionary<TaskItemStatus, int> TasksByStatus { get; set; } = new();
        public Dictionary<TaskItemPriority, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, int> TasksByAssignee { get; set; } = new();
        public List<TaskTrendDto> TaskTrends { get; set; } = new();
    }

    public class TaskTrendDto
    {
        public DateTime Period { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
    }

    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public Dictionary<string, int> UsersByActivity { get; set; } = new();
        public List<UserPerformanceDto> TopPerformers { get; set; } = new();
    }

    public class UserPerformanceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; }
        public double ProductivityScore { get; set; }
    }

    public class ActivitySummaryDto
    {
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public Dictionary<string, int> ActivityTypes { get; set; } = new();
        public Dictionary<DateTime, int> ActivityTimeline { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class PerformanceMetricsDto
    {
        public double OverallProductivity { get; set; }
        public double ProjectSuccessRate { get; set; }
        public double TaskEfficiency { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public double ResourceUtilization { get; set; }
        public Dictionary<string, double> MetricsByProject { get; set; } = new();
        public List<KeyValuePair<string, double>> MetricsTrend { get; set; } = new();
    }

    public class DashboardFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public int DaysToShow { get; set; } = 30;
    }
}

```

## Dashboard Service Interface

```cs
// SmartTaskManagement.Application/Interfaces/Services/IDashboardService.cs
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Dashboard;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(DashboardFilterDto filter, Guid userId);
        Task<Response<ProjectProgressDto>> GetProjectProgressAsync(Guid projectId, Guid userId);
        Task<Response<IEnumerable<ProjectProgressDto>>> GetAllProjectsProgressAsync(Guid userId);
        Task<Response<UserPerformanceDto>> GetUserPerformanceAsync(Guid userId, Guid requestingUserId);
        Task<Response<IEnumerable<UserPerformanceDto>>> GetTeamPerformanceAsync(Guid requestingUserId);
        Task<Response<PerformanceMetricsDto>> GetPerformanceMetricsAsync(DashboardFilterDto filter, Guid userId);
        Task<Response<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10, Guid? userId = null);
    }
}
```

## Dashboard Service Implementation
```cs
// SmartTaskManagement.Infrastructure/Services/DashboardService.cs
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Dashboard;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(DashboardFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<DashboardSummaryDto>.FailureResponse("User not found", 404);
                }

                // Get data based on user role
                var isAdmin = user.Role == UserRole.Admin;
                var isManager = user.Role == UserRole.ProjectManager;

                // Get projects based on permissions
                var projects = await GetAccessibleProjects(user);

                // Apply date filter
                if (filter.StartDate.HasValue)
                {
                    projects = projects.Where(p => p.CreatedAt >= filter.StartDate.Value);
                }
                if (filter.EndDate.HasValue)
                {
                    projects = projects.Where(p => p.CreatedAt <= filter.EndDate.Value);
                }

                // Get all tasks
                var tasks = new List<TaskItem>();
                foreach (var project in projects)
                {
                    var projectTasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
                    tasks.AddRange(projectTasks.Where(t => !t.IsDeleted));
                }

                // Build dashboard summary
                var dashboard = new DashboardSummaryDto
                {
                    ProjectStatistics = await GetProjectStatisticsAsync(projects, tasks),
                    TaskStatistics = await GetTaskStatisticsAsync(tasks, projects),
                    UserStatistics = await GetUserStatisticsAsync(user),
                    RecentActivity = await GetActivitySummaryAsync(tasks, user),
                    PerformanceMetrics = await CalculatePerformanceMetrics(projects, tasks, user),
                    GeneratedAt = DateTime.UtcNow
                };

                return Response<DashboardSummaryDto>.SuccessResponse(dashboard, "Dashboard summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard summary for user {UserId}", userId);
                return Response<DashboardSummaryDto>.FailureResponse(
                    "An error occurred while generating dashboard summary", 500);
            }
        }

        public async Task<Response<ProjectProgressDto>> GetProjectProgressAsync(Guid projectId, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectProgressDto>.FailureResponse("User not found", 404);
                }

                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<ProjectProgressDto>.FailureResponse("Project not found", 404);
                }

                // Check permissions
                if (!await HasProjectAccess(project, user))
                {
                    return Response<ProjectProgressDto>.FailureResponse(
                        "You don't have permission to view this project", 403);
                }

                var progress = await CalculateProjectProgress(project);
                return Response<ProjectProgressDto>.SuccessResponse(progress, "Project progress retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project progress for project {ProjectId}", projectId);
                return Response<ProjectProgressDto>.FailureResponse(
                    "An error occurred while retrieving project progress", 500);
            }
        }

        public async Task<Response<IEnumerable<ProjectProgressDto>>> GetAllProjectsProgressAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<ProjectProgressDto>>.FailureResponse("User not found", 404);
                }

                var projects = await GetAccessibleProjects(user);
                var progressList = new List<ProjectProgressDto>();

                foreach (var project in projects)
                {
                    var progress = await CalculateProjectProgress(project);
                    progressList.Add(progress);
                }

                return Response<IEnumerable<ProjectProgressDto>>.SuccessResponse(
                    progressList, "Projects progress retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all projects progress for user {UserId}", userId);
                return Response<IEnumerable<ProjectProgressDto>>.FailureResponse(
                    "An error occurred while retrieving projects progress", 500);
            }
        }

        public async Task<Response<UserPerformanceDto>> GetUserPerformanceAsync(Guid userId, Guid requestingUserId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return Response<UserPerformanceDto>.FailureResponse("User not found", 404);
                }

                var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
                if (requestingUser == null)
                {
                    return Response<UserPerformanceDto>.FailureResponse("Requesting user not found", 404);
                }

                // Check permissions
                if (requestingUser.Role != UserRole.Admin &&
                    requestingUser.Role != UserRole.ProjectManager &&
                    requestingUserId != userId)
                {
                    return Response<UserPerformanceDto>.FailureResponse(
                        "You don't have permission to view this user's performance", 403);
                }

                var performance = await CalculateUserPerformance(user);
                return Response<UserPerformanceDto>.SuccessResponse(performance, "User performance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance for user {UserId}", userId);
                return Response<UserPerformanceDto>.FailureResponse(
                    "An error occurred while retrieving user performance", 500);
            }
        }

        public async Task<Response<IEnumerable<UserPerformanceDto>>> GetTeamPerformanceAsync(Guid requestingUserId)
        {
            try
            {
                var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
                if (requestingUser == null)
                {
                    return Response<IEnumerable<UserPerformanceDto>>.FailureResponse("User not found", 404);
                }

                // Only admins and project managers can view team performance
                if (requestingUser.Role != UserRole.Admin && requestingUser.Role != UserRole.ProjectManager)
                {
                    return Response<IEnumerable<UserPerformanceDto>>.FailureResponse(
                        "You don't have permission to view team performance", 403);
                }

                // Get all active users
                var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted && u.IsActive);
                var performanceList = new List<UserPerformanceDto>();

                foreach (var user in users)
                {
                    // For project managers, only show team members (not admins)
                    if (requestingUser.Role == UserRole.ProjectManager && user.Role == UserRole.Admin)
                    {
                        continue;
                    }

                    var performance = await CalculateUserPerformance(user);
                    performanceList.Add(performance);
                }

                // Order by productivity score
                performanceList = performanceList.OrderByDescending(p => p.ProductivityScore).ToList();

                return Response<IEnumerable<UserPerformanceDto>>.SuccessResponse(
                    performanceList, "Team performance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team performance for user {UserId}", requestingUserId);
                return Response<IEnumerable<UserPerformanceDto>>.FailureResponse(
                    "An error occurred while retrieving team performance", 500);
            }
        }

        public async Task<Response<PerformanceMetricsDto>> GetPerformanceMetricsAsync(DashboardFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PerformanceMetricsDto>.FailureResponse("User not found", 404);
                }

                var projects = await GetAccessibleProjects(user);
                var tasks = new List<TaskItem>();

                foreach (var project in projects)
                {
                    var projectTasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
                    tasks.AddRange(projectTasks.Where(t => !t.IsDeleted));
                }

                var metrics = await CalculatePerformanceMetrics(projects, tasks, user);
                return Response<PerformanceMetricsDto>.SuccessResponse(metrics, "Performance metrics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance metrics for user {UserId}", userId);
                return Response<PerformanceMetricsDto>.FailureResponse(
                    "An error occurred while retrieving performance metrics", 500);
            }
        }

        public async Task<Response<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10, Guid? userId = null)
        {
            try
            {
                var activities = new List<RecentActivityDto>();

                // Get recent tasks
                var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);

                if (userId.HasValue)
                {
                    tasks = tasks.Where(t => t.AssignedToUserId == userId.Value || t.CreatedByUserId == userId.Value);
                }

                var recentTasks = tasks
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(count)
                    .ToList();

                foreach (var task in recentTasks)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);
                    var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);

                    activities.Add(new RecentActivityDto
                    {
                        Timestamp = task.UpdatedAt,
                        User = user?.Username ?? "Unknown",
                        Action = task.CreatedAt == task.UpdatedAt ? "Created" : "Updated",
                        Entity = "Task",
                        EntityName = task.Title,
                        Details = $"Status: {task.Status}, Priority: {task.Priority}"
                    });
                }

                // Get recent projects
                var projects = await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);

                if (userId.HasValue)
                {
                    projects = projects.Where(p => p.CreatedByUserId == userId.Value);
                }

                var recentProjects = projects
                    .OrderByDescending(p => p.UpdatedAt)
                    .Take(count)
                    .ToList();

                foreach (var project in recentProjects)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(project.CreatedByUserId);

                    activities.Add(new RecentActivityDto
                    {
                        Timestamp = project.UpdatedAt,
                        User = user?.Username ?? "Unknown",
                        Action = project.CreatedAt == project.UpdatedAt ? "Created" : "Updated",
                        Entity = "Project",
                        EntityName = project.Name,
                        Details = project.IsActive ? "Active" : "Inactive"
                    });
                }

                // Sort by timestamp and take top count
                activities = activities
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .ToList();

                return Response<IEnumerable<RecentActivityDto>>.SuccessResponse(
                    activities, "Recent activities retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return Response<IEnumerable<RecentActivityDto>>.FailureResponse(
                    "An error occurred while retrieving recent activities", 500);
            }
        }

        #region Private Methods

        private async Task<ProjectStatisticsDto> GetProjectStatisticsAsync(IEnumerable<Project> projects, List<TaskItem> tasks)
        {
            var projectList = projects.ToList();
            var activeProjects = projectList.Count(p => p.IsActive);
            var inactiveProjects = projectList.Count(p => !p.IsActive);

            // Calculate project completion
            var completedProjects = 0;
            var inProgressProjects = 0;
            var projectProgress = new List<ProjectProgressDto>();

            foreach (var project in projectList)
            {
                var projectTasks = tasks.Where(t => t.ProjectId == project.Id).ToList();
                var totalTasks = projectTasks.Count;
                var completedTasks = projectTasks.Count(t => t.Status == TaskItemStatus.Completed);

                var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

                if (progress == 100 && totalTasks > 0)
                {
                    completedProjects++;
                }
                else if (progress > 0 && progress < 100)
                {
                    inProgressProjects++;
                }

                projectProgress.Add(new ProjectProgressDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    Progress = Math.Round(progress, 2),
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    EndDate = project.EndDate,
                    IsOverdue = project.EndDate.HasValue && project.EndDate < DateTime.UtcNow && progress < 100
                });
            }

            // Group projects by month
            var projectsByMonth = projectList
                .GroupBy(p => new DateTime(p.CreatedAt.Year, p.CreatedAt.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return new ProjectStatisticsDto
            {
                TotalProjects = projectList.Count,
                ActiveProjects = activeProjects,
                InactiveProjects = inactiveProjects,
                ProjectsCompleted = completedProjects,
                ProjectsInProgress = inProgressProjects,
                ProjectCompletionRate = projectList.Count > 0
                    ? Math.Round((double)completedProjects / projectList.Count * 100, 2)
                    : 0,
                ProjectProgress = projectProgress,
                ProjectsByMonth = projectsByMonth
            };
        }

        private async Task<TaskStatisticsDto> GetTaskStatisticsAsync(List<TaskItem> tasks, IEnumerable<Project> projects)
        {
            var taskList = tasks.ToList();
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
            var inProgressTasks = taskList.Count(t => t.Status == TaskItemStatus.InProgress);
            var toDoTasks = taskList.Count(t => t.Status == TaskItemStatus.ToDo);
            var cancelledTasks = taskList.Count(t => t.Status == TaskItemStatus.Cancelled);
            var overdueTasks = taskList.Count(t =>
                t.DueDate < DateTime.UtcNow &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled);

            // Calculate average completion time
            var completedTaskList = taskList.Where(t => t.Status == TaskItemStatus.Completed).ToList();
            var avgCompletionTime = 0.0;
            if (completedTaskList.Any())
            {
                var totalDays = completedTaskList.Sum(t => (t.UpdatedAt - t.CreatedAt).TotalDays);
                avgCompletionTime = Math.Round(totalDays / completedTaskList.Count, 2);
            }

            // Tasks by assignee
            var tasksByAssignee = taskList
                .Where(t => t.AssignedToUserId.HasValue)
                .GroupBy(t => t.AssignedToUserId!.Value)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            // Get assignee names
            var assigneeNames = new Dictionary<string, int>();
            foreach (var kvp in tasksByAssignee)
            {
                if (Guid.TryParse(kvp.Key, out var userId))
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);
                    var userName = user?.Username ?? "Unknown";
                    assigneeNames[userName] = kvp.Value;
                }
            }

            // Task trends (last 30 days)
            var taskTrends = new List<TaskTrendDto>();
            var startDate = DateTime.UtcNow.AddDays(-30);

            for (int i = 0; i < 30; i++)
            {
                var date = startDate.AddDays(i);
                var dayStart = date.Date;
                var dayEnd = dayStart.AddDays(1);

                var created = taskList.Count(t => t.CreatedAt >= dayStart && t.CreatedAt < dayEnd);
                var completed = taskList.Count(t =>
                    t.UpdatedAt >= dayStart &&
                    t.UpdatedAt < dayEnd &&
                    t.Status == TaskItemStatus.Completed);
                var inProgress = taskList.Count(t =>
                    t.UpdatedAt >= dayStart &&
                    t.UpdatedAt < dayEnd &&
                    t.Status == TaskItemStatus.InProgress);

                taskTrends.Add(new TaskTrendDto
                {
                    Period = date,
                    Created = created,
                    Completed = completed,
                    InProgress = inProgress
                });
            }

            return new TaskStatisticsDto
            {
                TotalTasks = taskList.Count,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                ToDoTasks = toDoTasks,
                CancelledTasks = cancelledTasks,
                OverdueTasks = overdueTasks,
                TasksDueThisWeek = taskList.Count(t =>
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(7) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled),
                TasksDueNextWeek = taskList.Count(t =>
                    t.DueDate > DateTime.UtcNow.AddDays(7) &&
                    t.DueDate <= DateTime.UtcNow.AddDays(14) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled),
                CompletionRate = taskList.Count > 0
                    ? Math.Round((double)completedTasks / taskList.Count * 100, 2)
                    : 0,
                AverageCompletionTime = avgCompletionTime,
                TasksByStatus = taskList.GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TasksByPriority = taskList.GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TasksByAssignee = assigneeNames,
                TaskTrends = taskTrends
            };
        }

        private async Task<UserStatisticsDto> GetUserStatisticsAsync(User currentUser)
        {
            var allUsers = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
            var userList = allUsers.ToList();

            var usersByRole = userList
                .GroupBy(u => u.Role.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Users by activity (based on task assignments)
            var usersByActivity = new Dictionary<string, int>();
            var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
            var taskList = tasks.ToList();

            foreach (var user in userList)
            {
                var assignedTasks = taskList.Count(t => t.AssignedToUserId == user.Id);
                var activityLevel = assignedTasks switch
                {
                    >= 10 => "Very Active",
                    >= 5 => "Active",
                    >= 1 => "Moderate",
                    _ => "Inactive"
                };

                if (!usersByActivity.ContainsKey(activityLevel))
                    usersByActivity[activityLevel] = 0;
                usersByActivity[activityLevel]++;
            }

            // Top performers (only if current user is admin or manager)
            var topPerformers = new List<UserPerformanceDto>();
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.ProjectManager)
            {
                foreach (var user in userList)
                {
                    var performance = await CalculateUserPerformance(user);
                    topPerformers.Add(performance);
                }
                topPerformers = topPerformers.OrderByDescending(p => p.ProductivityScore).Take(5).ToList();
            }

            return new UserStatisticsDto
            {
                TotalUsers = userList.Count,
                ActiveUsers = userList.Count(u => u.IsActive),
                InactiveUsers = userList.Count(u => !u.IsActive),
                UsersByRole = usersByRole,
                UsersByActivity = usersByActivity,
                TopPerformers = topPerformers
            };
        }

        private async Task<ActivitySummaryDto> GetActivitySummaryAsync(List<TaskItem> tasks, User currentUser)
        {
            var recentActivities = new List<RecentActivityDto>();

            // Get recent tasks (last 10)
            var recentTasks = tasks
                .OrderByDescending(t => t.UpdatedAt)
                .Take(10)
                .ToList();

            foreach (var task in recentTasks)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);

                recentActivities.Add(new RecentActivityDto
                {
                    Timestamp = task.UpdatedAt,
                    User = user?.Username ?? "Unknown",
                    Action = task.CreatedAt == task.UpdatedAt ? "Created" : "Updated",
                    Entity = "Task",
                    EntityName = task.Title,
                    Details = $"{project?.Name} - {task.Status}"
                });
            }

            // Activity types
            var activityTypes = new Dictionary<string, int>
            {
                ["Task Created"] = tasks.Count(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                ["Task Completed"] = tasks.Count(t =>
                    t.Status == TaskItemStatus.Completed &&
                    t.UpdatedAt >= DateTime.UtcNow.AddDays(-7)),
                ["Task Updated"] = tasks.Count(t =>
                    t.CreatedAt != t.UpdatedAt &&
                    t.UpdatedAt >= DateTime.UtcNow.AddDays(-7))
            };

            // Activity timeline (last 7 days)
            var activityTimeline = new Dictionary<DateTime, int>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i).Date;
                var count = tasks.Count(t => t.UpdatedAt.Date == date);
                activityTimeline[date] = count;
            }

            return new ActivitySummaryDto
            {
                RecentActivities = recentActivities,
                ActivityTypes = activityTypes,
                ActivityTimeline = activityTimeline
            };
        }

        private async Task<PerformanceMetricsDto> CalculatePerformanceMetrics(IEnumerable<Project> projects, List<TaskItem> tasks, User user)
        {
            var projectList = projects.ToList();
            var taskList = tasks.ToList();

            // Overall productivity
            var totalTasks = taskList.Count;
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
            var onTimeTasks = taskList.Count(t =>
                t.Status == TaskItemStatus.Completed &&
                t.UpdatedAt <= t.DueDate);

            var productivity = totalTasks > 0
                ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                : 0;

            var projectSuccessRate = projectList.Count > 0
                ? Math.Round((double)projectList.Count(p =>
                    p.Tasks.Any() &&
                    p.Tasks.Count(t => !t.IsDeleted && t.Status == TaskItemStatus.Completed) == p.Tasks.Count(t => !t.IsDeleted)
                ) / projectList.Count * 100, 2)
                : 0;

            var taskEfficiency = completedTasks > 0
                ? Math.Round((double)onTimeTasks / completedTasks * 100, 2)
                : 0;

            var onTimeDeliveryRate = totalTasks > 0
                ? Math.Round((double)onTimeTasks / totalTasks * 100, 2)
                : 0;

            var resourceUtilization = totalTasks > 0
                ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                : 0;

            // Metrics by project
            var metricsByProject = new Dictionary<string, double>();
            foreach (var project in projectList)
            {
                var projectTasks = taskList.Where(t => t.ProjectId == project.Id).ToList();
                if (projectTasks.Any())
                {
                    var projectCompleted = projectTasks.Count(t => t.Status == TaskItemStatus.Completed);
                    var projectTotal = projectTasks.Count;
                    var projectSuccess = Math.Round((double)projectCompleted / projectTotal * 100, 2);
                    metricsByProject[project.Name] = projectSuccess;
                }
            }

            // Metrics trend (last 30 days)
            var metricsTrend = new List<KeyValuePair<string, double>>();
            for (int i = 30; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                var dailyTasks = taskList.Where(t => t.CreatedAt.Date == date.Date).ToList();
                if (dailyTasks.Any())
                {
                    var dailyCompleted = dailyTasks.Count(t => t.Status == TaskItemStatus.Completed);
                    var dailyProductivity = Math.Round((double)dailyCompleted / dailyTasks.Count * 100, 2);
                    metricsTrend.Add(new KeyValuePair<string, double>(
                        date.ToString("MMM dd"),
                        dailyProductivity
                    ));
                }
            }

            return new PerformanceMetricsDto
            {
                OverallProductivity = productivity,
                ProjectSuccessRate = projectSuccessRate,
                TaskEfficiency = taskEfficiency,
                OnTimeDeliveryRate = onTimeDeliveryRate,
                ResourceUtilization = resourceUtilization,
                MetricsByProject = metricsByProject,
                MetricsTrend = metricsTrend
            };
        }

        private async Task<ProjectProgressDto> CalculateProjectProgress(Project project)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
            var taskList = tasks.Where(t => !t.IsDeleted).ToList();
            var totalTasks = taskList.Count;
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);

            var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

            return new ProjectProgressDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Progress = Math.Round(progress, 2),
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                EndDate = project.EndDate,
                IsOverdue = project.EndDate.HasValue &&
                            project.EndDate < DateTime.UtcNow &&
                            progress < 100
            };
        }

        private async Task<UserPerformanceDto> CalculateUserPerformance(User user)
        {
            var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                !t.IsDeleted &&
                (t.AssignedToUserId == user.Id || t.CreatedByUserId == user.Id));

            var taskList = tasks.ToList();
            var assignedTasks = taskList.Count(t => t.AssignedToUserId == user.Id);
            var completedTasks = taskList.Count(t =>
                t.Status == TaskItemStatus.Completed &&
                t.AssignedToUserId == user.Id);
            var overdueTasks = taskList.Count(t =>
                t.AssignedToUserId == user.Id &&
                t.DueDate < DateTime.UtcNow &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled);

            var completionRate = assignedTasks > 0
                ? Math.Round((double)completedTasks / assignedTasks * 100, 2)
                : 0;

            // Average completion time
            var completedTaskList = taskList
                .Where(t => t.Status == TaskItemStatus.Completed && t.AssignedToUserId == user.Id)
                .ToList();

            var avgCompletionTime = 0.0;
            if (completedTaskList.Any())
            {
                var totalDays = completedTaskList.Sum(t => (t.UpdatedAt - t.CreatedAt).TotalDays);
                avgCompletionTime = Math.Round(totalDays / completedTaskList.Count, 2);
            }

            // Productivity score (weighted calculation)
            var productivityScore = (completionRate * 0.4) +
                                    (Math.Min(assignedTasks / 10.0, 1.0) * 30) +
                                    (Math.Max(0, (100 - (avgCompletionTime * 2))) * 0.3);

            productivityScore = Math.Min(100, productivityScore);

            return new UserPerformanceDto
            {
                UserId = user.Id,
                UserName = user.Username,
                FullName = $"{user.FirstName} {user.LastName}",
                AssignedTasks = assignedTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                CompletionRate = completionRate,
                AverageCompletionTime = avgCompletionTime,
                ProductivityScore = Math.Round(productivityScore, 2)
            };
        }

        private async Task<IEnumerable<Project>> GetAccessibleProjects(User user)
        {
            if (user.Role == UserRole.Admin)
            {
                return await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);
            }
            else if (user.Role == UserRole.ProjectManager)
            {
                return await _unitOfWork.Projects.FindAsync(p =>
                    !p.IsDeleted && p.CreatedByUserId == user.Id);
            }
            else // Team Member
            {
                // Get projects where user is assigned to tasks or created the project
                var allProjects = await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);
                var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                    !t.IsDeleted &&
                    (t.AssignedToUserId == user.Id || t.CreatedByUserId == user.Id));

                var projectIds = tasks.Select(t => t.ProjectId).Distinct();
                return allProjects.Where(p => projectIds.Contains(p.Id));
            }
        }

        private async Task<bool> HasProjectAccess(Project project, User user)
        {
            if (user.Role == UserRole.Admin)
                return true;

            if (user.Role == UserRole.ProjectManager && project.CreatedByUserId == user.Id)
                return true;

            // Check if user is assigned to any task in the project
            var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                !t.IsDeleted &&
                t.ProjectId == project.Id &&
                t.AssignedToUserId == user.Id);

            return tasks.Any();
        }

        #endregion
    }
}
```

## Dashboard Controller

```cs
// SmartTaskManagement.API/Controllers/DashboardController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Dashboard;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete dashboard summary
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DashboardFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetDashboardSummaryAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get progress for a specific project
        /// </summary>
        [HttpGet("projects/{projectId}/progress")]
        public async Task<IActionResult> GetProjectProgress(Guid projectId)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetProjectProgressAsync(projectId, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get progress for all accessible projects
        /// </summary>
        [HttpGet("projects/progress")]
        public async Task<IActionResult> GetAllProjectsProgress()
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetAllProjectsProgressAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get user performance
        /// </summary>
        [HttpGet("users/{userId}/performance")]
        public async Task<IActionResult> GetUserPerformance(Guid userId)
        {
            var requestingUserId = GetUserId();
            var result = await _dashboardService.GetUserPerformanceAsync(userId, requestingUserId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get team performance
        /// </summary>
        [HttpGet("team/performance")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> GetTeamPerformance()
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetTeamPerformanceAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get performance metrics
        /// </summary>
        [HttpGet("metrics")]
        public async Task<IActionResult> GetPerformanceMetrics([FromQuery] DashboardFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetPerformanceMetricsAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get recent activities
        /// </summary>
        [HttpGet("activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10, [FromQuery] Guid? userId = null)
        {
            var result = await _dashboardService.GetRecentActivitiesAsync(count, userId);
            return StatusCode(result.StatusCode, result);
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }
}
```

## Update Program.cs
Add the dashboard service registration:
```cs

// In Program.cs, add:
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

## Database Migration Build and Run

```bash
# navigate to the Infrastracture project ./SmartTaskManagement.Infrastracture
cd ./SmartTaskManagement.Infrastracture

# Create and apply migrations
dotnet ef migrations add DashboardModule --startup-project ../SmartTaskManagement.API

# update database
dotnet ef database update --startup-project ../SmartTaskManagement.API

# For build the project navigate to the API project
cd ../SmartTaskManagement.API

# then build
dotnet build

# then run
dotnet run
```

This is the End of Part 11

# Part 12 : AI Feature / Module
## DTOs
```cs
// SmartTaskManagement.Application/DTOs/AI/TaskImprovementRequestDto.cs
namespace SmartTaskManagement.Application.DTOs.AI
{
    public class TaskImprovementRequestDto
    {
        public string OriginalTitle { get; set; } = string.Empty;
        public string OriginalDescription { get; set; } = string.Empty;
        public string? AdditionalContext { get; set; }
        public ImprovementOptions Options { get; set; } = new();
    }

    public class ImprovementOptions
    {
        public bool CorrectGrammar { get; set; } = true;
        public bool ImproveClarity { get; set; } = true;
        public bool MakeProfessional { get; set; } = true;
        public bool ExpandDescription { get; set; } = true;
        public bool MakeActionable { get; set; } = true;
        public int MaxLength { get; set; } = 500;
        public string Tone { get; set; } = "Professional";
        public string? Language { get; set; } = "English";
    }

    public class TaskImprovementResponseDto
    {
        public string ImprovedTitle { get; set; } = string.Empty;
        public string ImprovedDescription { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public List<string> KeyPoints { get; set; } = new();
        public List<string> SuggestedActions { get; set; } = new();
        public ImprovementMetadata Metadata { get; set; } = new();
    }

    public class ImprovementMetadata
    {
        public string Model { get; set; } = string.Empty;
        public int OriginalLength { get; set; }
        public int ImprovedLength { get; set; }
        public double ProcessingTimeSeconds { get; set; }
        public int TokensUsed { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class AiConfigDto
    {
        public string ApiBaseUrl { get; set; } = "https://models.github.ai/inference/chat/completions";
        public string Model { get; set; } = "openai/gpt-4o-mini";
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 1000;
    }
}
```

```cs
// SmartTaskManagement.Application/DTOs/AI/TaskBulkImprovementDto.cs
namespace SmartTaskManagement.Application.DTOs.AI
{
    public class TaskBulkImprovementDto
    {
        public List<Guid> TaskIds { get; set; } = new();
        public ImprovementOptions Options { get; set; } = new();
    }

    public class TaskBulkImprovementResponseDto
    {
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<TaskImprovementResultDto> Results { get; set; } = new();
    }

    public class TaskImprovementResultDto
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TaskImprovementResponseDto? Improvement { get; set; }
    }
}
```

## AI Service Interface
```cs
// SmartTaskManagement.Application/Interfaces/Services/IAiService.cs
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.AI;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IAiService
    {
        /// <summary>
        /// Improve a single task description using AI
        /// </summary>
        Task<Response<TaskImprovementResponseDto>> ImproveTaskDescriptionAsync(
            TaskImprovementRequestDto request, Guid userId);

        /// <summary>
        /// Improve multiple task descriptions in bulk
        /// </summary>
        Task<Response<TaskBulkImprovementResponseDto>> BulkImproveTaskDescriptionsAsync(
            TaskBulkImprovementDto bulkRequest, Guid userId);

        /// <summary>
        /// Generate a summary for a task
        /// </summary>
        Task<Response<string>> GenerateTaskSummaryAsync(string description, Guid userId);

        /// <summary>
        /// Suggest next actions for a task
        /// </summary>
        Task<Response<List<string>>> SuggestNextActionsAsync(
            string title, string description, string status, Guid userId);

        /// <summary>
        /// Check if AI service is available
        /// </summary>
        Task<Response<bool>> CheckHealthAsync();
    }
}
```

## AI Configuration
```cs
// SmartTaskManagement.Infrastructure/Settings/AiSettings.cs
namespace SmartTaskManagement.Infrastructure.Settings
{
    public class AiSettings
    {
        public string ApiBaseUrl { get; set; } = "https://models.github.ai/inference/chat/completions";
        public string Model { get; set; } = "openai/gpt-4o-mini";
        public string? GitHubToken { get; set; }
        public double DefaultTemperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 1000;
        public int MaxRetries { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
        public bool EnableCaching { get; set; } = true;
        public int CacheDurationMinutes { get; set; } = 60;
    }
}
```

## AI Service Implementation
```cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.AI;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Infrastructure.Settings;

namespace SmartTaskManagement.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly AiSettings _aiSettings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AiService> _logger;

        public AiService(
            HttpClient httpClient,
            IOptions<AiSettings> aiSettings,
            IMemoryCache cache,
            ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _aiSettings = aiSettings.Value;
            _cache = cache;
            _logger = logger;

            // Configure HttpClient
            _httpClient.BaseAddress = new Uri(_aiSettings.ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(_aiSettings.GitHubToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _aiSettings.GitHubToken);
            }
        }

        public async Task<Response<TaskImprovementResponseDto>> ImproveTaskDescriptionAsync(
            TaskImprovementRequestDto request, Guid userId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.OriginalTitle) &&
                    string.IsNullOrWhiteSpace(request.OriginalDescription))
                {
                    return Response<TaskImprovementResponseDto>.FailureResponse(
                        "Task title or description is required for improvement", 400);
                }

                // Generate cache key
                var cacheKey = GenerateCacheKey(request);

                // Check cache
                if (_aiSettings.EnableCaching && _cache.TryGetValue(cacheKey, out TaskImprovementResponseDto? cachedResponse))
                {
                    if (cachedResponse != null)
                    {
                        _logger.LogInformation("Returning cached AI improvement for task");
                        return Response<TaskImprovementResponseDto>.SuccessResponse(
                            cachedResponse, "Task improved (cached)");
                    }
                }

                // Build the prompt
                var prompt = BuildImprovementPrompt(request);

                // Call AI model
                var startTime = DateTime.UtcNow;
                var response = await CallAIModelAsync(prompt);
                var processingTime = (DateTime.UtcNow - startTime).TotalSeconds;

                // Parse response
                var improvement = await ParseImprovementResponseAsync(response, request, processingTime);

                // Cache the result
                if (_aiSettings.EnableCaching && improvement != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_aiSettings.CacheDurationMinutes));
                    _cache.Set(cacheKey, improvement, cacheOptions);
                }

                _logger.LogInformation("Task improvement completed for user {UserId} in {ProcessingTime}s",
                    userId, processingTime);

                return Response<TaskImprovementResponseDto>.SuccessResponse(
                    improvement!, "Task description improved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error improving task description for user {UserId}", userId);
                return Response<TaskImprovementResponseDto>.FailureResponse(
                    $"AI service error: {ex.Message}", 500);
            }
        }

        public async Task<Response<TaskBulkImprovementResponseDto>> BulkImproveTaskDescriptionsAsync(
            TaskBulkImprovementDto bulkRequest, Guid userId)
        {
            try
            {
                var results = new List<TaskImprovementResultDto>();
                var successful = 0;
                var failed = 0;

                // Process each task (limited to 10 at a time to avoid rate limits)
                var batchSize = 10;
                for (int i = 0; i < bulkRequest.TaskIds.Count; i += batchSize)
                {
                    var batch = bulkRequest.TaskIds.Skip(i).Take(batchSize);
                    var tasks = batch.Select(async taskId =>
                    {
                        // In a real implementation, you would fetch the task from repository
                        // For this example, we'll use placeholder data
                        var request = new TaskImprovementRequestDto
                        {
                            OriginalTitle = $"Task {taskId}",
                            OriginalDescription = $"Description for task {taskId}",
                            Options = bulkRequest.Options
                        };

                        var result = await ImproveTaskDescriptionAsync(request, userId);

                        return new TaskImprovementResultDto
                        {
                            TaskId = taskId,
                            TaskTitle = request.OriginalTitle,
                            Success = result.Success,
                            ErrorMessage = result.Success ? null : result.Message,
                            Improvement = result.Success ? result.Data : null
                        };
                    });

                    var batchResults = await Task.WhenAll(tasks);
                    results.AddRange(batchResults);
                    successful += batchResults.Count(r => r.Success);
                    failed += batchResults.Count(r => !r.Success);

                    // Rate limit: wait 1 second between batches
                    if (i + batchSize < bulkRequest.TaskIds.Count)
                    {
                        await Task.Delay(1000);
                    }
                }

                var response = new TaskBulkImprovementResponseDto
                {
                    TotalProcessed = bulkRequest.TaskIds.Count,
                    Successful = successful,
                    Failed = failed,
                    Results = results
                };

                return Response<TaskBulkImprovementResponseDto>.SuccessResponse(
                    response, $"Processed {successful} out of {bulkRequest.TaskIds.Count} tasks");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk improvement for user {UserId}", userId);
                return Response<TaskBulkImprovementResponseDto>.FailureResponse(
                    $"Bulk improvement error: {ex.Message}", 500);
            }
        }

        public async Task<Response<string>> GenerateTaskSummaryAsync(string description, Guid userId)
        {
            try
            {
                var prompt = $@"
                Please provide a concise summary of the following task description. 
                Focus on the key objectives and deliverables.

                Description:
                {description}

                Summary:";

                var response = await CallAIModelAsync(prompt);
                var summary = ParseSimpleResponse(response);

                return Response<string>.SuccessResponse(summary, "Summary generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary for user {UserId}", userId);
                return Response<string>.FailureResponse($"Summary generation error: {ex.Message}", 500);
            }
        }

        public async Task<Response<List<string>>> SuggestNextActionsAsync(
            string title, string description, string status, Guid userId)
        {
            try
            {
                
                var prompt = $@"
                Based on the following task, suggest 3-5 specific, actionable next steps:

                Task Title: {title}
                Description: {description}
                Current Status: {status}

                Please provide only the list of actions, one per line, without numbering or additional text.
                Use clear, action-oriented language ( that is -, Review the API documentation, Set up the development environment)
                .Actions: ";

                var response = await CallAIModelAsync(prompt);
                var actions = ParseActionList(response);

                return Response<List<string>>.SuccessResponse(actions, "Next actions suggested successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting actions for user {UserId}", userId);
                return Response<List<string>>.FailureResponse($"Action suggestion error: {ex.Message}", 500);
            }
        }

        public async Task<Response<bool>> CheckHealthAsync()
        {
            try
            {
                // Simple health check - try to get model availability
                var testPrompt = "Respond with 'OK' if you are working.";
                var response = await CallAIModelAsync(testPrompt);
                var isHealthy = !string.IsNullOrWhiteSpace(response);

                return Response<bool>.SuccessResponse(isHealthy,
                    isHealthy ? "AI service is healthy" : "AI service is unhealthy");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI service health check failed");
                return Response<bool>.FailureResponse($"Health check failed: {ex.Message}", 503);
            }
        }

        #region Private Methods

        private string BuildImprovementPrompt(TaskImprovementRequestDto request)
        {
            var options = request.Options;
            var sb = new StringBuilder();

            sb.AppendLine("You are an expert project manager and technical writer. ");
            sb.AppendLine("Your task is to improve the following task description to make it more professional, clear, and actionable.");
            sb.AppendLine();

            // Build instruction based on options
            var instructions = new List<string>();

            if (options.CorrectGrammar)
                instructions.Add("- Correct any grammatical errors and improve sentence structure.");

            if (options.ImproveClarity)
                instructions.Add("- Enhance clarity by removing ambiguity and making the description easier to understand.");

            if (options.MakeProfessional)
                instructions.Add("- Use professional, business-appropriate language appropriate for a workplace setting.");

            if (options.ExpandDescription)
                instructions.Add("- Expand the description to include more detail and context where appropriate.");

            if (options.MakeActionable)
                instructions.Add("- Make the description actionable by clearly stating what needs to be done and why.");

            instructions.Add($"- Use a {options.Tone} tone.");
            instructions.Add($"- Keep the total description under {options.MaxLength} characters.");

            if (!string.IsNullOrEmpty(options.Language))
                instructions.Add($"- Respond in {options.Language}.");

            sb.AppendLine("Instructions:");
            foreach (var instruction in instructions)
            {
                sb.AppendLine(instruction);
            }
            sb.AppendLine();

            // Input
            sb.AppendLine("Original Task:");
            sb.AppendLine($"Title: {request.OriginalTitle}");
            sb.AppendLine($"Description: {request.OriginalDescription}");

            if (!string.IsNullOrEmpty(request.AdditionalContext))
            {
                sb.AppendLine($"Additional Context: {request.AdditionalContext}");
            }
            sb.AppendLine();

            // Response format
            sb.AppendLine("Please respond in the following JSON format:");
            sb.AppendLine(@"
            {
                ""improved_title"": ""[Improved title]"",
                ""improved_description"": ""[Improved description]"",
                ""summary"": ""[Brief 1-2 sentence summary]"",
                ""key_points"": [""Key point 1"", ""Key point 2"", ""Key point 3""],
                ""suggested_actions"": [""Action 1"", ""Action 2"", ""Action 3""]
            }

            Make sure the response is valid JSON and the improved version is significantly better than the original.
            ");

            return sb.ToString();
        }

        private async Task<string> CallAIModelAsync(string prompt)
        {
            var payload = new
            {
                model = _aiSettings.Model,
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = _aiSettings.DefaultTemperature,
                max_tokens = _aiSettings.MaxTokens,
                stream = false
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);

            return jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }

        private async Task<TaskImprovementResponseDto> ParseImprovementResponseAsync(
            string response, TaskImprovementRequestDto request, double processingTime)
        {
            try
            {
                // Try to parse JSON from the response
                var jsonStart = response.IndexOf('{');
                var jsonEnd = response.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonString = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var parsed = JsonSerializer.Deserialize<ImprovementJsonResponse>(jsonString);

                    if (parsed != null)
                    {
                        return new TaskImprovementResponseDto
                        {
                            ImprovedTitle = parsed.ImprovedTitle ?? request.OriginalTitle,
                            ImprovedDescription = parsed.ImprovedDescription ?? request.OriginalDescription,
                            Summary = parsed.Summary ?? "Task improved successfully.",
                            KeyPoints = parsed.KeyPoints ?? new List<string>(),
                            SuggestedActions = parsed.SuggestedActions ?? new List<string>(),
                            Metadata = new ImprovementMetadata
                            {
                                Model = _aiSettings.Model,
                                OriginalLength = (request.OriginalTitle + request.OriginalDescription).Length,
                                ImprovedLength = (parsed.ImprovedTitle ?? "" + parsed.ImprovedDescription ?? "").Length,
                                ProcessingTimeSeconds = processingTime,
                                TokensUsed = 0, // Would need to parse from API response headers
                                ProcessedAt = DateTime.UtcNow
                            }
                        };
                    }
                }

                // Fallback: If JSON parsing fails, try to extract improved version from text
                return new TaskImprovementResponseDto
                {
                    ImprovedTitle = request.OriginalTitle,
                    ImprovedDescription = response.Trim(),
                    Summary = "Improvement generated successfully.",
                    KeyPoints = new List<string>(),
                    SuggestedActions = new List<string>(),
                    Metadata = new ImprovementMetadata
                    {
                        Model = _aiSettings.Model,
                        OriginalLength = (request.OriginalTitle + request.OriginalDescription).Length,
                        ImprovedLength = response.Length,
                        ProcessingTimeSeconds = processingTime,
                        TokensUsed = 0,
                        ProcessedAt = DateTime.UtcNow
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing AI response");

                // Return original with improvement note
                return new TaskImprovementResponseDto
                {
                    ImprovedTitle = request.OriginalTitle,
                    ImprovedDescription = request.OriginalDescription +
                        "\n\n[AI Improvement: Unable to parse response. Please try again.]",
                    Summary = "Parsing error occurred.",
                    KeyPoints = new List<string>(),
                    SuggestedActions = new List<string>(),
                    Metadata = new ImprovementMetadata
                    {
                        Model = _aiSettings.Model,
                        OriginalLength = (request.OriginalTitle + request.OriginalDescription).Length,
                        ImprovedLength = (request.OriginalTitle + request.OriginalDescription).Length,
                        ProcessingTimeSeconds = processingTime,
                        TokensUsed = 0,
                        ProcessedAt = DateTime.UtcNow
                    }
                };
            }
        }

        private string ParseSimpleResponse(string response)
        {
            try
            {
                var jsonStart = response.IndexOf('{');
                var jsonEnd = response.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonString = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var parsed = JsonDocument.Parse(jsonString);
                    return parsed.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? response;
                }

                return response.Trim();
            }
            catch
            {
                return response.Trim();
            }
        }

        private List<string> ParseActionList(string response)
        {
            var actions = new List<string>();
            var lines = response.Split('\n');

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    // Remove numbering or bullet points
                    trimmed = System.Text.RegularExpressions.Regex.Replace(trimmed, @"^[\d]+[\.\)]\s*", "");
                    trimmed = System.Text.RegularExpressions.Regex.Replace(trimmed, @"^[\-\*•]\s*", "");

                    if (!string.IsNullOrWhiteSpace(trimmed) && trimmed.Length > 3)
                    {
                        actions.Add(trimmed);
                    }
                }
            }

            return actions.Count > 0 ? actions : new List<string>
        {
            "Review the task requirements",
            "Plan the implementation approach",
            "Start working on the task"
        };
        }

        private string GenerateCacheKey(TaskImprovementRequestDto request)
        {
            var key = $"{request.OriginalTitle}|{request.OriginalDescription}|{request.AdditionalContext}|";
            key += $"{request.Options.CorrectGrammar}|{request.Options.ImproveClarity}|";
            key += $"{request.Options.MakeProfessional}|{request.Options.ExpandDescription}|";
            key += $"{request.Options.MakeActionable}|{request.Options.Tone}";

            // Hash the key to ensure it's not too long
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(key);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        #endregion

        #region Inner Models

        private class ImprovementJsonResponse
        {
            public string? ImprovedTitle { get; set; }
            public string? ImprovedDescription { get; set; }
            public string? Summary { get; set; }
            public List<string>? KeyPoints { get; set; }
            public List<string>? SuggestedActions { get; set; }
        }

        #endregion
    }
}
```

##  AI Controller
```cs
// SmartTaskManagement.API/Controllers/AiController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.AI;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly ILogger<AiController> _logger;

        public AiController(IAiService aiService, ILogger<AiController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Improve a single task description using AI
        /// </summary>
        [HttpPost("improve-task")]
        public async Task<IActionResult> ImproveTaskDescription([FromBody] TaskImprovementRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.ImproveTaskDescriptionAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Bulk improve task descriptions
        /// </summary>
        [HttpPost("bulk-improve")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> BulkImproveTasks([FromBody] TaskBulkImprovementDto bulkRequest)
        {
            var userId = GetUserId();
            var result = await _aiService.BulkImproveTaskDescriptionsAsync(bulkRequest, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Generate a summary for a task description
        /// </summary>
        [HttpPost("summarize")]
        public async Task<IActionResult> GenerateSummary([FromBody] SummarizeRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.GenerateTaskSummaryAsync(request.Description, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Suggest next actions for a task
        /// </summary>
        [HttpPost("suggest-actions")]
        public async Task<IActionResult> SuggestActions([FromBody] SuggestActionsRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.SuggestNextActionsAsync(
                request.Title, request.Description, request.Status, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Check AI service health
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckHealth()
        {
            var result = await _aiService.CheckHealthAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Preview improvement without saving
        /// </summary>
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewImprovement([FromBody] TaskImprovementRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.ImproveTaskDescriptionAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }

    // Additional DTOs for specific endpoints
    public class SummarizeRequestDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public class SuggestActionsRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "To Do";
    }
}
```

## Add AI Service to Task Service Integration
Update the Task Service to use AI improvement:
```cs
// Add to TaskService.cs
private readonly IAiService _aiService;

public TaskService(
    IUnitOfWork unitOfWork, 
    ILogger<TaskService> logger,
    IAiService aiService) // Add this parameter
{
    _unitOfWork = unitOfWork;
    _logger = logger;
    _aiService = aiService;
}

// Add a new method to improve a task description
public async Task<Response<TaskDto>> ImproveTaskDescriptionAsync(
    Guid taskId, TaskImprovementOptionsDto options, Guid userId)
{
    try
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task == null || task.IsDeleted)
        {
            return Response<TaskDto>.FailureResponse("Task not found", 404);
        }

        // Check permission
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return Response<TaskDto>.FailureResponse("User not found", 404);
        }

        if (!await HasTaskPermission(task, userId, user.Role))
        {
            return Response<TaskDto>.FailureResponse(
                "You don't have permission to modify this task", 403);
        }

        // Call AI service
        var request = new TaskImprovementRequestDto
        {
            OriginalTitle = task.Title,
            OriginalDescription = task.Description,
            Options = new ImprovementOptions
            {
                CorrectGrammar = options.CorrectGrammar,
                ImproveClarity = options.ImproveClarity,
                MakeProfessional = options.MakeProfessional,
                ExpandDescription = options.ExpandDescription,
                MakeActionable = options.MakeActionable,
                Tone = options.Tone ?? "Professional"
            }
        };

        var aiResult = await _aiService.ImproveTaskDescriptionAsync(request, userId);
        
        if (!aiResult.Success)
        {
            return Response<TaskDto>.FailureResponse(
                $"AI improvement failed: {aiResult.Message}", 500);
        }

        // Update task with improved content
        task.Title = aiResult.Data!.ImprovedTitle;
        task.Description = aiResult.Data.ImprovedDescription;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = user.Username;

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.CompleteAsync();

        var taskDto = await MapToTaskDto(task);
        
        _logger.LogInformation("Task {TaskTitle} improved by AI for user {UserId}", 
            task.Title, userId);
        
        return Response<TaskDto>.SuccessResponse(taskDto, 
            "Task description improved successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error improving task {TaskId} by AI for user {UserId}", 
            taskId, userId);
        return Response<TaskDto>.FailureResponse(
            "An error occurred while improving the task", 500);
    }
}

// Helper DTO 
public class TaskImprovementOptionsDto
{
    public bool CorrectGrammar { get; set; } = true;
    public bool ImproveClarity { get; set; } = true;
    public bool MakeProfessional { get; set; } = true;
    public bool ExpandDescription { get; set; } = true;
    public bool MakeActionable { get; set; } = true;
    public string? Tone { get; set; }
}

```
## Update Program.cs
```cs
// In Program.cs

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

// Register AI Service
builder.Services.AddScoped<IAiService, AiService>();
```

## Configuration in appsettings.json

```json
{
  "AiSettings": {
    "ApiBaseUrl": "https://models.github.ai/inference/chat/completions",
    "Model": "openai/gpt-4o-mini",
    "GitHubToken": "YOUR_GITHUB_PAT_TOKEN",
    "DefaultTemperature": 0.7,
    "MaxTokens": 1000,
    "MaxRetries": 3,
    "TimeoutSeconds": 30,
    "EnableCaching": true,
    "CacheDurationMinutes": 60
  }
}
```

##  PROMPTS.md Documentation

```markdown
# PROMPTS.md - AI Feature Documentation

## Overview

The Smart Task Management System uses GitHub Models to provide AI-powered task description improvements. This document details the prompt design, structure, and safety considerations.

## AI Provider

**GitHub Models** - Free inference API available to all GitHub users with a Personal Access Token (PAT) with `models:read` scope.

### Available Models
- `openai/gpt-4o-mini` - Primary model (lightweight, fast)
- `openai/gpt-4.1` - Higher quality improvements
- `meta/meta-llama-3.1-8b-instruct` - Alternative open source model

## Prompt Design

### 1. Task Improvement Prompt

**Purpose**: Transform task descriptions into professional, clear, and actionable content.

**Structure**:
```

## Database migration, build and run the project

```bash
# navigate to the Infrastracture project ./SmartTaskManagement.Infrastracture
cd ./SmartTaskManagement.Infrastracture

# Create and apply migrations
dotnet ef migrations add AiModule --startup-project ../SmartTaskManagement.API

# update database
dotnet ef database update --startup-project ../SmartTaskManagement.API

# For build the project navigate to the API project
cd ../SmartTaskManagement.API

# then build
dotnet build

# then run
dotnet run

```
This is the End of Part 12
