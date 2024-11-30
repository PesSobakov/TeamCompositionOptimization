using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi.Models.Optimization
{
    public class OptTeamOption
    {
        public int Id { get; set; }
        public double Criteria1 { get; set; } = 0;
        public double Criteria2 { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double TeamworkTime { get; set; } = 0;
        public List<OptCandidate> Candidates { get; set; } = new();
        public List<OptCompliance> MaxCompetencies { get; set; } = new();
    }
}
