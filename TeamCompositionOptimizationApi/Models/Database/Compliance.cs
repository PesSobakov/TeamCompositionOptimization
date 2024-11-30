namespace TeamCompositionOptimizationApi.Models.Database;

public class Compliance
{
    public int Id { get; set; }
    public int CompetencyId { get; set; }
    public Competency Competency { get; set; } = new();
    public double Value { get; set; } = 0;
    public double Membership { get; set; } = 0;
}
