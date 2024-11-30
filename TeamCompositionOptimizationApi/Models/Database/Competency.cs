namespace TeamCompositionOptimizationApi.Models.Database;

public class Competency
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<CandidateCompetency> CandidateCompetencies { get; set; } = new();
}
