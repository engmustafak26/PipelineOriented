using PipelineOriented.Continuations;
using PipelineOriented.PipeLine;
using System.Diagnostics;

namespace Demo.RunTimeStatistics
{
    public static class PipeLineInspectorHandler
    {
        public static void SetPipelineInspectionMetrics()
        {
            PipeLineComposer.OnBeforeExecution = (pipe, name, correlation) =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var allocatedThreadBytes = System.GC.GetAllocatedBytesForCurrentThread();
                return new RunTimeMetrics
                {
                    StartDate = DateTime.Now,
                    AllocatedBytes = allocatedThreadBytes,
                    Watch = stopwatch
                };
            };

            PipeLineComposer.OnAfterExecution = (pipe, name, correlation, resultBefore) =>
            {
                var runTimeMetricObject = resultBefore as RunTimeMetrics;

                runTimeMetricObject.Watch.Stop();
                var allocatedThreadBytes = System.GC.GetAllocatedBytesForCurrentThread();

                var correlationSematics = BaseResponseDto<object, object>.GetCorrelationNameSematics(correlation);
                if (!StatisticsStorage.StatisticsStorageDictionary.ContainsKey(correlationSematics))
                {
                    StatisticsStorage.StatisticsStorageDictionary.Add(correlationSematics, new RunTimeAnalyzerResult
                    {
                        FeatureName = correlationSematics.Feature,
                        CorrelationId = correlationSematics.Correlation,
                        StartDate = runTimeMetricObject.StartDate,
                        UseCasesResults = new List<RunTimeAnalyzerUseCaseResult>()
                            {
                                new RunTimeAnalyzerUseCaseResult
                                {
                                     StartDate=runTimeMetricObject.StartDate,
                                     ConsumedMemoryInBytes=allocatedThreadBytes- runTimeMetricObject.AllocatedBytes,
                                     ElapsedTime=runTimeMetricObject.Watch.Elapsed,
                                     Name=(pipe.ToString()) + ": "+name,
                                }
                            }
                    });
                    return;
                }

                StatisticsStorage.StatisticsStorageDictionary[correlationSematics].UseCasesResults.Add(
                    new RunTimeAnalyzerUseCaseResult
                    {
                        StartDate = runTimeMetricObject.StartDate,
                        ConsumedMemoryInBytes = allocatedThreadBytes - runTimeMetricObject.AllocatedBytes,
                        ElapsedTime = runTimeMetricObject.Watch.Elapsed,
                        Name = (pipe.ToString()) + ": " + name,
                    });
            };
        }
    }
    public class RunTimeMetrics
    {
        public Stopwatch Watch { get; set; }
        public DateTime StartDate { get; set; }
        public long AllocatedBytes { get; set; }
    }
}



