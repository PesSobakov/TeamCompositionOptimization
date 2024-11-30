using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class OptimizationDto
    {
        public List<IndicatorDto> Indicators { set; get; } = [];
        public List<CandidateDto> Candidates { set; get; } = [];
        public double Threshold { set; get; }
        public double Budget { set; get; }
        public double Laboriousness { set; get; }
        public double Time { set; get; }

    }
}
