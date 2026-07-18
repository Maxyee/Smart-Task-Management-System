# Smart Task Management System (STMS)

A modern, secure, and maintainable full-stack task management application built with ASP.NET Core 10 and Angular 21.

## 📋 Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [API Overview](#api-overview)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Security](#security)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

## 🚀 Project Overview

Smart Task Management System (STMS) is a comprehensive task management solution designed to demonstrate modern software engineering practices. It provides a complete project and task management system with role-based access control, AI-powered features, and a responsive user interface.

### Key Highlights
- **Clean Architecture** for maintainability and testability
- **JWT Authentication** with refresh tokens
- **Role-Based Access Control** (Admin, Project Manager, Team Member)
- **AI-Powered Task Description Enhancement** using GitHub Models
- **Real-time Dashboard** with analytics and metrics
- **Responsive UI** built with Angular 21 and Tailwind CSS
- **Comprehensive API Documentation** with Swagger/OpenAPI

## ✨ Features

### Authentication & Authorization
- User Registration, Login, Logout
- JWT Authentication with Refresh Tokens
- Role-Based Authorization (Admin, Project Manager, Team Member)
- Password Hashing with Identity Framework
- Secure Configuration Management

### Project Management
- Create, Read, Update, Delete Projects
- Search, Filter, Sort, and Paginate Projects
- Project Status Management (Active/Inactive)
- Project Progress Tracking
- Project Statistics and Analytics

### Task Management
- Full CRUD Operations for Tasks
- Task Assignment to Users
- Status Management (To Do, In Progress, Completed, Cancelled)
- Priority Management (Low, Medium, High, Critical)
- Due Date Management
- Search, Filter, Sort, and Paginate Tasks
- Bulk Operations (Status Updates)

### Dashboard & Analytics
- Project Statistics (Total, Active, Inactive, Completion Rate)
- Task Analytics (Status Distribution, Priority Distribution)
- Team Performance Metrics
- Activity Timeline
- User Performance Tracking
- Real-time Updates

### AI-Powered Features
- Task Description Improvement
- Grammar Correction and Clarity Enhancement
- Professional Tone and Actionable Content
- Task Summarization
- Suggested Next Actions
- Bulk AI Improvement for Multiple Tasks

## 🛠️ Technology Stack

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core | 10.0 | Web API Framework |
| Entity Framework Core | 10.0 | ORM |
| SQL Server | 2022 | Database |
| JWT Authentication | - | Security |
| Swagger/OpenAPI | 6.5 | API Documentation |
| Serilog | 8.0 | Logging |
| FluentValidation | 11.8 | Input Validation |
| GitHub Models | - | AI Integration |

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 21.2 | Frontend Framework |
| TypeScript | 5.9 | Programming Language |
| Tailwind CSS | 4.1 | Styling |
| RxJS | 7.8 | Reactive Programming |

### Development Tools
- Visual Studio 2022 / VS Code
- Git & GitHub
- Postman / Swagger UI
- npm / Node.js

## Architecture

### Clean Architecture (Onion Architecture)
┌─────────────────────────────────────────────────────────────┐
│ Presentation Layer │
│ (API Controllers / Angular UI) │
├─────────────────────────────────────────────────────────────┤
│ Application Layer │
│ (Use Cases, DTOs, Interfaces) │
├─────────────────────────────────────────────────────────────┤
│ Domain Layer │
│ (Entities, Enums, Core Business Logic) │
├─────────────────────────────────────────────────────────────┤
│ Infrastructure Layer │
│ (Data Access, External Services, Logging) │
└─────────────────────────────────────────────────────────────┘


### Layer Responsibilities

**Domain Layer** (`SmartTaskManagement.Domain`)
- Core business entities and value objects
- Enums and constants
- Domain interfaces
- No external dependencies

**Application Layer** (`SmartTaskManagement.Application`)
- Use cases and business logic
- DTOs and mapping profiles
- Application interfaces
- Validation rules (FluentValidation)
- Depends on Domain layer

**Infrastructure Layer** (`SmartTaskManagement.Infrastructure`)
- Data access (Entity Framework Core)
- External service implementations
- JWT and authentication
- Logging (Serilog)
- Depends on Application layer

