namespace TeamCompositionOptimizationApi.Models.Optimization
{
    public class OptCandidate
    {
        public int Id { get; set; }
        public List<FuzzyValue> Competencies { get; set; } = new();
        public double WorkingTime { get; set; } = 0;
        public double Salary { get; set; } = 0;

    }
}
