# PROMPTS.md - AI Feature Documentation

## 📋 Table of Contents
- [Overview](#overview)
- [AI Provider](#ai-provider)
- [Prompt Design](#prompt-design)
- [Prompt Structure](#prompt-structure)
- [Example Inputs and Outputs](#example-inputs-and-outputs)
- [Validation Approach](#validation-approach)
- [Safety Considerations](#safety-considerations)
- [Rate Limits](#rate-limits)
- [Performance](#performance)
- [Error Handling](#error-handling)
- [Monitoring and Logging](#monitoring-and-logging)
- [Upgrade Options](#upgrade-options)

## 🎯 Overview

The Smart Task Management System integrates AI capabilities using GitHub Models to enhance task descriptions, generate summaries, and suggest actionable next steps. This document details the prompt design, structure, validation approach, and safety considerations for the AI feature.

### Key AI Capabilities
- **Task Description Improvement**: Transform vague descriptions into clear, professional, and actionable content
- **Task Summarization**: Generate concise summaries of task descriptions
- **Next Action Suggestions**: Provide specific, actionable next steps
- **Bulk Improvement**: Process multiple tasks simultaneously

## 🤖 AI Provider

### GitHub Models

GitHub Models provides free access to various AI models through GitHub's infrastructure.

**Available Models**
| Model | Description | Best For |
|-------|-------------|----------|
| `openai/gpt-4o-mini` | Lightweight, fast model | Quick improvements, summaries |
| `openai/gpt-4.1` | High-quality, comprehensive | Complex improvements |
| `meta/meta-llama-3.1-8b-instruct` | Open source alternative | Cost-effective, good quality |

**Configuration**
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

# Prompt Design

Purpose: Transform task descriptions into professional, clear, and actionable content.

Structure:
```text
[System Context]
[Instructions]
[User Input]
[Response Format]

```

System Context:

```text
You are an expert project manager and technical writer with 15+ years of experience. 
Your task is to improve task descriptions to make them more professional, clear, and actionable.
```

Instructions Builder:

```ts
private BuildImprovementPrompt(request: TaskImprovementRequestDto): string
{
    var sb = new StringBuilder();
    
    // Add system context
    sb.AppendLine("You are an expert project manager and technical writer.");
    sb.AppendLine("Your task is to improve the following task description.");
    
    // Build instructions based on options
    if (request.Options.CorrectGrammar)
        sb.AppendLine("- Correct grammatical errors and improve sentence structure.");
    
    if (request.Options.ImproveClarity)
        sb.AppendLine("- Enhance clarity by removing ambiguity.");
    
    if (request.Options.MakeProfessional)
        sb.AppendLine("- Use professional, business-appropriate language.");
    
    if (request.Options.ExpandDescription)
        sb.AppendLine("- Expand the description with relevant context and details.");
    
    if (request.Options.MakeActionable)
        sb.AppendLine("- Make the description actionable with clear next steps.");
    
    // Add tone and constraints
    sb.AppendLine($"- Use a {request.Options.Tone} tone.");
    sb.AppendLine($"- Keep total description under {request.Options.MaxLength} characters.");
    
    // Add input
    sb.AppendLine($"Title: {request.OriginalTitle}");
    sb.AppendLine($"Description: {request.OriginalDescription}");
    
    // Define response format
    sb.AppendLine("Please respond in JSON format with these fields:");
    sb.AppendLine("{
      \"improved_title\": \"string\",
      \"improved_description\": \"string\",
      \"summary\": \"string\",
      \"key_points\": [\"string\"],
      \"suggested_actions\": [\"string\"]
    }");
    
    return sb.ToString();
}
```
## Summary Generation Prompt
Purpose: Generate concise summaries of task descriptions.

```text

System: You are a project manager who can create concise summaries.
Task: Generate a 1-2 sentence summary of the following task description.
Description: {description}
Summary: [Generate concise summary]

```
## Next Actions Prompt

```text
System: You are a senior project manager who excels at breaking down tasks.
Task: Suggest 3-5 specific, actionable next steps for this task.
Title: {title}
Description: {description}
Status: {status}
Requirements: 
- Each action should be specific and measurable
- Use action-oriented language
- Provide only the list, one per line
- No numbering or additional text

Actions:
[Generate list]

```

## Bulk Improvement Prompt
Purpose: Process multiple tasks efficiently.

```text
System: You are a project management expert.
Task: Improve the following tasks in the provided JSON format.
Format: [
  { "id": "task1", "title": "...", "description": "..." },
  { "id": "task2", "title": "...", "description": "..." }
]
Instructions: {same as single task improvement}

```

# Prompt Structure

## Standard Prompt Template

```json

{
  "model": "openai/gpt-4o-mini",
  "messages": [
    {
      "role": "system",
      "content": "You are an expert project manager and technical writer..."
    },
    {
      "role": "user",
      "content": "Improve the following task description..."
    }
  ],
  "temperature": 0.7,
  "max_tokens": 1000,
  "stream": false
}
```

# Response Structure

```json
{
  "choices": [
    {
      "message": {
        "content": "{\n  \"improved_title\": \"...\",\n  \"improved_description\": \"...\",\n  \"summary\": \"...\",\n  \"key_points\": [...],\n  \"suggested_actions\": [...]\n}"
      }
    }
  ]
}

```

# Example Inputs and Outputs

## Example 1: Basic Task Improvement

Input :
```json
{
  "originalTitle": "Fix login bug",
  "originalDescription": "there is a bug in the login page where users cannot login sometimes. it happens randomly.",
  "options": {
    "correctGrammar": true,
    "improveClarity": true,
    "makeProfessional": true,
    "expandDescription": true,
    "makeActionable": true,
    "tone": "Professional"
  }
}

```

Output:

```json
{
  "improvedTitle": "Resolve Intermittent Login Page Authentication Error",
  "improvedDescription": "Users are experiencing intermittent login failures on the authentication page. The issue manifests as a '500 Internal Server Error' that occurs randomly during the login process. This prevents users from accessing their accounts and requires immediate investigation. \n\nKey investigation areas:\n1. Review application logs for error patterns\n2. Check database connection stability\n3. Validate authentication service configuration\n4. Test with different user credentials\n\nExpected resolution: Stable login functionality with 99.9% success rate.",
  "summary": "Fix intermittent login page errors causing 500 server responses.",
  "keyPoints": [
    "500 Internal Server Error on login attempts",
    "Random occurrence pattern",
    "Users cannot access accounts",
    "Immediate investigation required"
  ],
  "suggestedActions": [
    "Review application logs for error details",
    "Check database connectivity and connection pooling",
    "Validate authentication service configuration",
    "Test with various user credentials",
    "Monitor error rates after each fix"
  ]
}

```


## Example 2: Vague Task Improvement

Input: 

```json
{
  "originalTitle": "Update docs",
  "originalDescription": "need to update the API documentation for the new endpoints. add examples. also mention the authentication requirements.",
  "options": {
    "correctGrammar": true,
    "improveClarity": true,
    "makeProfessional": true,
    "makeActionable": true,
    "tone": "Professional"
  }
}

```

Output:

```json
{
  "improvedTitle": "Update API Documentation with Authentication Examples",
  "improvedDescription": "The API documentation requires comprehensive updates to cover the recently implemented endpoints. This includes:\n\n1. Adding detailed API specifications for all new endpoints\n2. Including practical code examples in multiple languages\n3. Documenting authentication requirements (JWT tokens)\n4. Providing error handling examples\n5. Updating the API reference with request/response schemas\n\nThe updated documentation should be clear, complete, and accessible to developers of all experience levels.",
  "summary": "Update API docs to cover new endpoints with examples and authentication details.",
  "keyPoints": [
    "Add new endpoint specifications",
    "Include code examples",
    "Document authentication",
    "Improve accessibility"
  ],
  "suggestedActions": [
    "Review all new endpoints and their parameters",
    "Create code examples in at least 3 languages",
    "Document authentication process with JWT",
    "Add error response examples",
    "Get peer review on updated documentation"
  ]
}

```

## Example 3: Summary Generation

Input:
```text
Description: "Implement a new authentication system with JWT tokens, refresh tokens, and role-based authorization. The system should support user registration, login, logout, and password reset functionality."

```

Output:

```text
Implement JWT-based authentication with refresh tokens, role-based authorization, and complete user management (registration, login, logout, password reset).

```

## Example 4: Next Actions

Input:

```json
{
  "title": "Implement User Authentication",
  "description": "Build JWT authentication with refresh tokens for the Smart Task Management System",
  "status": "InProgress"
}

```

Output:

```text
[
  "Review the existing authentication service implementation",
  "Create JWT token generation and validation logic",
  "Implement refresh token rotation mechanism",
  "Add role-based authorization checks",
  "Write unit tests for authentication flows"
]

```

#  Validation Approach

## Input Validation

Frontend Validation:

```ts

// Check minimum length
if (request.originalDescription.length < 10) {
  throw new Error('Description must be at least 10 characters');
}

// Check maximum length
if (request.originalDescription.length > 2000) {
  throw new Error('Description cannot exceed 2000 characters');
}

// Sanitize input
const sanitizedDescription = this.sanitizeInput(request.originalDescription);

// Validate options
if (!request.options.tone || !allowedTones.includes(request.options.tone)) {
  request.options.tone = 'Professional'; // Default
}

```

## Backend Validation:

```cs
public class TaskImprovementRequestValidator : AbstractValidator<TaskImprovementRequestDto>
{
    public TaskImprovementRequestValidator()
    {
        RuleFor(x => x.OriginalTitle)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title exceeds maximum length");

        RuleFor(x => x.OriginalDescription)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description exceeds maximum length");

        RuleFor(x => x.Options)
            .NotNull().WithMessage("Options are required");

        When(x => x.Options != null, () =>
        {
            RuleFor(x => x.Options.MaxLength)
                .InclusiveBetween(100, 2000)
                .WithMessage("MaxLength must be between 100 and 2000");

            RuleFor(x => x.Options.Tone)
                .Must(tone => new[] { "Professional", "Formal", "Friendly", "Technical" }.Contains(tone))
                .WithMessage("Invalid tone value");
        });
    }
}

```

## Output Validation

JSON Parsing:

```ts

try {
  const parsed = JSON.parse(response);
  const validation = validateImprovementResult(parsed);
  
  if (!validation.isValid) {
    return createFallbackResult(request);
  }
  
  return parsed;
} catch (error) {
  console.error('Failed to parse AI response:', error);
  return createFallbackResult(request);
}

```

## Result Validation:

```ts

function validateImprovementResult(result: any): ValidationResult {
  const errors: string[] = [];
  
  if (!result.improved_title || result.improved_title.length < 3) {
    errors.push('Invalid improved title');
  }
  
  if (!result.improved_description || result.improved_description.length < 10) {
    errors.push('Invalid improved description');
  }
  
  if (result.key_points && !Array.isArray(result.key_points)) {
    errors.push('Key points must be an array');
  }
  
  if (result.suggested_actions && !Array.isArray(result.suggested_actions)) {
    errors.push('Suggested actions must be an array');
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
}

```

## Fallback Strategy

```ts
function createFallbackResult(request: TaskImprovementRequestDto): TaskImprovementResponseDto {
  return {
    improvedTitle: request.originalTitle,
    improvedDescription: request.originalDescription + 
      "\n\n[AI Improvement: Unable to process. Please try again or improve manually.]",
    summary: "AI improvement failed. Please try again.",
    keyPoints: [],
    suggestedActions: [],
    metadata: {
      model: "Fallback",
      originalLength: request.originalDescription.length,
      improvedLength: request.originalDescription.length,
      processingTimeSeconds: 0,
      tokensUsed: 0,
      processedAt: new Date()
    }
  };
}

```

# Safety Considerations

## Content Filtering

```text
System: You are a professional project manager. 
You must:
- Use professional language only
- Not generate harmful or inappropriate content
- Focus on task improvement
- Respect cultural and professional norms

```

## Input Sanitization:

```ts
function sanitizeInput(input: string): string {
  // Remove potentially harmful characters
  return input
    .replace(/<[^>]*>/g, '') // Remove HTML tags
    .replace(/[^\w\s\-.,!?()]/g, '') // Remove special characters
    .trim()
    .substring(0, 2000); // Limit length
}

```

## Rate Limiting

Configuration:

```cs
// API Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AI", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5; // 5 requests per 10 seconds
        opt.QueueLimit = 2;
    });
});

// Usage in Controller
[HttpPost("improve-task")]
[EnableRateLimiting("AI")]
public async Task<IActionResult> ImproveTask(TaskImprovementRequestDto request)

```

Frontend Rate Limiting:

```ts
private throttleRequests() {
  // Prevent multiple requests within 2 seconds
  if (this.lastRequestTime && 
      Date.now() - this.lastRequestTime < 2000) {
    return throwError(() => new Error('Please wait before making another request'));
  }
  this.lastRequestTime = Date.now();
}

```

## Token Security

Secure Token Storage:

```json
// appsettings.Production.json
{
  "AiSettings": {
    "GitHubToken": "Use environment variable or Azure Key Vault"
  }
}

```

Environment Variable Setup:

```bash
# Set environment variable
export AI__GITHUB_TOKEN="your_github_pat_token"

# Or use Azure Key Vault
dotnet user-secrets set "AiSettings:GitHubToken" "your_token"

```

## Error Handling
Graceful Degradation:

```ts
try {
  const result = await aiService.improveTaskDescription(request);
  return result;
} catch (error) {
  console.error('AI service error:', error);
  
  // Return original content with note
  return {
    improvedTitle: request.originalTitle,
    improvedDescription: `${request.originalDescription}\n\n[AI improvement unavailable. Please try again later.]`,
    summary: 'AI service temporarily unavailable',
    keyPoints: [],
    suggestedActions: [],
    metadata: {
      model: 'Error Fallback',
      originalLength: request.originalDescription.length,
      improvedLength: request.originalDescription.length,
      processingTimeSeconds: 0,
      tokensUsed: 0,
      processedAt: new Date()
    }
  };
}

```

## Caching
Response Caching:

```ts

private async CallAIModelAsync(prompt: string, userId: Guid): Promise<string>
{
    // Generate cache key based on prompt and user
    var cacheKey = GenerateCacheKey(prompt, userId);
    
    // Check cache
    if (_aiSettings.EnableCaching && _cache.TryGetValue(cacheKey, out string? cachedResponse))
    {
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            _logger.LogInformation("Returning cached AI response");
            return cachedResponse;
        }
    }
    
    // Call API
    var response = await _httpClient.PostAsync("", content);
    var result = await response.Content.ReadAsStringAsync();
    
    // Cache the result
    if (_aiSettings.EnableCaching)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(_aiSettings.CacheDurationMinutes));
        _cache.Set(cacheKey, result, cacheOptions);
    }
    
    return result;
}
```