**Presentation Layer** (`SmartTaskManagement.API`)
- REST API Controllers
- Middleware (Exception Handling)
- Filters and validators
- Swagger documentation
- Depends on Infrastructure layer

**Frontend** (`stms-frontend`)
- Angular components and services
- State management
- HTTP interceptors
- Route guards
- Responsive UI

## 📦 Setup Instructions

### Prerequisites
- .NET 10.0 SDK or later
- Node.js 20+ and npm
- SQL Server 2022 (or SQL Server LocalDB)
- Git
- Visual Studio 2022 or VS Code

## Clone the repository

```bash
git clone https://github.com/Maxyee/Smart-Task-Management-System.git
cd Smart-Task-Management-System
```

## Frontend Setup

Go inside the ./STMS-Frontend

Then run 

```bash
npm install
```

After installing the package run

```bash
ng serve

# then visit url : http://localhost:4200
```


## Backend Setup

create file inside this url : ./STMS-Backend/src/SmartTaskManagement.API/Properties/launchSettings.json

```json
// launchSettings.json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5027",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7272;http://localhost:5027",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}


```

create file inside this url : ./STMS-Backend/src/SmartTaskManagement.API/appsettings.json

```json
// create file : appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartTaskManagement;Trusted_Connection=True;MultipleActiveResultSets=true",
    "DefaultConnection2": "Server=localhost\\SQLEXPRESS;Database=SmartDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YOUR-VERY-LONG-SECRET-KEY-HERE-MINIMUM-32-CHARACTERS",
    "Issuer": "SmartTaskManagement",
    "Audience": "SmartTaskManagementAPI",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "AiSettings": {
    "ApiBaseUrl": "https://models.github.ai/inference/chat/completions",
    "Model": "openai/gpt-4o-mini",
    "GitHubToken": "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",
    "DefaultTemperature": 0.7,
    "MaxTokens": 1000,
    "MaxRetries": 3,
    "TimeoutSeconds": 30,
    "EnableCaching": true,
    "CacheDurationMinutes": 60
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

Go inside the ./STMS-Backend/src/SmartTaskManagement.Infrastructure/

```bash
# Create and apply migrations
dotnet ef migrations add InitialCreate --startup-project ../SmartTaskManagement.API

# update database
dotnet ef database update --startup-project ../SmartTaskManagement.API


```

Then go to API project folder 

```bash
# first navigate to the API project
cd ./SmartTaskManagement.API

# build the project
dotnet build

# run the project
dotnet run

# Swagger URL : http://localhost:5027/swagger/index.html
# API URL: http://localhost:5027/api/
```

## Default Admin Credentials
Email: admin@smarttask.com
Password: Admin@123


# API Overview
## Authentication Endpoints
```text
Method	Endpoint	                Description
POST	/api/auth/register	        Register new user
POST	/api/auth/login	            Login and get JWT
POST	/api/auth/refresh	        Refresh access token
POST	/api/auth/logout	        Logout user
POST	/api/auth/change-password	Change password

```

## Project Endpoints

```text
Method	    Endpoint	                        Description
GET	        /api/projects	                    Get all projects (filtered)
GET	        /api/projects/{id}	                Get project by ID
POST	    /api/projects	                    Create new project
PUT	        /api/projects/{id}	                Update project
DELETE	    /api/projects/{id}	                Delete project
PATCH	    /api/projects/{id}/toggle-status	Toggle project status
GET	        /api/projects/{id}/summary	        Get project task summary

```


## Task Endpoints

```text
Method	            Endpoint	                Description
GET	                /api/tasks	                Get all tasks (filtered)
GET	                /api/tasks/{id}	            Get task by ID
POST	            /api/tasks	                Create new task
PUT	                /api/tasks/{id}	            Update task
DELETE	            /api/tasks/{id}	            Delete task
PATCH	            /api/tasks/{id}/status	    Update task status
PATCH	            /api/tasks/{id}/assign	    Assign task to user
GET	                /api/tasks/statistics	    Get task statistics
GET	                /api/tasks/overdue	        Get overdue tasks
GET	                /api/tasks/due-soon	        Get tasks due soon
PATCH	            /api/tasks/bulk-status	    Bulk update status

