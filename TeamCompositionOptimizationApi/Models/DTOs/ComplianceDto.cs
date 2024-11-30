using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class ComplianceDto
    {
        public int CompetencyId { get; set; }
        public double Value { get; set; } = 0;
        public double Membership { get; set; } = 0;

    }
}
