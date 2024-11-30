using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class OptimizationResultDto
    {
        public int Id { get; set; }
        public List<TeamOptionDto> TeamOptions { get; set; } = new();
        public List<IndicatorDto> Indicators { get; set; } = new();
        public List<CandidateDto> Candidates { get; set; } = new();
        public double Threshold { get; set; } = 0;
        public double Budget { get; set; } = 0;
        public double Laboriousness { get; set; } = 0;
        public double Time { get; set; } = 0;
    }
}
