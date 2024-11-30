namespace TeamCompositionOptimizationApi.Models.Database;

public class OptimizationResult
{
    public int Id { get; set; }
    public List<TeamOption> TeamOptions { get; set; } = new();
    public List<Indicator> Indicators { get; set; } = new();
    public List<Candidate> Candidates { get; set; } = new();
    public double Threshold { get; set; } = 0;
    public double Budget { get; set; } = 0;
    public double Laboriousness { get; set; } = 0;
    public double Time { get; set; } = 0;
}
