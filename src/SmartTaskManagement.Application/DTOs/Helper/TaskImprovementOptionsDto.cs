namespace SmartTaskManagement.Application.DTOs.Helper
{
    public class TaskImprovementOptionsDto
    {
        public bool CorrectGrammar { get; set; } = true;
        public bool ImproveClarity { get; set; } = true;
        public bool MakeProfessional { get; set; } = true;
        public bool ExpandDescription { get; set; } = true;
        public bool MakeActionable { get; set; } = true;
        public string? Tone { get; set; }
    }
}