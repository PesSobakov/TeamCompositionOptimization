namespace TeamCompositionOptimizationApi.Models.Database;

public class Candidate
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<CandidateCompetency> Competencies { get; set; } = new();
    public double WorkingTime { get; set; } = 0;
    public double Salary { get; set; } = 0;
    public List<OptimizationResult>? OptimizationResults { get; set; }
    public List<TeamOption>? TeamOptions { get; set; }
}