```

## Dashboard Endpoints

```text
Method	        Endpoint	                            Description
GET	            /api/dashboard/summary	                Get dashboard summary
GET	            /api/dashboard/projects/progress	    Get all projects progress
GET	            /api/dashboard/projects/{id}/progress	Get project progress
GET	            /api/dashboard/users/{id}/performance	Get user performance
GET	            /api/dashboard/team/performance	        Get team performance
GET	            /api/dashboard/metrics	                Get performance metrics
GET	            /api/dashboard/activities	            Get recent activities

```

## AI Endpoints
```text
Method	    Endpoint	                Description
POST	    /api/ai/improve-task	    Improve task description
POST	    /api/ai/bulk-improve	    Bulk improve tasks
POST	    /api/ai/summarize	        Generate task summary
POST	    /api/ai/suggest-actions	    Suggest next actions
GET	        /api/ai/health	            Check AI service health

```

## User Endpoints
```text
Method	        Endpoint	                        Description
GET	            /api/users	                        Get all users
GET	            /api/users/available-for-assignment	Get users for assignment
GET	            /api/users/active	                Get active users
GET	            /api/users/search	                Search users
GET	            /api/users/{id}	                    Get user by ID
GET	            /api/users/{id}/performance	        Get user performance
PUT	            /api/users/{id}	                    Update user
PATCH	        /api/users/{id}/toggle-status	    Toggle user status
GET	            /api/users/statistics	            Get user statistics

```

# API Response Format
## All API responses follow a consistent format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": null,
  "statusCode": 200
}

```
## Error Response

```json
{
  "success": false,
  "message": "Error message",
  "data": null,
  "errors": ["Error detail 1", "Error detail 2"],
  "statusCode": 400
}

```

# Project Structure
## Backend Structure
```text
SmartTaskManagement/
├── src/
│   ├── SmartTaskManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Project.cs
│   │   │   └── TaskItem.cs
│   │   ├── Enums/
│   │   │   ├── TaskStatus.cs
│   │   │   ├── TaskPriority.cs
│   │   │   └── Role.cs
│   │   └── Common/
│   │       └── BaseEntity.cs
│   │
│   ├── SmartTaskManagement.Application/
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Projects/
│   │   │   ├── Tasks/
│   │   │   ├── Dashboard/
│   │   │   ├── AI/
│   │   │   └── Users/
│   │   ├── Interfaces/
│   │   │   ├── Services/
│   │   │   └── Repositories/
│   │   ├── Validators/
│   │   └── Common/
│   │       └── Response.cs
│   │
│   ├── SmartTaskManagement.Infrastructure/
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   ├── Services/
│   │   ├── Settings/
│   │   └── Extensions/
│   │
│   └── SmartTaskManagement.API/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Filters/
│       └── Program.cs
│
├── tests/
│   └── SmartTaskManagement.Tests/
│
├── SmartTaskManagement.sln
├── README.md
└── PROMPTS.md

```

## Frontend Structure
```text
stms-frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── guards/
│   │   │   │   ├── auth.guard.ts
│   │   │   │   └── role.guard.ts
│   │   │   ├── interceptors/
│   │   │   │   ├── auth.interceptor.ts
│   │   │   │   └── error.interceptor.ts
│   │   │   ├── services/
│   │   │   │   ├── api.service.ts
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── project.service.ts
│   │   │   │   ├── task.service.ts
│   │   │   │   ├── user.service.ts
│   │   │   │   ├── dashboard.service.ts
│   │   │   │   ├── ai.service.ts
│   │   │   │   └── notification.service.ts
│   │   │   └── models/
│   │   │       ├── user.model.ts
│   │   │       ├── auth.model.ts
│   │   │       ├── project.model.ts
│   │   │       ├── task.model.ts
│   │   │       ├── dashboard.model.ts
│   │   │       └── ai.model.ts
│   │   │
│   │   ├── features/
│   │   │   ├── auth/
│   │   │   │   ├── login/
│   │   │   │   └── register/
│   │   │   ├── dashboard/
│   │   │   ├── projects/
│   │   │   │   ├── project-list/
│   │   │   │   ├── project-form/
│   │   │   │   └── project-detail/
│   │   │   └── tasks/
│   │   │       ├── task-list/
│   │   │       ├── task-form/
│   │   │       ├── task-detail/
│   │   │       └── task-ai-improvement/
│   │   │
│   │   ├── shared/
│   │   │   ├── components/
│   │   │   │   ├── header/
│   │   │   │   ├── footer/
│   │   │   │   └── notification/
│   │   │   └── utils/
│   │   │
│   │   ├── app.component.ts
│   │   ├── app.config.ts
│   │   └── app.routes.ts
│   │
│   ├── assets/
│   ├── environments/
│   ├── styles.css
│   ├── index.html
│   └── main.ts
│
├── angular.json
├── package.json
├── tsconfig.json
└── tailwind.config.js

```

