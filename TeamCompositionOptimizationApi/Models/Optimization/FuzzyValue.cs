namespace TeamCompositionOptimizationApi.Models.Optimization;

public class FuzzyValue
{
    public int Id { get; set; }
    public double Value { get; set; } = 0;
    public double DeviationLeft { get; set; } = 0;
    public double DeviationRight { get; set; } = 0;
}
