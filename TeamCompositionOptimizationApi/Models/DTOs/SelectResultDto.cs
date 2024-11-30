using TeamCompositionOptimizationApi.Models.Database;
using TeamCompositionOptimizationApi.Models.Optimization;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class SelectResultDto
    {
        public List<IndicatorDto> Indicators { get; set; } = new();
        public List<CandidateDto> Candidates { get; set; } = new();
        public List<GeneralizedCompetence> GeneralizedCompetences { get; set; } = new();
    }
}
