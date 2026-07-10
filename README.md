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