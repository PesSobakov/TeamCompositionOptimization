namespace TeamCompositionOptimizationApi.Models.Database;

public class SubscriptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MaxCandidatesCount { get; set; } = 0;
    public int MaxCompetenciesCount { get; set; } = 0;
}