# Database Schema

## Users Table

```text
Column	                    Type	            Description
Id	                        uniqueidentifier	Primary Key
Email	                    nvarchar(100)	    Unique email
Username	                nvarchar(50)	    Unique username
FirstName	                nvarchar(50)	    First name
LastName	                nvarchar(50)	    Last name
PasswordHash	            nvarchar(max)	    Hashed password
Role	                    int	                User role (0=Admin, 1=ProjectManager, 2=TeamMember)
RefreshToken	            nvarchar(max)	    JWT refresh token
RefreshTokenExpiryTime	    datetime2	        Token expiry
IsActive	                bit	                Active status
IsDeleted	                bit	                Soft delete flag
CreatedAt	                datetime2	        Creation timestamp
UpdatedAt	                datetime2	        Last update timestamp
```

## Projects Table

```text
Column	        Type	            Description
Id	            uniqueidentifier	Primary Key
Name	        nvarchar(100)	    Project name
Description	    nvarchar(500)	    Project description
StartDate	    datetime2	        Start date
EndDate	        datetime2	        End date
IsActive	    bit	                Active status

```

## Tasks Table

```text
Column	            Type	            Description
Id	                uniqueidentifier	Primary Key
Title	            nvarchar(200)	    Task title
Description	        nvarchar(1000)	    Task description
Status	            int	                Task status (0=ToDo, 1=InProgress, 2=Completed, 3=Cancelled)
Priority	        int	                Task priority (0=Low, 1=Medium, 2=High, 3=Critical)
DueDate	            datetime2	        Due date
EstimatedHours	    int	                Estimated hours
ActualHours	        int	                Actual hours
ProjectId	        uniqueidentifier	Foreign key to Projects
AssignedToUserId	uniqueidentifier	Foreign key to Users
CreatedByUserId	    uniqueidentifier	Foreign key to Users
IsDeleted	        bit	                Soft delete flag
CreatedAt	        datetime2	        Creation timestamp
UpdatedAt	        datetime2	        Last update timestamp

```


# Security
## Implemented Security Features

### JWT Authentication

```text
Stateless authentication
Access tokens (15 min expiry)
Refresh tokens (7 days expiry)
Secure token storage
```

### Role-Based Authorization
```text
Admin: Full system access
Project Manager: Manage projects and tasks
Team Member: View and update assigned tasks
```

### Password Security
```text
ASP.NET Core Identity PasswordHasher
Password complexity requirements
Password change functionality

```

### API Security
```text
HTTPS enforcement
CORS configuration
Rate limiting
Input validation (FluentValidation)
```

### Data Protection

```text
Parameterized queries (EF Core)
SQL injection prevention
XSS protection
```


# Authorization Rules

```text
Feature	Admin	    Project     Manager	    Team Member
View Users	        ✅	        ✅	        ❌
Manage Users	    ✅	        ❌	        ❌
Create Projects	    ✅	        ✅	        ❌
Update Projects	    ✅	        ✅	        ❌
Delete Projects	    ✅	        ✅	        ❌
View All Projects	✅	        ✅	        Limited
Create Tasks	    ✅	        ✅	        ✅
Update Tasks	    ✅	        ✅	        Limited
Delete Tasks	    ✅	        ✅	        ❌
View All Tasks	    ✅	        ✅	        Limited
AI Features	        ✅	        ✅	        ✅
Dashboard	        ✅	        Limited	      Limited

```

# 📝 License
This project is licensed under the MIT License - see the LICENSE file for details.


# Contact
Email: mdjulhashossainmohon@gmail.com