using PipelineOriented.Continuations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PipelineOriented.PipeLine
{
    public static class PipeLineComposer
    {
        public static Func<PipeType, string, string, object>? OnBeforeExecution = (pipe, name, correlation) => null;
        public static Action<PipeType, string, string, object>? OnAfterExecution = (pipe, name, correlation, resultBefore) => { };



        public static async Task<BaseResponseDto<TGlobalIn, TOut>> Handle<TGlobalIn, TLocalIn, TOut>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task<BaseResponseDto<TGlobalIn, TOut>>> func)
        {
            var result = await input;
            if (result.IsSuccess.HasValue)
            {
                result.SetExecutionTerminationFlag();
                return result.WithNewInterMethodResult(default(TOut));
            }

            var onBeforeResult = OnBeforeExecution(PipeType.UseCase, name, result.CorrelationName);
            var resultFunc = await func(await input);
            OnAfterExecution(PipeType.UseCase, name, result.CorrelationName, onBeforeResult);

            return resultFunc;
        }

        public static async Task<BaseResponseDto<TGlobalIn, TOut>> Handle<TGlobalIn, TLocalIn, TOut>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, bool toggleOff, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task<BaseResponseDto<TGlobalIn, TOut>>> func)
        {
            var result = await input;

            if (toggleOff)
            {
                if (result.InterMethodsResult != null && result.InterMethodsResult.Data is TOut internalResult)
                {
                    return result.WithNewInterMethodResult(internalResult);
                }
                else
                {
                    return result.WithNewInterMethodResult(default(TOut));
                }
            }

            if (result.IsSuccess.HasValue)
            {
                result.SetExecutionTerminationFlag();
                return result.WithNewInterMethodResult(default(TOut));
            }

            var onBeforeResult = OnBeforeExecution(PipeType.UseCase, name, result.CorrelationName);
            var resultFunc = await func(await input);
            OnAfterExecution(PipeType.UseCase, name, result.CorrelationName, onBeforeResult);

            return resultFunc;
        }
        public static async Task<BaseResponseDto<TGlobalIn>> End<TGlobalIn, TLocalIn>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task> func)
        {
            var result = await input;
            var onBeforeResult = OnBeforeExecution(PipeType.End, name, result.CorrelationName);            
            await func(result);
            OnAfterExecution(PipeType.End, name, result.CorrelationName,onBeforeResult);
            return result.ToFinal();

        }

        public static async Task<BaseResponseDto<TIn, TIn>> Start<TIn>(this BaseResponseDto<TIn> input, string name = null)
        {
            var result = new BaseResponseDto<TIn, TIn>(input.Data);
            result.SetPipeLineCorrelationName(name, Guid.NewGuid().ToString());            
            var onBeforeResult = OnBeforeExecution(PipeType.Feature, name, result.CorrelationName);            
            OnAfterExecution(PipeType.Feature, name,result.CorrelationName, onBeforeResult);


            return result;
        }
    }

    public enum PipeType
    {
        None = 0,
        Feature = 1,
        UseCase = 2,
        End = 3
    }
}
