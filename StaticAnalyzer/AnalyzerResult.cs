namespace StaticAnalyzer
{
    public class AnalyzerResult
    {
        public string Project { get; set; }
        public string Class { get; set; }
        public string DocumentPath { get; set; }
        public string MethodName { get; set; }
        public UseCaseResult[] UseCases { get; set; }
    }
    public class UseCaseResult
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsToggledOff { get; set; }
    }
}
