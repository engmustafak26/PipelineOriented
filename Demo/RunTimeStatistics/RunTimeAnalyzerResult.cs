namespace Demo.RunTimeStatistics
{
    public class RunTimeAnalyzerResult
    {
        public string FeatureName { get; set; }
        public string CorrelationId { get; set; }
        public DateTime StartDate { get; set; }
        public List<RunTimeAnalyzerUseCaseResult> UseCasesResults { get; set; } = new List<RunTimeAnalyzerUseCaseResult>();
    }

    public class RunTimeAnalyzerUseCaseResult
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public long ConsumedMemoryInBytes { get; set; }
    }
}
