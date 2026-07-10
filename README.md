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