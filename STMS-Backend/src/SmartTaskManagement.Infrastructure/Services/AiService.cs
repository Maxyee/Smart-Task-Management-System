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
            string apiKey = "AQ.Ab8RN6ILISeXYKTqII76CCMUy4lDucJvpbA7ZY53lZevWn0SPQ"; 
            string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Add the Authorization header
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);

            // Send request
            var response = await _httpClient.PostAsync(apiUrl, content);

            // Handle Errors
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Google API Error {response.StatusCode}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);

            // Extract text
            return jsonResponse.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
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