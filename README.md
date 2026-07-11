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
