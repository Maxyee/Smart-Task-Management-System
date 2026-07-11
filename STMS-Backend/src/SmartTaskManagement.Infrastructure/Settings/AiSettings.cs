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