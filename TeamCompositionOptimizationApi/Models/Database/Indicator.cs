namespace TeamCompositionOptimizationApi.Models.Database;

public class Indicator
{
    public int Id { get; set; }
    public int CompetencyId { get; set; }
    public Competency Competency { get; set; } = new();
    public double Value { get; set; } = 0;
    public double Deviation { get; set; } = 0;
    public double Weight { get; set; } = 0;
    public List<OptimizationResult>? OptimizationResults { get; set; }

}
