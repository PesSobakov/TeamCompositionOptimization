namespace TeamCompositionOptimizationApi.Models.Database;

public class OptIndicator
{
    public int Id { get; set; }
    public double Value { get; set; } = 0;
    public double Deviation { get; set; } = 0;
    public double Weight { get; set; } = 0;
}
