using Demo.Abstractions;
using Demo.DTO.Requests;
using Demo.RunTimeStatistics;
using Microsoft.AspNetCore.Mvc;
using PipelineOriented.Continuations;
using System.ComponentModel;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CodeAnalyzerController : ControllerBase
    {





        [HttpPost("ExtractFeatureWithUseCases")]
        public async Task<IActionResult> ExtractFeatureWithUseCases(BaseRequestDto<ExtractFeatureWithUseCasesRequestDto> request)
        {
            var results = await StaticAnalyzer.Anaylzer.Analyze(request.Data.SolutionPath, request.Data.MsBuildPath);

            return Ok(results);
        }


        [HttpPost("RunTimeStatistics")]
        public async Task<IActionResult> RunTimeStatistics()
        {

            return Ok(StatisticsStorage.StatisticsStorageDictionary.Values
                                       .GroupBy(x => x.FeatureName)
                                       .SelectMany(x => x.SelectMany(y => y.UseCasesResults))
                                       .GroupBy(x => x.Name)
                                       .Select(x => new
                                       {
                                           Name = x.Key,
                                           Count = x.Count(),
                                           MemoryInBytes = x.Sum(y => y.ConsumedMemoryInBytes),
                                           ElapsedTime = TimeSpan.FromTicks(x.Sum(y => y.ElapsedTime.Ticks))
                                       })

                                       .ToList());
        }

        [HttpPost("RunTimeDetailsStatistics")]
        public async Task<IActionResult> RunTimeDetailsStatistics()
        {

            return Ok(StatisticsStorage.StatisticsStorageDictionary.Values
                                       .GroupBy(x => x.FeatureName)
                                       .ToList());
        }


    }
    public class ExtractFeatureWithUseCasesRequestDto
    {

        [DefaultValue(@"D:\Work\Me\PipelineOriented\PipelineOriented.sln")]
        public string SolutionPath { get; set; }
        [DefaultValue(@"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin")]
        public string MsBuildPath { get; set; }
    }
}
