using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class IndicatorDto
    {
        public CompetencyDto Competency { get; set; } = new();
        public int CompetencyId { get; set; }
        public double Value { get; set; } = 0;
        public double Deviation { get; set; } = 0;
        public double Weight { get; set; } = 0;
    }
}
