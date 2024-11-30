using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class SelectDto
    {
        public List<IndicatorDto> Indicators { set; get; } = [];
        public List<CandidateDto> Candidates { set; get; } = [];
    }
}
