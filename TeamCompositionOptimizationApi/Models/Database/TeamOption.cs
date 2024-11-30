namespace TeamCompositionOptimizationApi.Models.Database;

public class TeamOption
{
    public int Id { get; set; }
    public double Criteria1 { get; set; } = 0;
    public double Criteria2 { get; set; } = 0;
    public double Cost { get; set; } = 0;
    public double TeamworkTime { get; set; } = 0;
    public List<Candidate> Candidates { get; set; } = new();
    public List<Compliance> MaxCompetencies { get; set; } = new();
    public List<OptimizationResult> OptimizationResults { get; set; } = new();
}
