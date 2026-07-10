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

