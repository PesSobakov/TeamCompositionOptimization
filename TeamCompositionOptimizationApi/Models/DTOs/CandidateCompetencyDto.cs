using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class CandidateCompetencyDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int CompetencyId { get; set; }
        public double Value { get; set; } = 0;
        public double DeviationLeft { get; set; } = 0;
        public double DeviationRight { get; set; } = 0;
    }
}
