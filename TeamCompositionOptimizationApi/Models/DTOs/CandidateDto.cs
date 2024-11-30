using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.DTOs
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<CandidateCompetencyDto> Competencies { get; set; } = new();
        public double WorkingTime { get; set; } = 0;
        public double Salary { get; set; } = 0;
    }
}
