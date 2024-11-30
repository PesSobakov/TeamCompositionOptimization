using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class TeamOptionDto
    {
        public double Criteria1 { get; set; } = 0;
        public double Criteria2 { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double TeamworkTime { get; set; } = 0;
        public List<CandidateDto> Candidates { get; set; } = new();
        public List<ComplianceDto> MaxCompetencies { get; set; } = new();

    }
}
