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