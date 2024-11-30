namespace TeamCompositionOptimizationApi.Models.Database;

public class CandidateCompetency
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public int CompetencyId { get; set; }
    public Candidate Candidate { get; set; } = new();
    public Competency Competency { get; set; } = new();
    public double Value { get; set; } = 0;
    public double DeviationLeft { get; set; } = 0;
    public double DeviationRight { get; set; } = 0;
}
