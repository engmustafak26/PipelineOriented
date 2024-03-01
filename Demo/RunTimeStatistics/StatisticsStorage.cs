namespace Demo.RunTimeStatistics
{
    public static class StatisticsStorage
    {
        public static Dictionary<(string Correlation, string Feature), RunTimeAnalyzerResult> StatisticsStorageDictionary = new Dictionary<(string Correlation, string Feature), RunTimeAnalyzerResult>();
    }
}
